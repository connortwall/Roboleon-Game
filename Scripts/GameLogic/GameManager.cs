using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Puppet.ReviewScripts;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    [Header("General Configuration")]
    public string playerTag = "Player";
    public PlayerInput control;
    public float minTimeOnLoadingScreen = 5;
    
    [Header("Managers")]
    public GameObject UiManagerObject;
    public GameObject TaskManagerObject;
    public GameObject AudioManagerObject;
    public SaveableManager saveableManager;

    [Header("Cinematics")]
    public PlayableDirector openingCinematicDirector;
    
    [Header("Debug")]
    public bool forceNewGame = false;
    public bool EnableDebugCommands = true;
    public bool printDebugs = true;
    public bool unlockTools = false;
    public bool disableCinematics = false;
    public bool cubertInScene = false;

    [HideInInspector] 
    public static GameManager instance;

    //player stuff
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public PuppetController puppetController;
    [HideInInspector]
    public Vector3 playerPosition;

    //logs
    [HideInInspector]
    public GameObject currentLog;
    [HideInInspector]
    public bool[] logKeeper;

    //attachment stuff
    [HideInInspector]
    public GameObject attachmentInRange;
    [HideInInspector]
    public List<ArmMode> toolKeeper;
    [HideInInspector]
    public ArmMode currentAttachment;

    //buttons
    private InputAction northButton;
    private InputAction eastButton;
    private InputAction southButton;
    private InputAction westButton;
    private InputAction dpadUp;
    private InputAction dpadRight;
    private InputAction dpadDown;
    private InputAction dpadLeft;
    private InputAction optionsButton;

    //save/load stuff
    [HideInInspector]
    public bool newGame;
    [HideInInspector]
    public bool loading;
    [HideInInspector]
    public float timeSinceStart;


    void Awake() 
    {
        // printDebugs = true;
        // forceNewGame = false;

        loading = true;
        timeSinceStart = 0;
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 

        //find player and puppetcontroller
        if (printDebugs) print ("looking for player...");
        player = GameObject.FindWithTag(playerTag);
        if (player == null) throw new Exception("player not found");
        else if (printDebugs) print("player found");
        if (!cubertInScene){
            PuppetController[] pcontrollers = FindObjectsOfType(typeof(PuppetController)) as PuppetController[];
            Assert.IsTrue(pcontrollers!= null && pcontrollers.Length > 0);
            puppetController = pcontrollers[0];
            //disable character movement
            puppetController.IsPlayerControllable = false;
        }

        //Start the managers
        /*** SAVEABLE MANAGER ***/
        if (saveableManager == null) throw new Exception("Save Manager not found in scene!");
        saveableManager.StartSM();
        /*** AUDIO MANAGER ***/
        AudioManager audioManager = AudioManagerObject.GetComponent<AudioManager>();
        if (audioManager == null) throw new Exception("Audio Manager not found in scene!");
        audioManager.soundEnabled = false;
        audioManager.StartAM();
        //update the room audio
        string roomAudio = AudioManager.instance.initWwiseEvent;
        //TODO: REPLACE ABOVE WITH LOADING ROOM AUDIO FROM SAVE FILE
        if (!roomAudio.Equals(AudioManager.instance.initWwiseEvent)){
            AudioManager.instance.currentRoomEvent = roomAudio;
            AudioEvent(roomAudio);
        }
        /*** UI MANAGER ***/
        UIManager uiManager = UiManagerObject.GetComponent<UIManager>();
        if (uiManager == null) throw new Exception("UI Manager not found in scene!");
        uiManager.StartUI();
        /*** TASK MANAGER ***/
        TaskManager taskManager = TaskManagerObject.GetComponent<TaskManager>();
        if (taskManager == null) throw new Exception("Task Manager not found in scene!");
        taskManager.StartTM();
        

        /*** CONTROLLER SETUP ***/
        Assert.IsNotNull(control);
        northButton = control.actions["NorthButton"];
        eastButton = control.actions["EastButton"];
        southButton = control.actions["SouthButton"];
        westButton = control.actions["WestButton"];
        dpadUp = control.actions["DPadUp"];
        dpadRight = control.actions["DPadRight"];
        dpadDown = control.actions["DPadDown"];
        dpadLeft = control.actions["DPadLeft"];
        optionsButton = control.actions["Options"];


        //set up log keeper (defaults all values to false)
        logKeeper = new bool[AudioManager.instance.logEvents.Count];
        // set up tool keeper
        toolKeeper = new List<ArmMode>();
        //change tool to empty
        ChangeAttachment(ArmMode.EmptyAttachment);
        if (unlockTools){
            toolKeeper.Add(ArmMode.Magnet);
            toolKeeper.Add(ArmMode.FireExtinguisher);
            toolKeeper.Add(ArmMode.ScrewDriver);
        }


        //refresh all text
        TranslationManager.instance.languageChangeEvent.Invoke();


        //save and load + keep track of debug command
        if (forceNewGame || !PlayerPrefs.HasKey("newGame")){
            printMsg("Debug command forced a new game.");
            newGame = true;
        }
        else {
            newGame = (PlayerPrefs.GetInt("newGame") == 1);
        }

        if (newGame) {
            printMsg("Saving a new game...");
            SaveableManager.instance.NewGame();
            SaveableManager.instance.SaveGame();
            float waitTime = minTimeOnLoadingScreen - timeSinceStart;
            if (waitTime < 0) waitTime = 0;
            Time.timeScale = 1;
            Invoke("StartGame", waitTime);
        }
        else {
            printMsg("Loading saved game...");
            SaveableManager.instance.LoadGame();
        }
    }

    /** the game manager has the only true update function in the game
     * it will call the update functions of the other managers
     * this ensures execution order
     */
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        //tick managers
        TaskManager.instance.UpdateTM();
        AudioManager.instance.UpdateAM();



        /******* CONTROLLER COMMANDS *******/
        if (loading) return;
        /*** BUTTONS ***/
        if(northButton.triggered){
            UIManager.instance.ToggleLogScreen();
        }
        if(eastButton.triggered){
            UIManager.instance.BackPress();
        }
        if(southButton.triggered){
            if (attachmentInRange != null) UnlockAttachment();
            else if (currentLog != null) CollectLog();
        }
        if (optionsButton.triggered){
            UIManager.instance.TogglePauseGame();
        }
        /*** DPAD (in game) ***/
        if (!UIManager.instance.paused){
            if (dpadUp.triggered) ChangeAttachment(ArmMode.Magnet);
            if (dpadRight.triggered) ChangeAttachment(ArmMode.ScrewDriver);
            if (dpadLeft.triggered) ChangeAttachment(ArmMode.FireExtinguisher);
        }


        /******** DEBUG COMMANDS ********/
        if (!EnableDebugCommands) return;
        if (Input.GetKeyDown(KeyCode.Y)){ //print task list
            TaskManager.instance.DebugPrintTasks();
        }
        if (Input.GetKeyDown(KeyCode.L)){ //open log screen
            UIManager.instance.ToggleLogScreen();
        }
        if (Input.GetKeyDown(KeyCode.M)){ //start alternate ending
            TaskManager.instance.AlternateEnding();
        }
        if (Input.GetKeyDown(KeyCode.T)){ //autocomplete task
            if (TaskManager.instance.currTask != null){
                TaskManager.instance.currTask.DebugCompleteTask();
            }
            else {
                TaskManager.instance.CompleteTask();
            }
        }
    }

    //show/hide UI elements
    public void LogInRange(GameObject log){
        if (log == null){
            //disable prompt
            currentLog = null;
            UIManager.instance.ToggleLogPrompt(false);
            return;
        }

        int logNum = log.GetComponent<LogObject>().idNumber;

        //check input
        if (logNum >= logKeeper.Length || logNum < 0){
            printMsg("A log with an invalid number has been collected");
            currentLog = null;
            UIManager.instance.ToggleLogPrompt(false);
            return;
        }

        //delete the log if it already unlocked
        if (logKeeper[logNum]){
            printMsg("INCORRECT SETUP!: A log that has already been collected was found in game. It has been deleted.");
            currentLog = null;
            UIManager.instance.ToggleLogPrompt(false);
            Destroy(log);
            return;
        }

        printMsg("In range of log #" + logNum);

        //allow user to pick up log
        currentLog = log;
        UIManager.instance.ToggleLogPrompt(true); 
    }

    /** 
     * used when a player clicks the A button to collect a log
     * should never be called when no log is available to 
     * be picked up/in range
     */
    public void CollectLog(){
        Assert.IsNotNull(currentLog);
        int logNum = currentLog.GetComponent<LogObject>().idNumber;
        //verify input
        Assert.IsTrue(logNum <= logKeeper.Length);
        Assert.AreNotEqual(-1, logNum);

        //unlock log and play
        logKeeper[logNum] = true;
        printMsg("Collected log #" + logNum);
        AudioEvent(AudioEvents.instance.pickUpLog);
        UIManager.instance.CollectLog(logNum);

        //destroy log from scene!
        Destroy(currentLog);
        //reset current log and hide collection panel
        LogInRange(null);
        
    }

    //show/hide attachment prompt
    public void AttachmentInRange(GameObject armModeObj){
        attachmentInRange = armModeObj;
        if (armModeObj == null){
            //disable prompt
            UIManager.instance.ToggleAttachmentPrompt(false);
            return;
        }

        ArmMode armMode = armModeObj.GetComponent<AttachmentAssetObject>().attachmentType;
        //input checking
        Assert.AreNotEqual(armMode, ArmMode.EmptyAttachment);

        printMsg("In range of " + armMode.ToString() + " attachment");
        //allow user to pick up attachment
        UIManager.instance.ToggleAttachmentPrompt(true); 
    }

    public void UnlockAttachment(){
        if (unlockTools) return;
        //input checking
        Assert.IsNotNull(attachmentInRange);
        ArmMode armMode = attachmentInRange.GetComponent<AttachmentAssetObject>().attachmentType;
        Assert.IsFalse(toolKeeper.Contains(armMode));

        //verify that there is a task active
        Attachment attachTask = (Attachment)TaskManager.instance.currTask;
        if (attachTask.typeOfTask != TaskType.Attachment){
            printMsg("ERROR: The current task is not an attachment task");
            return;
        }
        else if (attachTask.toolToGrab.GetInstanceID() != attachmentInRange.GetInstanceID()){
            printMsg("ERROR: The tool collected is not the tool in the task.");
            return;
        }
        else {
            //finish the task!
            attachTask.CompleteTask();
        }

        //unlock tool and change to it
        toolKeeper.Add(armMode);
        ChangeAttachment(armMode);
        
        //remove asset from scene
        Destroy(attachmentInRange);
        //hide prompt
        AttachmentInRange(null);
    }

    public void ChangeAttachment(ArmMode armMode){
        //prevent execution order problems with lack of null checks in puppetcontroller
        if (loading) return;
        if (armMode == ArmMode.EmptyAttachment || toolKeeper.Contains(armMode)){
            currentAttachment = armMode;
            //play sound 
            AudioEvent(AudioEvents.instance.changeTool);
            printMsg("Changing attachment to " + armMode.ToString());
            UIManager.instance.ChangeAttachmentIcon(armMode);
            if (cubertInScene || puppetController == null) return; // (for cubert scenes)
            puppetController.ChangeTool(armMode);
        }
    }

    public void GameComplete(){
        printMsg("No more tasks, game complete");
        UIManager.instance.UpdateTaskPanel("All tasks have been completed");
    }

    void OnDestroy() {
        AudioManager.instance.OnDestroyAM();
    }



