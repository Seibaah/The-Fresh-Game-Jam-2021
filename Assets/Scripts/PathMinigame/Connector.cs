using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public int correctState;

    public delegate void NotifyCorrectState();
    public NotifyCorrectState notifyCorrectStateDelegate;

    private int currentState;
    private RectTransform rectTransform;
    

    void Awake() {
        rectTransform = this.GetComponent<RectTransform>();
    }


    public void Rotate() {
        currentState++;
        if (currentState > 3) {currentState = 0;}

        rectTransform.Rotate(new Vector3(0, 0, -90));
        
        if (IsComplete()) { notifyCorrectStateDelegate(); }
    }

    public void Reset() {
        rectTransform.localRotation = Quaternion.Euler(0,0,0);
        currentState = 0;
    }

    public bool IsComplete() {
        return correctState == currentState;
    }
}
