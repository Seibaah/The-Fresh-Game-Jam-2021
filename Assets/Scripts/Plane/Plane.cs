using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public float flyingForce = 20f;  // the force apply to this plane when flying
    public float slidingForce = 12f;  // the force applyig to this plane when sliding on the ground
    public float slidingDistance = 30f;  // the distance of sliding


    public PlaneState currentState { get; set; }  // the current state of this plane

    private Rigidbody rb;  // the rigidbody component of this plane

    // Variable for the plane motion:
    private Vector3 prevPosition;  // used for adjusting facing direction

    // Just for testing
    private float desiredHeight = 100f;  // the desired height for take off action
    private float groundLevel = 6f;  // the ground level for landing action



    // Start is called before the first frame update
    void Start()
    {
        // initialize the variable fields
        rb = this.GetComponent<Rigidbody>();
        currentState = PlaneState.FLYING;  // assume we initially generate plane in the sky
    }

    // Update is called once per frame
    void Update()
    {
        // For testing landing + take off + circling
        if(currentState == PlaneState.FLYING)
        {
            landing(groundLevel);
        }
        else if(currentState == PlaneState.STOP_ON_LAND)
        {
            takeOff(desiredHeight);
        }
        else if(currentState == PlaneState.FINISHED_TAKING_OFF)
        {
            circling(20f, this.transform.position + this.transform.forward * 10);
            Destroy(this.gameObject, 21);
        }


    }


    /// <summary>
    /// Funtion for this plane to move toward a given direction with a given force 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    private void moveInDirection(Vector3 targetDirection, float force)
    {
        this.rb.AddForce(targetDirection * force);  // apply force of x-z direction

        /*
        // change the facing direction of this plane based on the current velocity
        Vector3 facingDirection = rb.velocity;
        facingDirection.y = 0;
        transform.forward = rb.velocity;
        */
    }


    /// <summary>
    /// Function for this plane to perform takeOff action (along the current facing direction) until reaching the desired height
    /// </summary>
    /// <param name="desiredHeight">the desired height of this plane will reach after taking-off is done</param>
    public void takeOff(float desiredHeight)
    {
        currentState = PlaneState.TAKING_OFF;
        StartCoroutine(SmoothTakeOff(desiredHeight));  // start the coroutine
    }


    //Co-routine for take off function
    protected IEnumerator SmoothTakeOff(float desiredHeight)
    {
        // take off along the current facing direction
        Vector3 targetDirection = new Vector3(this.transform.forward.x, 0, this.transform.forward.z);

        // sliding for a distance before taking-off
        float slidedDistance = 0;
        Vector3 currentPos = this.transform.position;
        while (slidedDistance < slidingDistance)
        {
            targetDirection.y = 0;  // reset the vertical direction, since we want to move on x-z plane
            moveInDirection(targetDirection, slidingForce);

            slidedDistance += (this.transform.position - currentPos).magnitude;  // update the slided distance
            currentPos = this.transform.position;

            // Yielding of any type, including null, results in the execution coming back on a later frame (next frame)
            //yield return null;
            yield return new WaitForFixedUpdate();
        }

        targetDirection = new Vector3(this.transform.forward.x, 0, this.transform.forward.z);

        // performe the taking off action while the plane is not reaching the desired height yet
        while (this.transform.position.y < desiredHeight)
        {
            // change the y direction at each frame until reaching a desired value
            if (targetDirection.y < 0.8f)
            {
                targetDirection.y += Time.deltaTime;
            }

            moveInDirection(targetDirection, flyingForce);

            // Yielding of any type, including null, results in the execution coming back on a later frame (next frame)
            //yield return null;
            yield return new WaitForFixedUpdate();
        }

        currentState = PlaneState.FINISHED_TAKING_OFF;  // update the state
    }


    /// <summary>
    /// Function for this plane to perform landing action until reaching ground level
    /// </summary>
    /// <param name="groundLevel">the ground level (plane will be considered landed after reaching the ground level)</param>
    public void landing(float groundLevel)
    {
        currentState = PlaneState.LANDING;
        StartCoroutine(SmoothLanding(groundLevel));  // start the coroutine
    }

    //Co-routine for take off function
    protected IEnumerator SmoothLanding(float groundLevel)
    {
        Vector3 targetDirection = new Vector3(this.transform.forward.x, 0, this.transform.forward.z);
        float currentFlyingForce = flyingForce;

        while (this.transform.position.y > groundLevel)
        {
            // change the y direction at each frame until reaching a desired value
            if(targetDirection.y > -0.6f)
            {
                targetDirection.y -= Time.deltaTime / 4;
            }
            if(currentFlyingForce > 0)
            {
                currentFlyingForce -= Time.deltaTime * 4;
            }
            //Debug.Log(targetDirection);
            moveInDirection(targetDirection.normalized, currentFlyingForce);

            // Yielding of any type, including null, results in the execution coming back on a later frame (next frame)
            //yield return null;
            yield return new WaitForFixedUpdate();
        }


        // sliding for a distance after reaching the ground level
        float slidedDistance = 0;
        Vector3 currentPos = this.transform.position;

        while(slidedDistance < slidingDistance)
        {
            targetDirection.y = 0;  // reset the vertical direction, since we want to move on x-z plane
            moveInDirection(targetDirection, slidingForce);

            slidedDistance += (this.transform.position - currentPos).magnitude;  // update the slided distance
            currentPos = this.transform.position;

            // Yielding of any type, including null, results in the execution coming back on a later frame (next frame)
            //yield return null;
            yield return new WaitForFixedUpdate();
        }

        currentState = PlaneState.STOP_ON_LAND;  // update the plane state
    }


    /// <summary>
    /// Function to make this plane circling at current height for a time around a center point
    /// </summary>
    /// <param name="time">the time of orbitation</param>
    /// <param name="center">the center point of the circle which this plane is orbiting around</param>
    public void circling(float time, Vector3 center)
    {
        currentState = PlaneState.CIRCLING;
        StartCoroutine(SmoothCircling(time, center));  // start the coroutine
    }

    //Co-routine for circling function
    protected IEnumerator SmoothCircling(float time, Vector3 center)
    {
        // disable the gravity and vertical velocity to simplify the physical calculation
        this.rb.useGravity = false;
        this.rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        float timer = 0f;  // count the time with a timer
        // while not exceed the time limit for circling
        while(timer <= time)
        {
            timer += Time.deltaTime;  // update the timer

            // make plane circle around the center at 35 degrees/second.
            transform.RotateAround(center, Vector3.up, 35 * Time.deltaTime);

            // Adjust the facing direction of plane
            Vector3 deltaPosition = transform.position - prevPosition;  // Calculate position change between frames
            if (deltaPosition != Vector3.zero)
            {
                // Same effect as rotating with quaternions, but simpler to read
                transform.forward = deltaPosition;
            }
            // Store current position as previous position for next frame
            prevPosition = transform.position;


            yield return new WaitForFixedUpdate();
        }

        // enable the gravity when finish circling
        this.rb.useGravity = true;

        currentState = PlaneState.FLYING;
    }
}
