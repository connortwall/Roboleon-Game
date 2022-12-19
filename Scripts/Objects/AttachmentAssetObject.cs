using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puppet.ReviewScripts;

public class AttachmentAssetObject : MonoBehaviour
{
    public ArmMode attachmentType = ArmMode.EmptyAttachment;

    //when player gets within range of attachment
    void OnTriggerEnter(Collider collision)
    {
        //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            //tell game manager a log has gotten close to player
            GameManager.instance.AttachmentInRange(gameObject);
        }
    }

    //when player leaves range of log
    void OnTriggerExit(Collider collision)
    {
       //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            //tell the game manager there is no log in range
            GameManager.instance.AttachmentInRange(null);
        }
    }
}
