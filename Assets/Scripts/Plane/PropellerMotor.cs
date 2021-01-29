using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerMotor : MonoBehaviour
{
    public float PropellerSpeedCoefficient = 0.2f;
    public GameObject Propeller;

    // Update is called once per frame
    void Update()
    {
        if (Propeller != null)
        {
            Propeller.transform.Rotate(0f, 90f * PropellerSpeedCoefficient, 0f, Space.Self);
        }
    }
}
