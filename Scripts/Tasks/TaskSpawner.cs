using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskSpawner
{

    [Header("Generic Task Logic")]
    /*** GENERIC TASK VARIABLES ***/
    public TaskType typeOfTask;
    [TextArea]
    public string taskInstructions;
    [TextArea]
    public string DkTaskInstructions;
    //timer behavior
    public bool timed;
    public float timeToComplete;
    //audio
    public bool audioOnStart = false;
    public string audioOnStartEvent = "";
    //on action items
    public List<GameObject> onActionAfterCompletion;
    public GameObject destination;

    /*** BUTTONPRESS ***/
    [Header("\nButton Press Task")]
    public GameObject buttonToPress;

    /*** DOOROPEN ***/
    [Header("\nDoor Open Task")]
    public GameObject doorPrefabToOpen;

    /*** TRAVEL ***/
    [Header("\nTravel Task")]
    public GameObject destinationCollider;

    /*** FIREFIGHTER ***/
    [Header("\nFirefighter Task")]
    public bool lockDoors;
    public List<GameObject> doorsToLock;
    public List<GameObject> fireSpawners;

    /*** ATTACHMENT ***/
    [Header("\nAttachment Task")]
    public GameObject toolToGrab;

    /*** SCREWDRIVER ***/
    [Header("\nScrewdriver Task")]
    public GameObject screwObject;

    /** SCANNER **/
    [Header("\nScanner Task")]
    public GameObject scanner;


}


