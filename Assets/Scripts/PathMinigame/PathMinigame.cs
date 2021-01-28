using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathMinigame : Task
{

    public Text gameStatus;
    public Connector[] connectors;
    public GameObject screen;

    void Start()
    {
        foreach (Connector c in connectors) {
            c.notifyCorrectStateDelegate = ConnectorComplete;
        }
    }

    //prepare the minigame to be played
    override public void TaskSetup() {
        screen.SetActive(true);

        gameStatus.color = new Color32(255,52,52,255);
        gameStatus.text = "INCOMPLETE";

        foreach (Connector c in connectors) {
            c.Reset();
        }
    }

    //called when a connector is in its correct state
    //checks to see if all other connectors are also in a correct state
    public void ConnectorComplete() {
        bool gameComplete = true;
        foreach (Connector c in connectors) {
            if (!c.IsComplete()) {
                gameComplete = false;
                break;
            }
        }

        if (gameComplete) {
            gameStatus.color = new Color32(52,255,52,255);
            gameStatus.text = "COMPLETE";
            TaskCompleted(); //alert task scheduler that minigame is completed
            StartCoroutine(TurnOffScreen());
        }

    }

    IEnumerator TurnOffScreen() {
        yield return new WaitForSeconds(2.0f);
        screen.SetActive(false);
    }
}
