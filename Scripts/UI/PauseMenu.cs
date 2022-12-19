using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Assertions;

public class PauseMenu : MonoBehaviour
{
    public GameObject[] menuButtons = new GameObject[6];
    
    [Header("Submenus")]
    public SettingsMenu settingsMenu;
    public ControlsMenu controlsMenu;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    [HideInInspector]
    public bool saveGameEnabled;

    // Start is called before the first frame update
    public void StartPM()
    {
        //add translations for pause menu
        TranslationManager.instance.AddToDictionary("Translations/PauseMenu");
        TranslationManager.instance.languageChangeEvent.AddListener(RefreshText);
        RefreshText();

        /**** SUBMENUS ****/
        settingsMenu.StartSM();
        settingsMenu.gameObject.SetActive(false);
        controlsMenu.StartCM();
        controlsMenu.gameObject.SetActive(false);

        //start buttons
        for (int i = 0; i < menuButtons.Length; i++){
            MenuButtonManager mbm = menuButtons[i].GetComponent<MenuButtonManager>();
            mbm.StartButton();
            if (i != (int)PauseButton.Resume){
               mbm.initButton = true;
            }
        }
        
        saveGameEnabled = true;
        
    }

    public void RefreshText(){
        //temp vars
        string newText = null;
        string key;
        MenuButtonManager mbm;
        //cycle through buttons to change their text
        for (int i = 0; i < menuButtons.Length; i++){
            key = ((PauseButton)i).ToString();

            //special text for save button
            if ((PauseButton)i == PauseButton.Save){
                if (!saveGameEnabled) key = "GameSaved";
            }

            newText = TranslationManager.instance.Get(key);
            mbm = menuButtons[i].GetComponent<MenuButtonManager>();

            //make sure we are able to change the button name!
            if (newText == null || mbm == null) continue;

            //update button text
            mbm.updateText(newText);
        }
    }

/***** BUTTON ACTIONS *******/

    
    public void SaveGame(){
        SaveGameButtonEnabled(false);
        SaveableManager.instance.SaveGame();
    }

    public void SaveGameButtonEnabled(bool saveEnabled){
        saveGameEnabled = saveEnabled;
        Button saveButton = menuButtons[(int)PauseButton.Save].GetComponent<Button>();
        Button resumeButton = menuButtons[(int)PauseButton.Resume].GetComponent<Button>();
        Button controlsButton = menuButtons[(int)PauseButton.Controls].GetComponent<Button>();
        //Disable the save button if the game has already been saved
        if (!saveEnabled){
            if (saveButton != null) saveButton.interactable = false;

            //change navigation around save game button
            if (resumeButton != null && controlsButton != null){
                //remap resume button
                Navigation navigation = resumeButton.navigation;
                navigation.selectOnDown = controlsButton;
                resumeButton.navigation = navigation;
                
                //remap controls button
                navigation = controlsButton.navigation;
                navigation.selectOnUp = resumeButton;
                controlsButton.navigation = navigation;

                //change selection to resume button
                ChangeSelection(PauseButton.Resume);
            }
        }
        else {
            if (saveButton != null) saveButton.interactable = true;

            //change navigation to include save game button
            if (resumeButton != null && controlsButton != null){
                //remap resume button
                Navigation navigation = resumeButton.navigation;
                navigation.selectOnDown = saveButton;
                resumeButton.navigation = navigation;
                
                //remap controls button
                navigation = controlsButton.navigation;
                navigation.selectOnUp = saveButton;
                controlsButton.navigation = navigation;
            }
        }

        RefreshText();
    }

    public void Controls(){
        if (!controlsMenu.gameObject.activeSelf){
            controlsMenu.gameObject.SetActive(true);
        } 
        else {
            controlsMenu.gameObject.SetActive(false);
            ChangeSelection(PauseButton.Controls);
        }
    }

    public void Settings(){
        if (!settingsMenu.gameObject.activeSelf){
            settingsMenu.gameObject.SetActive(true);
        } 
        else {
            settingsMenu.gameObject.SetActive(false);
            ChangeSelection(PauseButton.Settings);
        }
    }

    public void ExitMM(){
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }

    public void ExitDesktop(){
        print("Exiting game...");
        //save player preferences before quitting
        PlayerPrefs.Save();
        Application.Quit();
    }


    public void ChangeSelection(PauseButton button){
        //Debug.Log($"Changing Selection to {button.ToString()}");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuButtons[(int)button]);
        MenuButtonManager mbm = menuButtons[(int)button].GetComponent<MenuButtonManager>();
        if (!mbm.isSelected) mbm.OnSelect(null);
    }

    public void OnEnable(){
        MenuButtonManager mbm = menuButtons[(int)PauseButton.Resume].GetComponent<MenuButtonManager>();
        Assert.IsNotNull(mbm);
        mbm.initButton = false;
        ChangeSelection(PauseButton.Resume);

    }

    public void OnDisable(){
        //make sure subpages are closed
        settingsMenu.gameObject.SetActive(false);
        controlsMenu.gameObject.SetActive(false);

        //re-enabled save
        SaveGameButtonEnabled(true);
        //Click!
        AudioManager.instance.AddEvent(AudioEvents.instance.menuButtonPress);
        EventSystem.current.SetSelectedGameObject(null);
    }
}

public enum PauseButton {
    Save = 0,
    Controls = 1,
    Settings = 2,
    ExitMM = 3,
    ExitD = 4,
    Resume = 5
}
