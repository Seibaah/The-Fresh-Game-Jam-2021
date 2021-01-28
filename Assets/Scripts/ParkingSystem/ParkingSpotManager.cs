using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ParkingSpotManager : MonoBehaviour
{
    public TMP_Text noParkingSpotText;

    private List<ParkingSpot> parkingSpots = new List<ParkingSpot>();  // List storing the parking spots' scripts instances
    private bool isMessageShown;

    // Use this for initialization
    void Start()
    {
        // initialization
        noParkingSpotText.gameObject.SetActive(false);  // make the message inactive
        isMessageShown = false;

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
}
