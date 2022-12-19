using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for each object that needs to be saved
// Needs to be placed on an object to offer saving functionality
public interface ISaveable
{
    void LoadData(GameData gameData);
    
    // (Non-primitive types are automatically passed by reference) 
    void SaveData(GameData gameData);
}
