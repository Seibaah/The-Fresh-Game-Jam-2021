using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskEvent : MonoBehaviour
{

    public Task task;
    public string message;
    public float timeToComplete;
    public int priority;

    public delegate void TaskEventEndDelegate(TaskEvent taskEvent);
    public TaskEventEndDelegate endDelegate;

    private bool success = false;
    private bool stop = false;

    void Start() {
        task.taskCompleteDelegate = TaskCompleted;
        task.TaskStart();
    }

    void Update()
    {
        //end task event if time runs out
        if (!stop) {
            timeToComplete -= Time.deltaTime;
            if(timeToComplete < 0)
            {
                stop = true;
                endDelegate(this);
                task.TaskEnd();
            }
        }
    }

    //called by task if completed
    public void TaskCompleted() {
        stop = true;
        success = true;
        endDelegate(this);
        task.TaskEnd();
    }

    public bool IsSuccessful()
    {
        return success;
    }
}
