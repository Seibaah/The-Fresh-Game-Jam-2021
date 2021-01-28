using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapInteraction : MonoBehaviour {
    public Camera hoveringCamera;
    public Camera projectionCamera;
    public GameObject RawImage;
    public string PlaneTag;
    public string WayPointTag;

    private Vector3 ChosenTargetPos;
    private GameObject ChosenPlane;

    private void Update(){
       if (Input.GetMouseButtonDown(0)){
        Debug.Log("interacting with raw image");
           InteractWithRawImage();
       }
    }

    private void InteractWithRawImage(){
        RaycastResult result;
        if (hoveringCamera != null){
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            if (results.Count > 0){
                if (results[0].gameObject.layer == LayerMask.NameToLayer("WorldUI")){
                    Debug.Log(results[0].gameObject + results[0].worldPosition.ToString());
                    // found raw image
                    result = results[0];
                    Vector3 hitRawImageLocal = RawImage.transform.InverseTransformPoint(result.worldPosition);
                    Debug.Log("RawImageLocal" + hitRawImageLocal.ToString());

                    // now do a ray cast from the projection camera
                    if (RawImage == null){
                        Debug.Log("RawImage GameObject is null, terminating cast");
                    }
                    float offsetX = RawImage.GetComponent<RectTransform>().rect.width / 2;
                    float offsetY = RawImage.GetComponent<RectTransform>().rect.height / 2;
                    Vector3 hitRawImageLocalS = new Vector3(hitRawImageLocal.x + offsetX, hitRawImageLocal.y + offsetY, hitRawImageLocal.z);
                    Ray rayFromProjCam = projectionCamera.ScreenPointToRay(hitRawImageLocalS); 
                    RaycastHit hit;
                    if (Physics.Raycast(rayFromProjCam, out hit)){
                        GameObject obj = hit.transform.gameObject;
                        Debug.Log("Object hit" + obj);
                        if (obj.tag == PlaneTag) ChosenPlane = obj;
                        else if (obj.tag == WayPointTag) ChosenTargetPos = obj.transform.position;

                        if (ChosenPlane != null && ChosenTargetPos != Vector3.zero) applyChosenPathToPlane();
                    }
                }
            } else return;
        }

    }

    private void applyChosenPathToPlane()
    {
        Debug.Log("Guiding plane " + ChosenPlane + " towards " + ChosenTargetPos.ToString());

        ChosenPlane = null;
        ChosenTargetPos = Vector3.zero;
    }
}
