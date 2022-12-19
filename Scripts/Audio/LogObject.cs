using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogObject : MonoBehaviour
{

    public int idNumber = -1;

    //when player gets within range of log
    void OnTriggerEnter(Collider collision)
    {
        //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            //tell game manager a log has gotten close to player
            GameManager.instance.LogInRange(gameObject);
        }
    }

    //when player leaves range of log
    void OnTriggerExit(Collider collision)
    {
       //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            //tell the game manager there is no log in range
            GameManager.instance.LogInRange(null);
        }
    }
}
