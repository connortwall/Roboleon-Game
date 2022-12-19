using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class holds the attributes  
[Serializable]
public class GameManagerData : MonoBehaviour, ISaveable
{

    // implemented LoadData function for ISaveable interface; loads door information for each door from gameData
    public void LoadData(GameData gameData)
    {
       var gameManagerSaveData = gameData.savedGameManagerData.First();
       GMSaveData saveData = new GMSaveData(
            gameManagerSaveData.logKeeper,
            gameManagerSaveData.toolKeeper,
            gameManagerSaveData.playerPosition,
            gameManagerSaveData.roomAudio,
            gameManagerSaveData.taskIndex,
            gameManagerSaveData.currentTool
       );
        GameManager.instance.LoadGame(saveData);
    }

    // implemented SaveData function for ISaveable interface; saves door information for each door to savedDoorData List in gameData
    public void SaveData(GameData gameData)
    {
        var gameManagerSaveData = new GameData.GameManagerSaveData();
        GMSaveData saveData = GameManager.instance.SaveGame();
        // add object data to save data
        gameManagerSaveData.logKeeper = saveData.logKeeper;
        gameManagerSaveData.toolKeeper = saveData.toolKeeper;
        gameManagerSaveData.playerPosition = saveData.playerPosition;
        gameManagerSaveData.roomAudio = saveData.roomAudio;
        gameManagerSaveData.taskIndex = saveData.taskIndex;
        gameManagerSaveData.currentTool = saveData.currentTool;
        
        // add save data to list of door save data
        gameData.savedGameManagerData.Add(gameManagerSaveData);
    }
}

//GMSaveData saveData;