using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

// This class converts GameData.cs into a compressed format and writes it to file
// (and the reverse i.e. reads from file and decompresses into GameData.cs)
public class FileDataHandler
{
    // directory path of where data is saved
    private string dataDirPath = "";
    // name of file to save to
    private string dataFileName = "";
    // boolean to choose if save data is encrypted
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";
    private readonly string backupExtension = ".bak";

    // create a file directory path and decide to use encryption or not
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption) 
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }


    // load game using profileId and with option for a backup file
    public GameData Load(string profileId, bool allowRestoreFromBackup = true) 
    {
        // base case - if the profileId is null, return right away
        if (profileId == null) 
        {
            return null;
        }

        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath)) 
        {
            try 
            {
                // load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // optionally decrypt the data
                if (useEncryption) 
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // deserialize the data from Json back into the C# object, populate loaded data variable
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e) 
            {
                // since we're calling Load(..) recursively, we need to account for the case where
                // the rollback succeeds, but data is still failing to load for some other reason,
                // which without this check may cause an infinite recursion loop.
                if (allowRestoreFromBackup) 
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + e);
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                    {
                        // try to load again recursively
                        loadedData = Load(profileId, false);
                    }
                }
                // if we hit this else block, one possibility is that the backup file is also corrupt
                else 
                {
                    Debug.LogError("Error occured when trying to load file at path: " 
                        + fullPath  + " and backup did not work.\n" + e);
                }
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string profileId) 
    {
        // base case - if the profileId is null, return right away
        if (profileId == null) 
        {
            return;
        }

        // use Path.Combine to account for different operating systems having different path separators
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        string backupFilePath = fullPath + backupExtension;
        try 
        {
            // create directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json (format the game data with optional true parameter)
            string dataToStore = JsonUtility.ToJson(data, true);

            // optionally encrypt the data
            if (useEncryption) 
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // write the serialized data to the file
            // "using" blocks ensure connection to file is closed when reading or writing to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                // pass in stream 
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    // pass in data to be written to file
                    writer.Write(dataToStore);
                }
            }

            // verify the newly saved file can be loaded successfully
            GameData verifiedGameData = Load(profileId);
            // if the data can be verified, back it up
            if (verifiedGameData != null) 
            {   
                SaveableManager.instance.printMsg($"Game data successfully saved to: {fullPath}");
                File.Copy(fullPath, backupFilePath, true);
            }
            // otherwise, something went wrong and we should throw an exception
            else 
            {
                throw new Exception("Save file could not be verified and backup could not be created.");
            }

        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public void Delete(string profileId) 
    {
        // if the profileId is null, return right away
        if (profileId == null) 
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try 
        {
            // ensure the data file exists at this path before deleting the directory
            if (File.Exists(fullPath)) 
            {
                // delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else 
            {
                Debug.LogWarning("Tried to delete profile data, but data was not found at path: " + fullPath);
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Failed to delete profile data for profileId: " 
                + profileId + " at path: " + fullPath + "\n" + e);
        }
    }

    // load profiles on computer using game data
    public Dictionary<string, GameData> LoadAllProfiles() 
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        // loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos) 
        {
            string profileId = dirInfo.Name;

            // defensive programming - check if the data file exists
            // if it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: "
                    + profileId);
                continue;
            }

            // load the game data for this profile and put it in the dictionary
            GameData profileData = Load(profileId);
            // defensive programming - ensure the profile data isn't null,
            // because if it is then something went wrong and we should let ourselves know
            if (profileData != null) 
            {
                profileDictionary.Add(profileId, profileData);
            }
            else 
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
            }
        }

        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileId() 
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData) 
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            // skip this entry if the gamedata is null
            if (gameData == null) 
            {
                continue;
            }

            // if this is the first data we've come across that exists, it's the most recent so far
            if (mostRecentProfileId == null) 
            {
                mostRecentProfileId = profileId;
            }
            // otherwise, compare to see which date is the most recent
            else 
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                // the greatest DateTime value is the most recent
                if (newDateTime > mostRecentDateTime) 
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }

    // the below is a simple implementation of XOR encryption
    private string EncryptDecrypt(string data) 
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++) 
        {
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }

    private bool AttemptRollback(string fullPath) 
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;
        try 
        {
            // if the file exists, attempt to roll back to it by overwriting the original file
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back to backup file at: " + backupFilePath);
            }
            // otherwise, we don't yet have a backup file - so there's nothing to roll back to
            else 
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to roll back to backup file at: " 
                + backupFilePath + "\n" + e);
        }

        return success;
    }
}
