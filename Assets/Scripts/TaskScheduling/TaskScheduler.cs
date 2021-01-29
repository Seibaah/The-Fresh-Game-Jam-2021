using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TaskScheduler : MonoBehaviour
{
    [Header("Task variables")]
    public TaskEvent taskEventPrefab;
    public Text taskListText; //List of active tasks will be updated here

    public float timeInterval;
    public float minTimeToComplete;
    public float maxTimeToComplete;
    public List<Task> allTasks; //tasks are selected from this list
    public List<TaskEvent> taskEventList = new List<TaskEvent>();

    [Header("Plane Generation variables")]
    public GameObject planePrefab;
    public GameObject spawningPoint;  // the spawning point of the plane
    public int lives = 10;  // the counter for counting the plane crashes/task failures
    public float spawnPlanesTimeInterval;
    public float gas_limit_time_min = 25f;  // the min of gas limit time
    public float gas_limit_time_max = 40f;  // the max of gas limit time

    [Header("Game over conditions")]
    public GameObject gameOverMenu;
    public int maxPlaneCrashes = 3;  // the limit of plane crashes allowed (if > this limit, game over)
    public TMP_Text numCrashesMessage;

    private float timeElapsed = 0f;
    private float planeSpawningTimeElapsed = 0f;

    private Restart_menu gameOverScript;

    private void Start()
    {
        gameOverScript = gameOverMenu.GetComponent<Restart_menu>();
        numCrashesMessage.SetText(lives + " lives left!");
    }

    void Update()
    {
        //try creating a task event every time interval
        timeElapsed += Time.deltaTime;
        planeSpawningTimeElapsed += Time.deltaTime;

        if (timeElapsed > timeInterval)
        {
            //if there are no available tasks, try again at next interval
            Task tempTask = FindAvailableTask();
            if (tempTask != null) {
            taskEventList.Add(InstantiateTaskEvent(tempTask));
            UpdateTaskListText();
            }
            timeElapsed = 0;
        }

        // Spawn planes every time interval (use a different timer)
        if(planeSpawningTimeElapsed > spawnPlanesTimeInterval)
        {
            // generate a plane at the spawning point
            GameObject plane = Instantiate(planePrefab, spawningPoint.transform.position, spawningPoint.transform.rotation);
            PlaneTask planeTask = plane.GetComponent<PlaneTask>();
            taskEventList.Add(InstantiateTaskEvent(planeTask));  // add this landing task to the task event list
            UpdateTaskListText();
            planeSpawningTimeElapsed = 0;
        }

        /* check for game over state
        if(planeCrashCounter > maxPlaneCrashes)
        {
            // trigger game over
            gameOverScript.gameObject.SetActive(true);
            gameOverScript.GameOver();
        }
        */
    }

    private TaskEvent InstantiateTaskEvent(Task task) {
        TaskEvent newTaskEvent = Instantiate(taskEventPrefab, this.transform);

        newTaskEvent.endDelegate = TaskEventFinished;
        newTaskEvent.task = task;
        newTaskEvent.message = "Complete " + newTaskEvent.task.taskName;

        // if the task is the plane landing task, make the time to complete be the "out of gas time" of the plane
        if (task.taskName == "PlaneLandingTask")
        {
            newTaskEvent.timeToComplete = Random.Range(gas_limit_time_min, gas_limit_time_max);
            ((PlaneTask)task).gasTime = newTaskEvent.timeToComplete;
        }
        else
        {
            newTaskEvent.timeToComplete = Random.Range(minTimeToComplete, maxTimeToComplete);
        }

        newTaskEvent.priority = 1;

        return newTaskEvent;
    }

    //takes a TaskEvent and removes it from the list, noting whether or not it was succesful
    public void TaskEventFinished(TaskEvent taskEvent) {

        if (!taskEvent.IsSuccessful()) {
            removeLife();
        }

        taskEventList.Remove(taskEvent);
        Destroy(taskEvent.gameObject);
        UpdateTaskListText();
    }


    /*find first available task, return null if there are none
    public Task FindFirstAvailableTask() {
        foreach (Task task in allTasks) {
            if (!task.IsActive()) {
                return task;
            }
        }
        return null;
    }*/

    //randomly iterates through all tasks to find available task, returns null if there are none
    public Task FindAvailableTask() {
        int n = allTasks.Count;
        for (int i = 0; i < n; i++) {
            int r = i + Random.Range(0, n - i);
            Task t = allTasks[r];
            allTasks[r] = allTasks[i];
            allTasks[i] = t;

            if (!allTasks[i].IsActive()) {
                return t;
            }
        }
        return null;
    }

    public void UpdateTaskListText() {
        string taskListString = "";
        foreach (TaskEvent taskEvent in taskEventList) {
            taskListString += " - ";
            taskListString += taskEvent.message; 
            taskListString += "\n";
        }
        taskListText.text = taskListString;
    }

    /// <summary>
    /// Function to be called when a plane is destroyed (either colliding with another plane or out of gas)
    /// This function will increament the crash counter
    /// </summary>
    public void planeCrashes()
    {
        removeLife();
    }


    private void removeLife() {
        lives--;
        if(lives <= 0) {
            gameOverScript.gameObject.SetActive(true);
            gameOverScript.GameOver();
            numCrashesMessage.SetText(lives + " lives left!\nGame Over!");
        }
        else
            numCrashesMessage.SetText(lives + " lives left!");
    }

}
