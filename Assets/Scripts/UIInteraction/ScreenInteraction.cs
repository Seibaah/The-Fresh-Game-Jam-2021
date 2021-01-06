using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenInteraction : MonoBehaviour {
    public Camera OverlookingCamera;

    private GraphicRaycaster mRaycaster;
    private PointerEventData mPointerEventData;
    private EventSystem mEventSystem;

    public void Start() {
        mRaycaster = GetComponent<GraphicRaycaster>();
        mEventSystem = GetComponent<EventSystem>();
    }
    

    public void Update() {
        if (Input.GetKey(KeyCode.Mouse0)) {
            mPointerEventData = new PointerEventData(mEventSystem);
            mPointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            mRaycaster.Raycast(mPointerEventData, results);

            foreach (RaycastResult result in results) {
                if (result.gameObject.transform.IsChildOf(this.transform)) {
                    this.OnStartInteraction();
                    Debug.Log("Canvas Hit");
                }
            }
        }
    }

    public void OnStartInteraction() {
        // if there is no camera overlooking the UI, then do nothing
        if (OverlookingCamera == null) return;

        // otherwise, switch to this camera
        OverlookingCamera.enabled = true;
        
        foreach (Camera camera in Camera.allCameras) {
            if (camera != OverlookingCamera)
                camera.enabled = false;
        }
    }
}
