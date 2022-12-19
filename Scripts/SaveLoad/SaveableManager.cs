using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
// gives nicer syntax, helps with finding data persistance object

// This class keeps track of current state of game data, organizes save and load logic
public class SaveableManager : MonoBehaviour
{
    [Header("Debugging")] 
    [SerializeField] private bool disableDataPersistence = false;
    private bool initializeDataIfNull = true;
    public bool printDebugs = false;

    private string fileName = "SaveData.json";
    private bool useEncryption = false;

    [Header("Auto Saving Configuration")] [SerializeField]
    private float autoSaveTimeSeconds = 60f;
    public bool enableAutoSave = false;

    [HideInInspector]
    private Coroutine autoSaveCoroutine;

    // need a data handler to save game
    [HideInInspector]
    private FileDataHandler dataHandler;
    // list of saveable objects
    [HideInInspector]
    private List<ISaveable> dataPersistenceObjects;
    [HideInInspector]
    private GameData gameData;
    [HideInInspector]
    private string selectedProfileId = null;
    private string defaultProfileName = "SavedGame";

    

    // only want one SaveableManager in scene (Singleton), can get instance publically but only modify it here
    public static SaveableManager instance { get; private set; }

    public void StartSM()
    {
        // if there is an instance of this class error
        if (instance != null)
        {
            printMsg("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);

        if (disableDataPersistence) Debug.LogWarning("Data Persistence is currently disabled!");

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitializeSelectedProfileId();
    }

    private void UpdateSM()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            printMsg("Saving Game ");
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            printMsg("Loading Game ");
            LoadGame();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            printMsg("New Game ");
            NewGame();
        }

        if (Input.GetKeyDown(KeyCode.P)) printMsg(JsonUtility.ToJson(gameData));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        //LoadGame();

        // start up the auto saving coroutine
        if (!enableAutoSave) return;
        if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        selectedProfileId = newProfileId;
        // load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }

    public bool CanLoadGame(){
        // return if data persistence is disabled
        if (disableDataPersistence) return false;

        // load any saved data from a file using the data handler
        gameData = dataHandler.Load(selectedProfileId);

        // check if game data actually exits
        if (gameData != null) return true;
        else return false;
    }

    public void DeleteProfileData(string profileId)
    {
        // delete the data for this profile id
        dataHandler.Delete(profileId);
        // initialize the selected profile id
        InitializeSelectedProfileId();
        // reload the game so that our data matches the newly selected profile id
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (string.IsNullOrWhiteSpace(selectedProfileId)){
            selectedProfileId = defaultProfileName;
        }
    }

    // create new GameData object
    public void NewGame()
    {
        gameData = new GameData();
    }

    // loads game by passing each object in the ISaveable interface and calling its LoadData function in its script from GameData
    public bool LoadGame()
    {
        // return if data persistence is disabled
        if (disableDataPersistence) return false;

        // load any saved data from a file using the data handler
        gameData = dataHandler.Load(selectedProfileId);

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        

        // if no data can be loaded, stop
        if (gameData == null){
            if (initializeDataIfNull){
                NewGame();
                printMsg("GameData was null; New Game created");
            }
            else {
                printMsg("Error loading:No data was found. A New Game needs to be started before data can be loaded.");
                return false;
            } 
        }

        //update list of objects in the scene
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (dataPersistenceObjects == null){
            printMsg("Error loading: there are no data persistence objects in scene");
            return false;
        }
        // push the loaded data to all other scripts that need it
        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        printMsg("Loading game from disk completed.");
        return true;
    }


    // saves game by passing each object in the ISaveable interface and calling its SaveData function in its script
    // these scripts modify GameData object 
    public void SaveGame()
    {
        // return right away if data persistence is disabled
        if (disableDataPersistence) return;

        // if we don't have any data to save, log a warning here
        if (gameData == null)
        {
            if (initializeDataIfNull){
                printMsg("No save data found, creating a new game");
                NewGame();
            }
            else {
                Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
                return;
            }
        }
        

        // load in new objects from scene to be saved
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        gameData.savedGameManagerData.Clear();
        gameData.savedScrewPanelData.Clear();

        // pass the data to other scripts so they can update it
        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        //printMsg("Game Saved");

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = DateTime.Now.ToBinary();

        // save that data to a file using data handler
        dataHandler.Save(gameData, selectedProfileId);
    }
 

    private List<ISaveable> FindAllDataPersistenceObjects()
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        // must extend from monobehavior to function to search completely for all objects in IEnumerable
        var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<ISaveable>();

        return new List<ISaveable>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            printMsg("Auto Saved Game");
        }
    }

    public void printMsg(string msg){
        if (!printDebugs) return;
        Debug.Log(msg);
    }
}