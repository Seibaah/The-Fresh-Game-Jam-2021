using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ParkingSpotManager : MonoBehaviour
{
    public TMP_Text noParkingSpotText;
    public Button takeOffOnRunway1;  // button for taking-off on the runway1
    public Button takeOffOnRunway2;  // button for taking-off on the runway2

    public GameObject runway1StartPoint;  // the runway1 start point
    public GameObject runway2StartPoint;  // the runway2 start point
    public GameObject runway1EndPoint;  // the runway1 end point
    public GameObject runway2EndPoint;  // the runway2 end point
    public float desiredTakingOffHeight = 100f;  // the desired height after the plane is taking off

    private enum RunwayNumber
    {
        ONE = 0,
        TWO = 1
    }

    private List<ParkingSpot> parkingSpots = new List<ParkingSpot>();  // List storing the parking spots' scripts instances
    private bool isMessageShown;

    // Use this for initialization
    void Start()
    {
        // initialization
        noParkingSpotText.gameObject.SetActive(false);  // make the message inactive
        isMessageShown = false;
        // initialize taking off button
        takeOffOnRunway1.onClick.AddListener(() => takeOffPlane(RunwayNumber.ONE));
        takeOffOnRunway2.onClick.AddListener(() => takeOffPlane(RunwayNumber.TWO));


        // Add the ParkingSpot script attached to every parking spot gameobject to a list
        foreach (ParkingSpot ps in this.gameObject.GetComponentsInChildren<ParkingSpot>())
        {
            parkingSpots.Add(ps);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Function for finding an available parking spot
    /// </summary>
    /// <returns>return Vector3.zero when no available parking spot, otherwise return the corresponding parking spot position</returns>
    public Vector3 findParkingSpot()
    {
        // iterate through of parking spot list to find a available parking spot
        foreach(ParkingSpot ps in this.parkingSpots)
        {
            // if the parking spot is not occupied
            if(!ps.isOccupied)
            {
                // return the position of this parking spot
                return ps.getCenterPosition();
            }
        }

        // Show the no parking spot message on the screen for 3 seconds
        if(!isMessageShown) StartCoroutine(ShowMessage(this.noParkingSpotText, 3f));

        // return Vector Zero when no available parking spot
        return Vector3.zero;
    }

    /// <summary>
    /// Coroutine for showing a message 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator ShowMessage(TMP_Text message, float delay)
    {
        // show the message for x seconds, x is the delay
        message.gameObject.SetActive(true);
        isMessageShown = true;
        yield return new WaitForSeconds(delay);

        message.gameObject.SetActive(false);
        isMessageShown = false;
    }


    /// <summary>
    /// Make a parking plane taking off if there is one in the parking area
    /// </summary>
    private void takeOffPlane(RunwayNumber runwayNum)
    {
        // if the button for runway1 is clicked 
        if(runwayNum == RunwayNumber.ONE)
        {
            // iterate all parking spots to take off a plane if there is one
            foreach (ParkingSpot ps in this.gameObject.GetComponentsInChildren<ParkingSpot>())
            {
                if(ps.parkedPlane != null)
                {
                    ps.parkedPlane.takeOff(desiredTakingOffHeight, runway1StartPoint.transform.position, runway1EndPoint.transform.position);
                    break;
                }
            }
        }
        else if(runwayNum == RunwayNumber.TWO)
        {
            foreach (ParkingSpot ps in this.gameObject.GetComponentsInChildren<ParkingSpot>())
            {
                if (ps.parkedPlane != null)
                {
                    ps.parkedPlane.takeOff(desiredTakingOffHeight, runway2StartPoint.transform.position, runway2EndPoint.transform.position);
                    break;
                }
            }
        }
    }
}
