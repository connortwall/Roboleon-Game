using System;
using Puppet.ReviewScripts;
using UnityEngine;
using UnityEngine.UIElements;

// This class holds the attributes  
[Serializable]
public class ScrewPanelSaveData : MonoBehaviour, ISaveable
{
    // ScrewPanelPlatform save data variables
    public Vector3 testPlatformComponentTransformPosition;
    public GameObject platformObject;
    
    // unique id for screw panel
    private string id = Guid.NewGuid().ToString();

    private void Start()
    {
        testPlatformComponentTransformPosition = platformObject.transform.position;
    }


    //implemented LoadData for ISaveable interface; loads screw panel information for each door from gameData
    public void LoadData(GameData gameData)
    {
        //load data from each screw panel in savedScrewPanelData List in gameData
        foreach (var screwPanelData in gameData.savedScrewPanelData)
            //check for matching panel Unique id to load from
            if (screwPanelData.id == id)
            {
                // Debug.Log("found screwpanel");
                platformObject.transform.position = screwPanelData.testPlatformComponentTransformPosition;
                break;
            }
        
        //Debug.Log(JsonUtility.ToJson(gameData));
    }

    // implemented SaveData for ISaveable interface; saves door information for each door to savedButtonData List in gameData
    public void SaveData(GameData gameData)
    {
        // create new gameData to save button data 
        var screwPanelSaveData = new GameData.ScrewPanelSaveData();

        // assign id of object to id in struct data
        screwPanelSaveData.id = id;

        // add object data to save data
        screwPanelSaveData.testPlatformComponentTransformPosition = platformObject.transform.position;

        // add save data to list of button save data
        gameData.savedScrewPanelData.Add(screwPanelSaveData);
    }
}