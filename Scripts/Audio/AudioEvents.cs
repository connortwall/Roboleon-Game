using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*** this script holds the references to all audio event names ***/

[System.Serializable]
public class AudioEvents: MonoBehaviour
{
    [HideInInspector]
    public static AudioEvents instance;
    
    [Header("Door Sounds")]
    [Tooltip("Leave events that have not been implemented blank")]
    public string StartDoorOpen = "";
    public string FinishDoorOpen = "";
    public string StartDoorClose = "";
    public string FinishDoorClose = "";

    [Header("Interactable Objects")]
    public string scannerBeep = "";
    public string enabledButtonPress = "";
    public string disabledButtonPress = "";
    public string brokenButtonPress = "";
    public string pickUpLog = "";
    public string pickUpTool = "";

    [Header("Tools")]
    public string firehoseStart = "";
    public string firehoseStop = "";
    public string screwdriverAction = "";
    public string screwdriverActionStop = "";
    public string magnetAction = "";
    public string magnetActionStop = "";
    public string putOutFlame = "";
    public string changeTool = "";
    public string stopAllTools = "";

    [Header("CharacterController")]
    public string rtpcCharacterSpeed = "";
    public string rtpcTailSpeed = "";
    public string characterJump = "";
    public string characterLand = "";

    [Header("UI")]
    public string menuSelection = "";
    public string menuButtonPress = "";
    public string startGameButton = "";
    public string playLogButton = "";
    public string pauseAudio = "";
    public string resumeAudio = "";

    [Header("Music & Scenes")]
    public string mainMenu = "";
    public string gameMusic = "";
    public string logScreen = "";
    public string pauseMenu = "";
    public string loadingScreen = "";
 
    public void StartAudioEvents(){
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 
    }

}
