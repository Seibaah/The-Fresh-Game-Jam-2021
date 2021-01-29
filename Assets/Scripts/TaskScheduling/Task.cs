using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public string taskName;
    public delegate void TaskCompleteDelegate();
    public TaskCompleteDelegate taskCompleteDelegate;

    public delegate void TaskEndDelegate();
    public TaskEndDelegate taskEndDelegate;

    private bool active; //true when task is used by a task event 

    public abstract void TaskSetup(); //setup task so that it is ready to be played

    //call when task is succesfully completed
    public void TaskCompleted()
    {
        taskCompleteDelegate();
        TaskEnd();
    }

    public void TaskStart() {
        TaskSetup();
        active = true;
    }

    public void TaskEnd() {
        active = false;
        if (taskEndDelegate != null) {
            taskEndDelegate();
        }
    }

    public bool IsActive() {
        return active;
    }
}