/********************* SAVE & LOAD ************************/



    public GMSaveData SaveGame(){
        //make sure character position exists
        if (playerPosition == null) UpdatePlayerPosition(); 
        //prepare toolkeeper list
        List<int> saveToolKeeper = new List<int>();
        foreach (ArmMode armMode in toolKeeper){
            saveToolKeeper.Add((int)armMode);
        }
        //package data up
        GMSaveData saveData = new GMSaveData(
            logKeeper,
            saveToolKeeper,
            playerPosition,
            AudioManager.instance.currentRoomEvent,
            TaskManager.instance.numTasksCompleted,
            (int)currentAttachment
        );
        //and sent it out!
        return saveData;
    }

    public void LoadGame(GMSaveData saveData){
        if (saveData == null) return;

        //unlock logs
        this.logKeeper = saveData.logKeeper;
        UIManager.instance.LoadUnlockedLogs();
        //delete logs in scene that have already been collected
        LogObject[] logsInScene = Resources.FindObjectsOfTypeAll(typeof(LogObject)) as LogObject[];
        if (logsInScene != null && logsInScene.Length > 0){
            foreach (LogObject log in logsInScene){
                if (log.idNumber >= logKeeper.Length) continue;
                if (logKeeper[log.idNumber]){
                    Destroy(log.gameObject);
                }
            }
        }

        //cycle through tasks
        TaskManager.instance.LoadTaskSave(saveData.taskIndex);

        //update player position and tools
        if (!cubertInScene){
            puppetController.CurrentPosition = saveData.playerPosition;
            ChangeAttachment((ArmMode)saveData.currentTool);
        }

        //get room audio
        AudioManager.instance.currentRoomEvent = saveData.roomAudio;
    
        //hide any menus that may have popped up
        UIManager.instance.HideAllPages();
        
        //set current attachment 
        currentAttachment = (ArmMode)saveData.currentTool;
        //make sure save screen is up the minimum amount of time
        float waitTime = minTimeOnLoadingScreen - timeSinceStart;
        if (waitTime < 0) waitTime = 0;
        Time.timeScale = 1;
        printMsg($"Starting game after delay of {waitTime} seconds...");
        Invoke("StartGame", waitTime);
    }

    public void StartGame(){
        printMsg("Starting game...");
        Time.timeScale = 1; 

        //start audio
        AudioManager.instance.soundEnabled = true;
        AudioEvent(AudioManager.instance.initWwiseEvent);
        AudioEvent(AudioManager.instance.currentRoomEvent);
        //turn off loading screen
        UIManager.instance.loadingScreen.SetActive(false);
        //Destroy loading screen
        Destroy(UIManager.instance.loadingScreen);

        //only play the new game cinematic if its a new game
        if (newGame && !disableCinematics){
            if (openingCinematicDirector == null){
                Debug.LogError("Opening cinematic Playable Director was not assigned.");
            }
            else {
                StartOpeningCinematic();
                return;
            }
        }
        
        //if no cinematic, just play the task audio
        //scene is no longer loading
        loading = false;
        //change attachment to loaded
        if (!newGame) ChangeAttachment(currentAttachment);

        //unlocking character
        if (puppetController != null){
        puppetController.IsPlayerControllable = true;
        }

        //(delay AI audio)
        if (TaskManager.instance.currTask != null && TaskManager.instance.currTask.audioOnStart){
            StartCoroutine(DelayedAudioEvent(TaskManager.instance.currTask.audioOnStartEvent, 1));
        }
    
    }

    //gets a fresh location from the puppetcontroller
    public void UpdatePlayerPosition(){
        if (puppetController == null) return;
        playerPosition = puppetController.CurrentPosition;
    }



