using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

//SETTINGS MENU MUST BE INSTANTIATED BEFORE CONTROLS MENU
public class SettingsMenu : MonoBehaviour
{
    [Header("Language")]
    public Button[] langButtons = new Button[2];
    public GameObject[] langLabels = new GameObject[2];
    private Language currLanguage;

    [Header("Volume")]
    public Slider[] volumeSliders = new Slider[3];
    private float masterVol;
    private float sfxVol;
    private float musicVol;

    [Header("Other buttons")]
    public TMP_Text applyButton;
    public TMP_Text backButton;

    // Start is called before the first frame update
    public void StartSM()
    {
        //get translations and subscribe to future language change events
        TranslationManager.instance.AddToDictionary("Translations/SettingsMenu");
        TranslationManager.instance.languageChangeEvent.AddListener(RefreshText);
        currLanguage = TranslationManager.instance.currLanguage;

        RefreshText();
        LoadSavedData();
        ReloadValues();
    }


/*********************** LANGUAGE LOCALIZATION ****************/

    public void LangButtonPress(){
        //Click!
        AudioManager.instance.AddEvent(AudioEvents.instance.menuButtonPress);
        if (currLanguage == Language.en) currLanguage = Language.dk;
        else if (currLanguage == Language.dk) currLanguage = Language.en;

        UpdateLangButton();
    }

    //makes the lang button UI Match the currlang variable
    public void UpdateLangButton(){
        //Debug.Log($"Currlang {currLanguage.ToString()}");
        for (int i = 0; i < langLabels.Length; i++){
            if ((Language)i == currLanguage){
                langLabels[i].SetActive(true);
            }
            else langLabels[i].SetActive(false);
        }
    }

/**************** VOLUME SLIDERS *************/

    //called when one of the volume sliders have changed their values;
    public void VolumeSliderChange(){
        //update values to reflect slider value
        masterVol = volumeSliders[(int)SettingsLabel.MasterVol].value;
        sfxVol = volumeSliders[(int)SettingsLabel.SFXVol].value;
        musicVol = volumeSliders[(int)SettingsLabel.MusicVol].value;

        UpdateSliders();
    }

    //makes the sliders match current values
    public void UpdateSliders(){
        //update sliders
        volumeSliders[(int)SettingsLabel.MasterVol].value = masterVol;
        volumeSliders[(int)SettingsLabel.SFXVol].value = sfxVol;
        volumeSliders[(int)SettingsLabel.MusicVol].value = musicVol;
    }


/**************** SAVE/LOAD/APPLY SETTINGS ***************/


    public void ApplySettings(){
        //Click!
        AudioManager.instance.AddEvent(AudioEvents.instance.menuButtonPress);
        //check for change in language
        if (currLanguage != TranslationManager.instance.currLanguage){
            TranslationManager.instance.ChangeLanguage(currLanguage);
            PlayerPrefs.SetInt("language", (int)currLanguage);
        }

        //check for changes in each of the volume sections, then apply if change detected
        if (PlayerPrefs.GetFloat("masterVolume") != masterVol){
            PlayerPrefs.SetFloat("masterVolume", masterVol);
            AudioManager.instance.SendRTPCEvent("MasterVolume", masterVol);
        }
        if (PlayerPrefs.GetFloat("sfxVolume") != sfxVol){
            PlayerPrefs.SetFloat("sfxVolume", sfxVol);
            AudioManager.instance.SendRTPCEvent("SFXVolume", sfxVol);
        }
        if (PlayerPrefs.GetFloat("musicVolume") != musicVol){
            PlayerPrefs.SetFloat("musicVolume", musicVol);
            AudioManager.instance.SendRTPCEvent("MusicVolume", musicVol);
        }

        PlayerPrefs.Save();
    }

    //update variables to reflect save data
    public void LoadSavedData(){
        currLanguage = (Language)PlayerPrefs.GetInt("language");
        masterVol = PlayerPrefs.GetFloat("masterVolume");
        sfxVol = PlayerPrefs.GetFloat("sfxVolume");
        musicVol = PlayerPrefs.GetFloat("musicVolume");
    }

    //Makes the UI match what is saved here in the code
    public void ReloadValues(){
        UpdateLangButton();
        UpdateSliders();
    }

    //refreshes all text on screen to account for language change
    public void RefreshText(){
        //temp vars
        string newText = null;
        string key;
        MenuButtonManager mbm;

        /*** LANGUAGE ***/
        mbm = langButtons[0].GetComponent<LanguageToggleButton>().label;
        newText = TranslationManager.instance.Get(SettingsLabel.Language.ToString());
        if (newText != null || mbm != null) {
            //update button text
            mbm.updateText(newText);
        }

        /***** VOLUME LABELS *****/
        for (int i = 0; i < volumeSliders.Length; i++){
            key = ((SettingsLabel)i).ToString();
            newText = TranslationManager.instance.Get(key);
            mbm = volumeSliders[i].GetComponent<VolumeSlider>().label;

            //make sure we are able to change the button name!
            if (newText == null || mbm == null) continue;

            //update button text
            mbm.updateText(newText);
        }

        /*** BACK AND APPLY ***/
        newText = TranslationManager.instance.Get(SettingsLabel.Apply.ToString());
        if (newText != null) applyButton.text = newText;
        newText = TranslationManager.instance.Get(SettingsLabel.Back.ToString());
        if (newText != null) backButton.text = newText;
    }

    public void OnEnable(){
        LoadSavedData();
        ReloadValues();
        ChangeSelection(langButtons[1].gameObject);
        LanguageToggleButton lbutton = langButtons[1].gameObject.GetComponent<LanguageToggleButton>();
        if (lbutton == null) return;
        if (lbutton.label.isSelected == false){
            lbutton.label.OnSelect(null);
        }
    }

    public void OnDisable(){
        //Click!
        AudioManager.instance.AddEvent(AudioEvents.instance.menuButtonPress);
        ReloadValues();
    }

/*********** HELPER FUNCS ***********/

    public void ChangeSelection(GameObject objectToSelect){
        //volumeSliders[(int)label].gameObject
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToSelect);        
    }

}


public enum SettingsLabel {
    MasterVol = 0,
    SFXVol = 1,
    MusicVol = 2,
    Language = 3,
    Apply = 4,
    Back = 5
}