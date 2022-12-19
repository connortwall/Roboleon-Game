using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Puppet.ReviewScripts;

public class TriggerTimeline : MonoBehaviour
{
    public PlayableDirector playableDirector;

    [HideInInspector]
    public bool played = false;

    public void Start(){
        //subcribe to stop event
        playableDirector.stopped += OnClipStop;
        played = false;
    }

    private void OnTriggerEnter(Collider collision){
        if (played) return;
        if (GameManager.instance.disableCinematics) return;
        // check for player tag collision with object
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            played = true;
            //stop character
            PuppetController pc = GameManager.instance.puppetController;
            if (pc != null) pc.IsPlayerControllable = false;

            //switch to timeline
            playableDirector.Play();
        }
    }

    public void OnClipStop(PlayableDirector aDirector){
        PuppetController pc = GameManager.instance.puppetController;
        if (pc != null) pc.IsPlayerControllable = true;
    }

}
