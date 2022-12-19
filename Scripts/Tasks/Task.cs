using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[System.Serializable]
public class Task
{
    public TaskType typeOfTask;
    public string taskInstructions = "";
    public string DkTaskInstructions = "";

    //timer behavior
    public bool timed = false;
    public float timeToComplete = -1;
    public float timer = -1;

    //audio
    public bool audioOnStart = false;
    public string audioOnStartEvent = "";

    //onaction
    public List<GameObject> onActionAfterCompletion = new List<GameObject>();

    //visual scripting stuff
    public GameObject destination;

    public Task(){

    }
    
    public Task(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination)
    {
        //set task type
        this.typeOfTask = TaskType.None;

        //generic task vars
        this.taskInstructions = taskInstructions;
        this.DkTaskInstructions = DkTaskInstructions;
        this.timed = timed;
        this.timeToComplete = timeToComplete;
        this.audioOnStart = audioOnStart;
        this.audioOnStartEvent = audioOnStartEvent;
        this.onActionAfterCompletion = onActionAfterCompletion;
        this.destination = destination;

        //make sure timer starts at 0
        this.timer = 0f;
    }

    /**
     * called by the game manager on completion of previous task
     * activates audio on start if necessary
     */
    public virtual void StartTask()
    {
        timer = 0f;
    }

    /**
     * called by the game manager's update function every frame
     * while the task is active
     */
    public virtual void UpdateTask()
    {
        if (typeOfTask != TaskType.None) return; //only the none task can use a timer
        if (timer < 0) return; //make sure we dont update the task before start
        if (GameManager.instance.loading) return;//dont update timer while loading
        timer += Time.deltaTime;
        if (timer >= timeToComplete) CompleteTask();
    }

    /** 
     * called on completion of the task within the game manager
     */
    public virtual void CompleteTask()
    {
        TaskManager.instance.CompleteTask();
    }

    /** 
     * returns if the parameters are correctly populated
     */
    public virtual bool IsValid()
    {
        
        if (timed && timeToComplete <= 0) return false;
        if (audioOnStart && string.IsNullOrWhiteSpace(audioOnStartEvent)){
            TaskManager.instance.printTaskError(this, @"A task with 'audioOnStart' checked did not have 
            an audioStartEvent");
            return false;
        }

        //A NONE TYPE TASK MUST HAVE A TIMER
        if (typeOfTask == TaskType.None && !timed) {
            TaskManager.instance.printTaskError(this, @"A None type task did not have a timer.");
            return false;
        }

        //onactionobjects must be of type onactionobject
        foreach (GameObject onActionObj in onActionAfterCompletion){
            if (onActionObj.GetComponent<OnActionObject>() == null) {
                TaskManager.instance.printTaskError(this, @"An OnActionObject after completion 
                game object did not have an valid OnActionObject script attached to it.");
                return false;
            }
        }

        //return true if none of the checks failed
        return true;
    }

/***** DEBUG FUNCTIONS BELOW *******/
    public virtual void DebugCompleteTask(){
        CompleteTask();
    }
}


