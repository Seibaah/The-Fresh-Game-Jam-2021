using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
    public float spawnPlanesTimeInterval;
    public float gas_limit_time_min = 25f;  // the min of gas limit time
    public float gas_limit_time_max = 40f;  // the max of gas limit time

    [Header("Game over conditions")]
    public int maxPlaneCrashes = 3;  // the limit of plane crashes allowed (if > this limit, game over)

    private float timeElapsed = 0f;
    private float planeSpawningTimeElapsed = 0f;

    private int planeCrashCounter = 0;  // the counter for counting the plane crashes

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

        // check for game over state
        if(planeCrashCounter > maxPlaneCrashes)
        {
            // TODO: trigger game over
            //Debug.Log("Game over");

        }
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
        print(taskEvent.name + " was successful: " + taskEvent.IsSuccessful());
        taskEventList.Remove(taskEvent);
        Destroy(taskEvent.gameObject);
        UpdateTaskListText();
    }


    //find first available task, return null if there are none
    public Task FindAvailableTask() {
        foreach (Task task in allTasks) {
            if (!task.IsActive()) {
                return task;
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
        planeCrashCounter++;
    }

}
