using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Puppet.ReviewScripts;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public PauseMenu pauseMenu;
    public GameObject pauseScreen;
    public GameObject loadingScreen;
    public GameObject UIOverlayPanel;
    
    [Header("Tasks")]
    public TMP_Text taskInstructions;
    public TMP_Text taskLabel;

    [Header("Logs")]
    public GameObject promptBoxLog;
    public TMP_Text logPromptText;
    public TMP_Text logCounter;
    public GameObject logScreen;
    public List<GameObject> logButtons = new List<GameObject>();

    [Header("Attachments")]
    public GameObject promptBoxAttachment;
    public TMP_Text attachmentPromptText;
    public Image attachmentImageBox;
    public Sprite[] attachmentIcons = new Sprite[4];

    [Header("Managers")]
    public TranslationManager translationManager;
    
    [HideInInspector]
    public static UIManager instance;
    [HideInInspector]
    public bool paused;

    public void StartUI() 
    { 
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 

        //set up translation manager
        translationManager.StartTM("Translations/GameUI");
        translationManager.currLanguage = (Language)PlayerPrefs.GetInt("language");
        translationManager.languageChangeEvent.AddListener(RefreshText);
        RefreshText();

        //pull up the loading screen
        if (loadingScreen != null) loadingScreen.SetActive(true);

        //verify the log buttons were setup correctly
        Assert.AreEqual(AudioManager.instance.logEvents.Count, logButtons.Count);

        //deactivate then hide stuff
        foreach (GameObject logButton in logButtons){
            logButton.GetComponent<Button>().interactable = false;
        }

        UpdateLogCounter();

        /**** submenus ****/
        pauseMenu.StartPM();

        HideAllPages();
    }

    public void RefreshText(){
        if (TaskManager.instance == null || TaskManager.instance.currTask == null) return;
        if (TranslationManager.instance.currLanguage == Language.dk){
            UpdateTaskPanel(TaskManager.instance.currTask.DkTaskInstructions);
        }
        else {
            UpdateTaskPanel(TaskManager.instance.currTask.taskInstructions);
        }

        //active process text
        string newText = TranslationManager.instance.Get("taskLabel");
        if (newText != null) taskLabel.text = newText;

        //prompt boxes
        newText = TranslationManager.instance.Get("logPrompt");
        if (newText != null) logPromptText.text = newText;

        //prompt boxes
        newText = TranslationManager.instance.Get("attachmentPrompt");
        if (newText != null) attachmentPromptText.text = newText;

    }


