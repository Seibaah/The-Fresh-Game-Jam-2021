using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScheduler : MonoBehaviour
{
    public TaskEvent taskEventPrefab;
    public float timeInterval;
    public float minTimeToComplete;
    public float maxTimeToComplete;
    public List<Task> allTasks; //select tasks from this list
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
                taskEventList.Add(InstantiateTaskEvent());
             }
             timeElapsed = 0;
         }
    }

    private TaskEvent InstantiateTaskEvent() {
        TaskEvent newTaskEvent = Instantiate(taskEventPrefab, this.transform);

        newTaskEvent.endDelegate = TaskEventFinished;
        newTaskEvent.task = allTasks[Random.Range(0, allTasks.Count)];
        newTaskEvent.message = "The task '" + newTaskEvent.task.taskName + "' needs to be done.";
        newTaskEvent.timeToComplete = Random.Range(minTimeToComplete, maxTimeToComplete);
        newTaskEvent.priority = 1;
        print(newTaskEvent.message);

        return newTaskEvent;
    }

    //takes a TaskEvent and removes it from the list, noting whether or not it was succesful
    public void TaskEventFinished(TaskEvent taskEvent) {
        print(taskEvent.name + " was successful: " + taskEvent.IsSuccessful());
        taskEventList.Remove(taskEvent);
        Destroy(taskEvent.gameObject);
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

}
