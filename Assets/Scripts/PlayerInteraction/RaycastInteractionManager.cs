using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RaycastInteractionManager : MonoBehaviour{
    public int MaximumInteractingDistance = 10;

    private void Update() {
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked) {
            AttemptInteractWithScreen();
        }
    }

    /// <summary>
    /// Do a RayCast and attempt to interact with a screen if hit.
    /// Interaction is initialized by calling ScreenInteraction.OnStartInteraction()
    /// </summary>
    /// <returns>true if successfully initiated </returns>
    private bool AttemptInteractWithScreen() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        // Debug.Log("RayCast Hit" + hit.transform.gameObject);

        // attempt to interact with the screen
        ScreenInteraction screen;
        if (hit.transform.parent != null) screen = hit.transform.parent.GetComponent<ScreenInteraction>();
        else return false;

        if (screen != null && hit.distance <= MaximumInteractingDistance) {
            screen.OnStartInteraction();
            return true;
        }

        return false;
    }
}