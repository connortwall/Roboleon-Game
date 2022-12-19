using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Events;

public class TranslationManager : MonoBehaviour
{
    [HideInInspector]
    public static TranslationManager instance;
    [HideInInspector]
    public Dictionary<string, Translation> translations;
    [HideInInspector]
    public Language currLanguage = Language.en;
    [HideInInspector]
    public UnityEvent languageChangeEvent = new UnityEvent();

    public void StartTM(string fileName)
    {
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 

        translations = new Dictionary<string, Translation>();
        AddToDictionary(fileName);
    }

    //returns the text associated with the key in the current language
    public string Get(string key){
        if (translations == null || !translations.ContainsKey(key)){
            return null;
        }
        
        switch(currLanguage){
            case Language.en: 
                return translations[key].en;
            case Language.dk:
                return translations[key].dk;
            default:
                return null;
        }
    }

    public void AddToDictionary(string fileName){
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        var jsonTxt = jsonFile.text;

        //read in strings and convert to dictionary
        List<TranslationWrapper> tw = JsonConvert.DeserializeObject<List<TranslationWrapper>>(jsonTxt);
        foreach (TranslationWrapper temp in tw){
            if (translations.ContainsKey(temp.key)) continue;
            translations.Add(temp.key, new Translation(temp.en, temp.dk));
        }
    }

    public void ChangeLanguage(Language lang){
        currLanguage = lang;
        languageChangeEvent.Invoke();
    }


}//end of TranslationManager

//for use in dictionary
public class Translation { 
    public string en;
    public string dk;

    public Translation(string en, string dk) {
        this.en = en;
        this.dk = dk;
    }
}

//for json parsing only (kinda bad implementation but we will deal)
public class TranslationWrapper {
    [JsonProperty("key")]
    public string key { get; set; }
    [JsonProperty("en")]
    public string en { get; set; }
    [JsonProperty("dk")]
    public string dk { get; set; }
}

public enum Language {
    en = 0,
    dk = 1
}