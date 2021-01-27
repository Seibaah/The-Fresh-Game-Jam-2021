using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// the task of the plane (landing task)
public class PlaneTask : Task
{
    public float gasTime { get; set; }  // the time before the plane is out of gas
    
    private Plane thisPlane;
    private bool isComplete;

    // Start is called before the first frame update
    void Start()
    {
        thisPlane = this.GetComponent<Plane>();
    }

    // Update is called once per frame
    void Update()
    {
        // when the plane is in parking state, the task is complete (!isComplete for avoiding repeating calls)
        if (thisPlane.currentState == PlaneState.LANDING && !isComplete)
        {
            isComplete = true;
            TaskCompleted();
        }
    }

    /// <summary>
    /// Override the abstract method in task class
    /// </summary>
    public override void TaskSetup()
    {
        isComplete = false;
    }
}
