using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOption : MonoBehaviour
{
    public bool isLandingPoint = false;

    private Color NormalColor;
    public Color SelectedColor;
    private Material ObjectMaterial;

    public void Start()
    {
        ObjectMaterial = GetComponent<Renderer>().material;
    }

    public virtual void onSelected()
    {
        if (ObjectMaterial == null)
        {
            Debug.Log("WayPointMaterial null, terminating");
        }

        NormalColor = ObjectMaterial.color;

        ObjectMaterial.color = SelectedColor;
    }

    public virtual void onReset()
    {
        if (ObjectMaterial == null)
        {
            Debug.Log("WayPointMaterial null, terminating");
        }

        ObjectMaterial.color = NormalColor;
    }
}
