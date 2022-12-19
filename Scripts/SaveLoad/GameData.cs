using System;
using System.Collections.Generic;
using UnityEngine;

// This class stores state of game data, serializable allows
[Serializable]
public class GameData
{
    // time when game last updated
    public long lastUpdated;
    
    // game variables to be saved
    public List<GameManagerSaveData> savedGameManagerData = new();
    public List<ScrewPanelSaveData> savedScrewPanelData = new();


    // default values for game when there's no data to load
    public GameData()
    {
    }

    // need a struct and list of each saveable object type with variables to be saved; these variables 
    //will be loaded and saved within object scripts with ISaveable
    
    // screw panel save data variables
    [Serializable]
    public struct ScrewPanelSaveData
    {
        public string id;
        public Vector3 testPlatformComponentTransformPosition;
        // public bool requiredByTask;
        // public bool freezeAfterTaskCompeleted;
        // public Transform rotatorTransform;
        // public Vector3 rotatorRotation;
    }

    [Serializable]
    public struct GameManagerSaveData
    {
        public bool[] logKeeper;
        // toolkeeper list might be empty if no tools picked up by player
        public List<int> toolKeeper;
        public Vector3 playerPosition;
        public string roomAudio;
        public int taskIndex;
        public int currentTool;
    }
}