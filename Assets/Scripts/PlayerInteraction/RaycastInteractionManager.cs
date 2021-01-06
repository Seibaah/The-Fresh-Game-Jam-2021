using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastInteractionManager : MonoBehaviour{
    public int MaximumInteractingDistance = 10;

    // Start is called before the first frame update
    private void Start() {

    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            AttemptInteractWithScreen();
        }
    }

    /// <summary>
    /// Do a RayCast and attempt to interact with a screen if hit.
    /// Interaction is initialized by calling ScreenInteraction.OnStartInteraction()
    /// </summary>
    /// <returns>true if successfully initiated </returns>
    private bool AttemptInteractWithScreen() {
        //Debug.Log("Interaction Attempt Triggered");

        PointerEventData cursor = new PointerEventData(EventSystem.current);
        cursor.position = Input.mousePosition;
        List<RaycastResult> objectsHit = new List<RaycastResult>();
        EventSystem.current.RaycastAll(cursor, objectsHit);

        if (objectsHit.Count > 0) {
            //GameObject objectHit = 
            //var objectTransformHit = hit.transform;
            //var hitObject = objectTransformHit.gameObject;

            // see if it is a screen
            //ScreenInteraction screen = hitObject.GetComponent<ScreenInteraction>();
            //if (screen == null) // interaction failed: not a screen
                return false;

            //if (hit.distance <= MaximumInteractingDistance) {
                //screen.OnStartInteraction();
                //return true;
            //}
            
            //return false;

        }

        // interaction failed: no target found
        return false;
    }
}