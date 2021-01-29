using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectOptionPlane : MonoBehaviour
{
    public GameObject NormalSprite;
    public GameObject SelectedSprite;
    public void onSelected()
    {
        if (NormalSprite != null && SelectedSprite != null)
        {
            NormalSprite.SetActive(false);
            SelectedSprite.SetActive(true);
        }
    }

    public void onReset()
    {
        if (NormalSprite != null && SelectedSprite != null)
        {
            NormalSprite.SetActive(true);
            SelectedSprite.SetActive(false);
        }
    }
}
