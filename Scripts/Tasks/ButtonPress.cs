using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ButtonPress : Task
{
    public GameObject buttonToPress;

    //constructer utilized by TaskSpawner
    public ButtonPress(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        GameObject buttonToPress)
    {
        //set task type
        base.typeOfTask = TaskType.ButtonPress;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //button specific vars
        this.buttonToPress = buttonToPress;
    }

    public override void StartTask(){
        //subscribe to the button press event
        ButtonObject buttonObj = buttonToPress.GetComponent<ButtonObject>();
        Assert.IsNotNull(buttonObj);
        buttonObj.buttonPressEvent.AddListener(CompleteTask);
    }

    public override void UpdateTask(){
    }

    public override void CompleteTask(){
        ButtonObject buttonObj = buttonToPress.GetComponent<ButtonObject>();
        buttonObj.buttonPressEvent.RemoveListener(CompleteTask);
        TaskManager.instance.CompleteTask();
    }

    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;
        //check button 
        if (buttonToPress == null){
            TaskManager.instance.printTaskError(this, @"buttonToPress was not assigned");
            return false;
        }
        if (buttonToPress.GetComponent<ButtonObject>() == null){
            TaskManager.instance.printTaskError(this, @"buttonToPress did not have a ButtonObject 
            script attached to it");
            return false;
        }

        return true;
    }

/************ DEBUG FUNCTIONS BELOW **************/
    public override void DebugCompleteTask(){
        if (!IsValid()){
            CompleteTask();
            return;
        }
        //pretend that you pressed the button
        ButtonObject buttonObj = buttonToPress.GetComponent<ButtonObject>();
        //make sure is unlocked then press
        buttonObj.OnAction();
        buttonObj.OnPress();
    }

}
