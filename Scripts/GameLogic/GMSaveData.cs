using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMSaveData
{
    public bool[] logKeeper;
    public List<int> toolKeeper;
    public Vector3 playerPosition;
    public string roomAudio;
    public int taskIndex;
    public int currentTool;

    public GMSaveData(
        bool[] logKeeper,
        List<int> toolKeeper,
        Vector3 playerPosition,
        string roomAudio,
        int taskIndex,
        int currentTool
    ){
        this.logKeeper = logKeeper;
        this.toolKeeper = toolKeeper;
        this.playerPosition = playerPosition;
        this.roomAudio = roomAudio;
        this.taskIndex = taskIndex;
        this.currentTool = currentTool;
    }
}
