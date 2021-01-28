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
    public int PathLength = 3;

    private List<Vector3> ChosenPath;
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
                foreach (RaycastResult possibleResult in results)
                {
                    if (possibleResult.gameObject.name == "Mini Map Raw Image")
                    {
                        result = possibleResult;
                        Debug.Log(result.gameObject + result.worldPosition.ToString());
                        // found raw image
                        Vector3 hitRawImageLocal = RawImage.transform.InverseTransformPoint(result.worldPosition);
                        Debug.Log("RawImageLocal" + hitRawImageLocal.ToString());

                        // now do a ray cast from the projection camera
                        if (RawImage == null)
                        {
                            Debug.Log("RawImage GameObject is null, terminating cast");
                        }
                        float offsetX = RawImage.GetComponent<RectTransform>().rect.width / 2;
                        float offsetY = RawImage.GetComponent<RectTransform>().rect.height / 2;
                        Vector3 hitRawImageLocalS = new Vector3(hitRawImageLocal.x + offsetX, hitRawImageLocal.y + offsetY, hitRawImageLocal.z);
                        Ray rayFromProjCam = projectionCamera.ScreenPointToRay(hitRawImageLocalS);
                        RaycastHit hit;
                        if (Physics.Raycast(rayFromProjCam, out hit))
                        {
                            GameObject obj = hit.transform.gameObject;
                            Debug.Log("Object hit" + obj);
                            if (ChosenPath == null) ChosenPath = new List<Vector3>();
                            if (obj.tag == PlaneTag) ChosenPlane = obj;
                            else if (obj.tag == WayPointTag) ChosenPath.Add(obj.transform.position);

                            if (ChosenPlane != null && ChosenPath.Count >= PathLength) applyChosenPathToPlane();
                        }

                        break;
                    }
                    
                }
            } else return;
        }

    }

    private void applyChosenPathToPlane()
    {
        Debug.Log("Guiding plane " + ChosenPlane);

        Plane ChosenPlaneComp = ChosenPlane.GetComponent<Plane>();
        if (ChosenPlaneComp != null) ChosenPlaneComp.followPath(new List<Vector3>(ChosenPath), false, new Vector3(0, 0, 0));

        ChosenPlane = null;
        ChosenPath.Clear();
    }
}
