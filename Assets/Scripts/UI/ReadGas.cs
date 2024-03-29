using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadGas : MonoBehaviour
{
    public Plane mPlane;
    public PropellerMotor mPropellerMotor;
    public Slider mGasSlider;
    public GameObject GasCanvas;

    // Update is called once per frame
    void Update()
    {
        mGasSlider.value = 1 - (mPlane.gasTimer / mPlane.gas_limit_time);
        mPropellerMotor.PropellerSpeedCoefficient = mGasSlider.value * 0.7f;

        //mPlane.currentState = PlaneState.PARKING;
        if (mPlane.currentState == PlaneState.PARKING)
        {
            GasCanvas.SetActive(false);
        } else
        {
            GasCanvas.SetActive(true);
        }

    }
}
