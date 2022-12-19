using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    private bool characterOnly = true;
    private string eventName = "";
    private bool roomAudio = false;

    public void StartAT(bool co, string en, bool ra){
        characterOnly = co;
        eventName = en;
        roomAudio = ra;
    }

    void OnTriggerEnter(Collider collision)
    {
        //check if collision is player
        if (!characterOnly || collision.gameObject.tag == GameManager.instance.playerTag) {
            PlayTriggerAudio();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //check if collision is player
        if (!characterOnly || collision.gameObject.tag == GameManager.instance.playerTag) {
            PlayTriggerAudio();
        }
    }

    void PlayTriggerAudio(){
        //make sounds!
        GameManager.instance.AudioEvent(eventName);
        //save if room audio
        if (roomAudio) AudioManager.instance.currentRoomEvent = eventName;
    }
}
