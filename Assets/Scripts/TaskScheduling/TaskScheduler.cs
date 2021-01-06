using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TaskScheduler : MonoBehaviour
{
    public TaskEvent taskEventPrefab;
    public Text taskListText; //List of active tasks will be updated here

    public float timeInterval;
    public float minTimeToComplete;
    public float maxTimeToComplete;
    public List<Task> allTasks; //tasks are selected from this list
    public List<TaskEvent> taskEventList = new List<TaskEvent>();

    private float timeElapsed = 0f;

    void Update()
    {
        //try creating a task event every time interval
        timeElapsed += Time.deltaTime;
         if(timeElapsed > timeInterval)
         {
             //if there are no available tasks, try again at next interval
             Task tempTask = FindAvailableTask();
             if (tempTask != null) {
                taskEventList.Add(InstantiateTaskEvent(tempTask));
                UpdateTaskListText();
             }
             timeElapsed = 0;
         }
    }

    private TaskEvent InstantiateTaskEvent(Task task) {
        TaskEvent newTaskEvent = Instantiate(taskEventPrefab, this.transform);

        newTaskEvent.endDelegate = TaskEventFinished;
        newTaskEvent.task = task;
        newTaskEvent.message = "Complete " + newTaskEvent.task.taskName;
        newTaskEvent.timeToComplete = Random.Range(minTimeToComplete, maxTimeToComplete);
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

}
