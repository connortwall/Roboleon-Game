using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    //public TaskSpawner tasktester;
    [Header("Tasks")]
    public bool checkIfTasksValid = true;
    public List<TaskSpawner> Act1TaskSequence = new List<TaskSpawner>();
    public List<TaskSpawner> AlternateTaskSequence = new List<TaskSpawner>();

    [HideInInspector] 
    public static TaskManager instance;
    [HideInInspector] 
    public List<Task> taskList;
    [HideInInspector] 
    public Task currTask;
    [HideInInspector] 
    public bool debugMode = true;
    [HideInInspector]
    public int numTasksCompleted;
    [HideInInspector]
    public GameObject taskDestination;


    public void StartTM()
    {
        // singleton stuff
        if (instance != null && instance != this) Destroy(this);  
        else instance = this; 
        //create task list and select first task
        PopulateTasks(Act1TaskSequence);
        DebugPrintTasks();
        if (taskList.Count != 0){
            currTask = taskList[0];
            //start the first task 
            StartTask();
        }

        numTasksCompleted = 0;
    }

    public void UpdateTM(){
        //run the update function of the current task 
        if (currTask != null) currTask.UpdateTask();
    }


/********************* SAVE & LOAD *********************/
    public void LoadTaskSave(int index){
        //must have at least one task left in game in order to load
        if (index >= taskList.Count){
            printMsg("ERROR LOADING TASKS FROM SAVE (INDEX OUT OF BOUNDS)");
            return;
        }

        for (int i = 0; i < index; i++){
            if (currTask != null){
                currTask.DebugCompleteTask();
            }
            else {
                CompleteTask();
            }
        }
    }


/******************** NORMAL TASK EXECUTION **********************/

    /**
     * removes previous current task from task list
     * updates the current task
     * then updates the UI
     */
    public void CompleteTask(){
        //make sure bad things cant happen
        if (taskList == null || taskList.Count <= 0) return;
        
        if(currTask != null && currTask.onActionAfterCompletion != null){
            //check for onActionObjects
            foreach(GameObject onActionObj in currTask.onActionAfterCompletion){
                if (onActionObj == null) continue;
                onActionObj.GetComponent<OnActionObject>().OnAction();
            }
        }

        if (currTask != null){
            printMsg($"{currTask.typeOfTask.ToString()} task ({GetTaskSnippet(currTask, 20)}...) Complete! Changing to next task...");
        }   
        else {
            printMsg("Invalid task marked as complete. Changing to next task...");
        }
        
        taskList.RemoveAt(0);
        numTasksCompleted++;

        //check for game completion
        if (taskList.Count > 0){
            //handle setting up new task
            currTask = taskList[0];
            StartTask();
        }    
        else {
            GameManager.instance.GameComplete();
        }   
    }

    public void AlternateEnding(){
        //clear the current task system
        taskList.Clear();
        //add the new ending
        PopulateTasks(AlternateTaskSequence);
        if (taskList.Count != 0){
            currTask = taskList[0];
            //start the first task 
            StartTask();
        }
    }


    /**
     * only starts a task if the requirements are met for the task
     * otherwise forces debug commands
     */
    private void StartTask(){
        //set destination for directional arrow visualscripting
        taskDestination = currTask.destination;
        //start the task if valid
        if (currTask.IsValid()){
            //language support
            string instructions;
            if (TranslationManager.instance.currLanguage == Language.dk){
                instructions = currTask.DkTaskInstructions;
            }
            else {
                instructions = currTask.taskInstructions;
            }

            //update UI then play sounds
            UIManager.instance.UpdateTaskPanel(instructions);
            if (currTask.audioOnStart){
                AudioManager.instance.AddEvent(currTask.audioOnStartEvent);
            }
            currTask.StartTask();
        }
        else {
            //tell g&l designers task is not valid
            string errMsg = " (use T to continue)";
            if (debugMode) UIManager.instance.UpdateTaskPanel("INVALID TASK: " + currTask.taskInstructions + errMsg);
            //set task to null so that it is not used
            currTask = null;
        }

        //update player's position at time of task start
        GameManager.instance.UpdatePlayerPosition();
    }


    /**
     * converts task spawner elements
     * utilized by the g&l designers in the inspector
     * into the task list
     */
    public void PopulateTasks(List<TaskSpawner> taskSequence){
        printMsg("Creating task list...");

        //helper var
        Task tempTask;
        
        //populate task list with appropriate derivative class
        foreach (TaskSpawner task in taskSequence){
            switch(task.typeOfTask) {

                case TaskType.ButtonPress:
                    tempTask = new ButtonPress(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.buttonToPress);
                    break;

                case TaskType.DoorOpen:
                    tempTask = new DoorOpen(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.doorPrefabToOpen);
                    break;

                case TaskType.Travel:
                    tempTask = new Travel(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.destinationCollider);
                    break;
                
                case TaskType.Firefighter:
                    tempTask = new Firefighter(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.lockDoors,
                        task.doorsToLock,
                        task.fireSpawners);
                    break;
                
                case TaskType.Attachment:
                    tempTask = new Attachment(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.toolToGrab);
                    break;

                case TaskType.Screwdriver:
                    tempTask = new Screwdriver(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.screwObject);
                    break;

                case TaskType.Scanner:
                    tempTask = new Scanner(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination,
                        task.scanner);
                    break;

                default:
                        tempTask = new Task(
                        task.taskInstructions,
                        task.DkTaskInstructions,
                        task.timed,
                        task.timeToComplete,
                        task.audioOnStart,
                        task.audioOnStartEvent,
                        task.onActionAfterCompletion,
                        task.destination);
                    break;
                }//end switch

            //check on newly created task and add to task list
            if (!checkIfTasksValid || tempTask.IsValid()){
                taskList.Add(tempTask);
            }
            else {
                //get info about bad task, and printMsg
                string taskError = "INVALID TASK! INFORMATION:"; 
                string taskName = tempTask.typeOfTask.ToString();
                taskError += string.Format("\nTask type: {0}", taskName);
                int instructLength = 20;
                if (instructLength > tempTask.taskInstructions.Length){
                    instructLength = tempTask.taskInstructions.Length;
                }
                taskError += string.Format("\nTask instructions: {0}...", tempTask.taskInstructions.Substring(0,instructLength));
                if (GameManager.instance.printDebugs || checkIfTasksValid) printMsg(taskError);
            }
        }//end foreach

    printMsg("Done creating task list!");
    }//end PopulateTasks()




/******************* DEBUG FUNCTIONS BELOW ******************/

    /**
     * prints the remaining task list to the console
     */
    public void DebugPrintTasks(){
        string taskPrint = "\n****** CURRENT TASK LIST ******";
        int index = 0;

        foreach (Task task in taskList){
            taskPrint += string.Format("\n Element {0}: {1}", index, task.typeOfTask.ToString());
            index++;
        }

        taskPrint += "\n";
        printMsg(taskPrint);
    }

    private void printMsg(string msg){
        if (GameManager.instance.printDebugs) Debug.Log(msg);
    }

    public string GetTaskSnippet(Task task, int instructLength){
        if (instructLength > task.taskInstructions.Length){
            instructLength = task.taskInstructions.Length;
        }
        string taskSnippet = task.taskInstructions.Substring(0,instructLength);
        return taskSnippet;
    }

    public void printTaskError(Task task, string errTxt){
        if (!debugMode) return;
        string errMsg = $"Invalid {task.typeOfTask.ToString()} Task: \"{GetTaskSnippet(task, 30)}...\" ({errTxt})";
        Debug.LogError(errMsg);
    }
   
}
