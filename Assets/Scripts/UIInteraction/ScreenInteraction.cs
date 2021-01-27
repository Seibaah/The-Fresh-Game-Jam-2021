using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ScreenInteraction : MonoBehaviour {
    public int MaxInteractableDistance = 5;
    public Camera HoveringCamera;
    public Camera ExitCamera;

    private GraphicRaycaster mRaycaster;
    private PointerEventData mPointerEventData;
    private EventSystem mEventSystem;
    private Button ExitButton;

    [SerializeField] private int screenID;
    [SerializeField] private GameObject myCanvas;

    public void Start() {
        if (HoveringCamera == null) Debug.Log("ScreenInteraction @ " + this.name + " : HoveringCamera not specified");
        if (ExitCamera == null) Debug.Log("ScreenInteraction @ " + this.name + " : ExitCamera not specified");

        mRaycaster = GetComponent<GraphicRaycaster>();
        mEventSystem = GetComponent<EventSystem>();
        ExitButton = this.transform.Find("ExitButton").gameObject.GetComponent<Button>();
        ExitButton.onClick.AddListener(onExitButtonClicked);

        //left screen = 0; right screen = 1
        if (gameObject.name.Equals("RightCanvas")){
            screenID = 1;
        }
        else
        {
            screenID = 0;
        }
    }

    public void OnStartInteraction() {
        // if there is no camera overlooking the UI, then do nothing
        if (HoveringCamera == null) return;

        // otherwise, switch to this camera
        switchToCamera(HoveringCamera);

        //load ufo target minigame
        if (screenID == 1)
        {
            GameObject canvas = Instantiate(myCanvas, gameObject.transform);
            canvas.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            canvas.transform.localPosition = new Vector3(0, 0, 0);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void onExitInteraction() {
        Camera targetCamera = ExitCamera;

        if (targetCamera == null) targetCamera = Camera.main;

        switchToCamera(targetCamera);

        //load ufo target minigame
        if (screenID == 1)
        {
            Destroy(myCanvas);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void switchToCamera(Camera targetCamera) {
        if (targetCamera == null) return;

        targetCamera.enabled = true;
        targetCamera.tag = "MainCamera";

        foreach (Camera camera in Camera.allCameras) {
            if (camera != targetCamera) {
                camera.enabled = false;
                camera.tag = "Untagged";
            }
        }
    }

    public void onExitButtonClicked() {
        onExitInteraction();
    }
}
