using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTask : Task
{
    public bool complete;

    void Update() {
        if (complete) {
            TaskCompleted();
            complete = false;
        }
    }

    public override void TaskSetup()
    {
        print("Sample task setup!");
    }

}
