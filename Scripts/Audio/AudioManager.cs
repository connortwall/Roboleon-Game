using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("General")]
    public string initWwiseEvent = "";
    public string stopAllAudioEvent = "";

    [SerializeField]
    public List<string> logEvents = new List<string>();

    [Header("Debug")]
    public bool printDebugs = false;

    [HideInInspector]
    public static AudioManager instance;
    [HideInInspector]
    public string currentRoomEvent;
    [HideInInspector]
    private List<string> audioEvents = new List<string>();
    [HideInInspector]
    public bool soundEnabled = false;
    

    // Start is called before the first frame update
    public void StartAM()
    {
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 

        //start audio Events instance
        gameObject.GetComponent<AudioEvents>().StartAudioEvents();

        //init currentRoomEvent in case there is no room saved
        currentRoomEvent = initWwiseEvent;

        //wise stuff
        AkSoundEngine.RegisterGameObj(gameObject);
        //AddEvent(initWwiseEvent);
        //AddEvent(AudioEvents.instance.gameMusic);
    }

    public void UpdateAM()
    {
        //send all audio events
        if (audioEvents.Count > 0){
            //send all events to wise
            foreach (var eventName in audioEvents){
                SendEvent(eventName);
                printMsg("Sent event to Wwise!: " + eventName);
            } 
            //delete sent events from the queue
            audioEvents.Clear();
        }
    }

    public void PlayLog(int logNum){
        //add log to audio queue if valid
        if (logEvents.Count <= logNum) return;
        AddEvent(logEvents[logNum]);

    }

    public void OnDestroyAM() { 
        SendEvent(stopAllAudioEvent);
        //save room audio 
        printMsg("The room audio on exit was " + currentRoomEvent);
    }

/**** HELPER FUNCTIONS ****/

    private void SendEvent(string eventName){
        //do not allow sending an event if this game is loading
        if (!soundEnabled) return;
        AkSoundEngine.PostEvent(eventName, gameObject);
    }

    public void SendRTPCEvent(string eventName, float value){
        if (!soundEnabled) return;
        //do not add event if the event is empty
        if (string.IsNullOrWhiteSpace(eventName) || value < 0 || value > 1) return;

        //if (printDebugs) Debug.Log($"Sent RTPC event to WWise with name {eventName} and value {value}");
        AkSoundEngine.SetRTPCValue(eventName, value);
    }

    public void AddEvent(string eventName){
        //do not add event if the event is empty
        if (string.IsNullOrWhiteSpace(eventName)) return;
        audioEvents.Add(eventName);
    }

    public void printMsg(string msg){
        if (printDebugs) Debug.Log(msg);
    }
}