/************************ CINEMATICS ***********************/


    public void StartOpeningCinematic(){
        Assert.IsNotNull(openingCinematicDirector);
        Assert.IsFalse(disableCinematics);

        openingCinematicDirector.Play();
        //stop character AGAIN
        if (puppetController != null){
            printMsg("Disabling character for opening cinematic...");
            puppetController.IsPlayerControllable = false;
        }
        
        
        //subscribe to end of play
        openingCinematicDirector.stopped += StopOpeningCinematic;
        //hide overlay
        UIManager.instance.UIOverlayPanel.SetActive(false);
    }

    public void StopOpeningCinematic(PlayableDirector aDirector){
        //play first task audio after 1s delay
        if (TaskManager.instance.currTask != null && TaskManager.instance.currTask.audioOnStart){
            StartCoroutine(DelayedAudioEvent(TaskManager.instance.currTask.audioOnStartEvent, 1));
        }

        //unlock character
        if (puppetController != null) {
            printMsg("Enabling character after opening cinematic...");
            puppetController.IsPlayerControllable = true;
        }

        loading = false;
        //show overlay
        UIManager.instance.UIOverlayPanel.SetActive(true);
    }



/***************** HELPER FUNCTIONS BELOW ******************/

    public void AudioEvent(string eventName){
        AudioManager.instance.AddEvent(eventName);
    }

    public IEnumerator DelayedAudioEvent(string eventName, float timeDelay){
        yield return new WaitForSeconds(timeDelay);
        AudioManager.instance.AddEvent(eventName);
    }

    public void RTPCEvent(string eventName, float value){
        AudioManager.instance.SendRTPCEvent(eventName, value);
    }

/******************* DEBUG FUNCTIONS BELOW ******************/

    public void printMsg(string msg){
        if (printDebugs) Debug.Log(msg);
    }
}
