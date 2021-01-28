using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSpot : MonoBehaviour
{
    public bool isOccupied { get; set; }  // whether this parking spot is currently occupied by a plane

    private Vector3 centerPosition;  // store the center position of this parking spot

    // Start is called before the first frame update
    void Start()
    {
        isOccupied = false;  // initially no plane on the parking spot
        centerPosition = this.transform.GetChild(0).transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Getter function for the center position of this parking spot
    /// </summary>
    /// <returns></returns>
    public Vector3 getCenterPosition()
    {
        return centerPosition;
    }

    /// <summary>
    /// Detect whether a plane is on this parking spot
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Plane")
        {
            isOccupied = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Plane")
        {
            isOccupied = false;
        }
    }
}