/********************** PAGES ************************/
    public void TogglePauseGame(){
        if (!pauseScreen.gameObject.activeSelf){
            paused = true;
            PuppetController pc = GameManager.instance.puppetController;
            if (pc != null) pc.IsPlayerControllable = false;
            Time.timeScale = 0;
            pauseScreen.gameObject.SetActive(true);
            AudioManager.instance.AddEvent(AudioManager.instance.stopAllAudioEvent);
            AudioManager.instance.AddEvent(AudioEvents.instance.pauseMenu);
        } 
        else {
            if (!logScreen.activeSelf){
                paused = false;
                PuppetController pc = GameManager.instance.puppetController;
                if (pc != null) pc.IsPlayerControllable = true;
                Time.timeScale = 1;   
            }

            pauseScreen.gameObject.SetActive(false);
            AudioManager.instance.AddEvent(AudioManager.instance.stopAllAudioEvent);
            if (logScreen.activeSelf){
                AudioManager.instance.AddEvent(AudioEvents.instance.logScreen);
                int numLogs = GameManager.instance.logKeeper.Length;

                //find a button to select
                for(int logIndex = 0; logIndex < numLogs; logIndex++){
                    if (GameManager.instance.logKeeper[logIndex]) {
                        EventSystem.current.SetSelectedGameObject(logButtons[logIndex]);
                    }
                }
            }
            else {
                AudioManager.instance.AddEvent(AudioManager.instance.initWwiseEvent);
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    //handle back button input from game manager
    public void BackPress(){
        if (!paused) return;
        //unpause if that menu is open
        if (pauseMenu.settingsMenu.gameObject.activeSelf){
            pauseMenu.Settings();
        }
        else if (pauseMenu.controlsMenu.gameObject.activeSelf){
            pauseMenu.Controls();
        }
        else if (pauseScreen.gameObject.activeSelf){
            TogglePauseGame();
        }  
        //if its not in the pause menu, then try the log page
        else if (logScreen.activeSelf){
            ToggleLogScreen();
        }
    }

/*********************** ATTACHMENTS ************************/

    public void ToggleAttachmentPrompt(bool enable){
        if (enable) promptBoxAttachment.SetActive(true);
        else promptBoxAttachment.SetActive(false);
    }

    public void ChangeAttachmentIcon(ArmMode armMode){
        switch(armMode){
            case ArmMode.Magnet:
                attachmentImageBox.sprite = attachmentIcons[1];
                break;
            case ArmMode.FireExtinguisher:
                attachmentImageBox.sprite = attachmentIcons[2];
                break;
            case ArmMode.ScrewDriver:
                attachmentImageBox.sprite = attachmentIcons[3];
                break;
            default:
                attachmentImageBox.sprite = attachmentIcons[0];
                break;
        }
    }

/*********************** LOGS *****************************/

    public void ToggleLogPrompt(bool enable){
        if (enable) promptBoxLog.SetActive(true);
        else promptBoxLog.SetActive(false);
    }

    public void LogButtonPress(){

        //play log button press sound
        AudioManager.instance.AddEvent(AudioEvents.instance.playLogButton);
        //get log number from button
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        buttonName = buttonName.Replace("logPanel", "");
        int logIndex = int.Parse(buttonName);
        //printMsg("Clicked log #" + logIndex);

        //play the log!
        AudioManager.instance.PlayLog(logIndex);
        
    }

    public void ToggleLogScreen(){
        //cant use the log page while we are in the pause menu
        if (pauseScreen.gameObject.activeSelf) return;
        if (!logScreen.activeSelf) {
            logScreen.SetActive(true);
            paused = true;
            PuppetController pc = GameManager.instance.puppetController;
           if (pc != null) pc.IsPlayerControllable = false;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(null);

            int numLogs = GameManager.instance.logKeeper.Length;

            //find a button to select
            for(int logIndex = 0; logIndex < numLogs; logIndex++){
                if (GameManager.instance.logKeeper[logIndex]) {
                    EventSystem.current.SetSelectedGameObject(logButtons[logIndex]);
                }
            }
            
            //stop level audio and play log screen audio
            AudioManager.instance.AddEvent(AudioManager.instance.stopAllAudioEvent);
            AudioManager.instance.AddEvent(AudioEvents.instance.logScreen);
        }
        else{
            EventSystem.current.SetSelectedGameObject(null);
            Time.timeScale = 1;
            logScreen.SetActive(false);
            paused = false;
            PuppetController pc = GameManager.instance.puppetController;
            if (pc != null) pc.IsPlayerControllable = true;

            //stop log level audio and resume game audio
            AudioManager.instance.AddEvent(AudioManager.instance.stopAllAudioEvent);
            AudioManager.instance.AddEvent(AudioManager.instance.initWwiseEvent);
        }
    }

    //this toggle is called only when a log is collected from the scene
    public void CollectLog(int logNum){
        logScreen.SetActive(true);
        paused = true;
        Time.timeScale = 0;
        
        EventSystem.current.SetSelectedGameObject(null);

        //enable and select button
        logButtons[logNum].GetComponent<Button>().interactable = true;
        EventSystem.current.SetSelectedGameObject(logButtons[logNum]);
        UpdateLogCounter();

        //stop level audio and play log screen audio
        AudioManager.instance.AddEvent(AudioManager.instance.stopAllAudioEvent);
        AudioManager.instance.AddEvent(AudioEvents.instance.logScreen);

        //play the log that was collected after short wait
        StartCoroutine(LogCollectionAudio(logNum));
        
    }

    public void UpdateLogCounter(){
        int numCollected = GameManager.instance.logKeeper.Count(log => log == true);
        int numLogs = logButtons.Count;
        if (logCounter == null) return;
        string logCounterText = $"{numCollected}/{numLogs}";
        logCounter.text = logCounterText;
    }

/************************** TASKS *****************************/

    public void UpdateTaskPanel(string instructions){
        if (taskInstructions == null) return;
        taskInstructions.text = instructions;
    }

/*************************** SAVE&LOAD ************************/

    public void LoadUnlockedLogs(){
        bool[] logKeeper = GameManager.instance.logKeeper;
        //deactivate then hide stuff
        for (int i = 0; i < logKeeper.Length; i++){
            if (logKeeper[i]){
                logButtons[i].GetComponent<Button>().interactable = true;
            }
        }
        UpdateLogCounter();
    }


/******************** HELPER FUNCTIONS BELOW ******************/

    public void HideAllPages(){
        logScreen.SetActive(false);
        ToggleLogPrompt(false);
        ToggleAttachmentPrompt(false);
        pauseScreen.SetActive(false);
    }
    
    public void printMsg(string msg){
        if (GameManager.instance.printDebugs) Debug.Log(msg);
    }

    private IEnumerator LogCollectionAudio(int logNum){
        float delay = 0.5f;
        yield return StartCoroutine(WaitForRealSeconds(delay));
        AudioManager.instance.PlayLog(logNum);
        yield return null;
    }

    private IEnumerator WaitForRealSeconds (float seconds) {
        float startTime = UnityEngine.Time.realtimeSinceStartup;
        while (UnityEngine.Time.realtimeSinceStartup-startTime < seconds) {
            yield return null;
        }
        yield return null;
    } 
}
