using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveableManager saveableManager;
    public GameObject[] menuButtons = new GameObject[5];
    public string gameScene = "spaceship";

    [Header("Submenus")]
    public SettingsMenu settingsMenu;
    public ControlsMenu controlsMenu;

    [Header("Debug Options")]
    public bool debugMode;
    
    [HideInInspector]
    public static MainMenu instance;
    [HideInInspector]
    public bool loadingDisabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 

        /***** START MANAGERS *****/
        //create translation manager, start it, and sub to lang change event
        TranslationManager tm = gameObject.AddComponent<TranslationManager>();
        tm.StartTM("Translations/MainMenu");
        tm.languageChangeEvent.AddListener(RefreshText);
        //start main menu audio
        audioManager.StartAM();
        AudioManager.instance.soundEnabled = true;
        AudioManager.instance.AddEvent(AudioEvents.instance.mainMenu);
        //start SL 
        saveableManager.StartSM();

        /***** PLAYER PREFS *****/
        CreateUserPrefs();
        //update lang in the translation manager
        tm.currLanguage = (Language)(PlayerPrefs.GetInt("language"));

        /**** SUBMENUS ****/
        settingsMenu.StartSM();
        settingsMenu.gameObject.SetActive(false);
        controlsMenu.StartCM();
        controlsMenu.gameObject.SetActive(false);

        /***** BUTTONS ***/
        //start all buttons, with special setup for startgame button
        for (int i = 0; i < menuButtons.Length; i++){
            MenuButtonManager mbm = menuButtons[i].GetComponent<MenuButtonManager>();
            mbm.StartButton();
            //mbm.updateText("load Game");
            if (i == 0){
                ChangeSelection(MenuButton.Start);
                mbm.startButton = true;
            }
            mbm.initButton = true;
        }

        //Disable the load button if there is no loadable games
        if (!SaveableManager.instance.CanLoadGame()){
            loadingDisabled = true;
            Button loadButton = menuButtons[(int)MenuButton.Load].GetComponent<Button>();
            if (loadButton != null) loadButton.interactable = false;

            //change navigation around load game button
            Button startButton = menuButtons[(int)MenuButton.Start].GetComponent<Button>();
            Button controlsButton = menuButtons[(int)MenuButton.Controls].GetComponent<Button>();
            if (startButton != null && controlsButton != null){
                //remap save button
                Navigation navigation = startButton.navigation;
                navigation.selectOnDown = controlsButton;
                startButton.navigation = navigation;
                
                //remap controls button
                navigation = controlsButton.navigation;
                navigation.selectOnUp = startButton;
                controlsButton.navigation = navigation;

            }
        }


        RefreshText();

    }

    void Update(){
        AudioManager.instance.UpdateAM();
    }

    public void ChangeSelection(MenuButton button){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuButtons[(int)button]);
    }

    //refresh text on all main menu UI elements to account for language change
    public void RefreshText(){
        //temp vars
        string newText = null;
        string key;
        MenuButtonManager mbm;
        //cycle through buttons to change their text
        for (int i = 0; i < menuButtons.Length; i++){
            key = ((MenuButton)i).ToString();
            newText = TranslationManager.instance.Get(key);
            mbm = menuButtons[i].GetComponent<MenuButtonManager>();

            //make sure we are able to change the button name!
            if (newText == null || mbm == null) continue;

            //update button text
            mbm.updateText(newText);
        }
    }

    private void CreateUserPrefs(){
        //settings
        if (!PlayerPrefs.HasKey("language")) PlayerPrefs.SetInt("language", (int)Language.en);
        if (!PlayerPrefs.HasKey("masterVolume")) PlayerPrefs.SetFloat("masterVolume", 0.75f);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 0.75f);
        if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 0.75f);

        //new game behavior
        if (!PlayerPrefs.HasKey("newGame")) PlayerPrefs.SetInt("newGame", 1);
    }


/******************** BUTTON FUNCTIONALITY *****************/

   
    public void StartGame(){
        //TODO: NEW SAVE FUNCTIONALITY
        PlayerPrefs.SetInt("newGame", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameScene);
    }

    public void SettingsMenu(){
        if (!settingsMenu.gameObject.activeSelf){
            settingsMenu.gameObject.SetActive(true);
        } 
        else {
            settingsMenu.gameObject.SetActive(false);
            ChangeSelection(MenuButton.Settings);
        }
    }


    public void ControlsMenu(){
        if (!controlsMenu.gameObject.activeSelf){
            controlsMenu.gameObject.SetActive(true);
        } 
        else {
            controlsMenu.gameObject.SetActive(false);
            ChangeSelection(MenuButton.Controls);
        }
    }

    public void LoadGame(){
        PlayerPrefs.SetInt("newGame", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameScene);
    }

    //CONTROLS AND SETTINGS PAGES HAVE THEIR OWN FILE

    public void ExitGame(){
        print("Exiting game...");
        //save player preferences before quitting
        PlayerPrefs.Save();
        Application.Quit();
    }    
}

public enum MenuButton {
    Start = 0,
    Load = 1,
    Controls = 2,
    Settings = 3,
    Exit = 4
}


