using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public float flyingForce = 20f;  // the force apply to this plane when flying
    public float slidingForce = 12f;  // the force applyig to this plane when sliding on the ground
    public float slidingDistance = 30f;  // the distance of sliding
    public float gas_limit_time = 40f;  // the time (in seconds) before this plane becomes out of gas

    public PlaneState currentState { get; set; }  // the current state of this plane

    private Rigidbody rb;  // the rigidbody component of this plane

    // Variable for the plane motion:
    private Vector3 prevPosition;  // used for adjusting facing direction
    private bool isFollowingPath;  // whether the plane is following a path
    private Coroutine currentMovingCoroutine;  // the current FollowPath coroutine or the Circling coroutine
    private float gasTimer = 0;  // the timer for counting the gas of this plane
    

    // Just for testing
    private float desiredHeight = 100f;  // the desired height for take off action
    private float groundLevel = 6f;  // the ground level for landing action



    // Start is called before the first frame update
    void Start()
    {
        // initialize the variable fields
        rb = this.GetComponent<Rigidbody>();
        currentState = PlaneState.START;  // initial state

        /*
        // testing following a path
        List<Vector3> path = new List<Vector3>();
        path.Add(new Vector3(180, 82.2f, -60));
        path.Add(new Vector3(68, 82.2f, -100));
        path.Add(new Vector3(150, 82.2f, -170));
        path.Add(new Vector3(207.53f, 82.2f, -113.52f));
        followPath(path, false, new Vector3(-4.2f, 2, -17.8f));
        */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
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
        */

        /*
        // Fot testing 
        if (currentState == PlaneState.READY_TO_LAND)
        {
            landing(groundLevel);
        }
        else if (currentState == PlaneState.STOP_ON_LAND)
        {
            takeOff(desiredHeight);
        }
        else if (currentState == PlaneState.READY_TO_CIRCLING)
        {
            float range = 50f;
            // randomly pick a center point around the plane for circling
            Vector2 randomPoint = Random.insideUnitCircle;
            circling(this.transform.position + new Vector3(randomPoint.x, 0 , randomPoint.y) * range);
            //Destroy(this.gameObject, 21);
        }*/

        // Update gas timer
        gasTimer += Time.deltaTime;
        checkGas();  // check the gas timer

        // The State Machine of this plane
        // When the plane is just generated
        if (currentState == PlaneState.START)
        {
            float airportRange = 190f;  // the approx radius of airport map
            Vector2 centerOfAirport = new Vector2(0, 0);  // the center coord of airport

            // flying to a random point above the airport
            List<Vector3> path = new List<Vector3>();
            Vector2 randomPoint = Random.insideUnitCircle;
            randomPoint = centerOfAirport + randomPoint * airportRange;  // map the random point to the airport

            path.Add(new Vector3(randomPoint.x, this.transform.position.y, randomPoint.y));  // add the random point to the path
            followPath(path, false, Vector3.zero);  // move to this random point (the third parameter is irrelevant when the second is false)
        }
        // when the plane is ready to circling
        else if(currentState == PlaneState.READY_TO_CIRCLING)
        {
            float range = 50f;
            // randomly pick a center point around the plane for circling
            Vector2 randomPoint = Random.insideUnitCircle;
            circling(this.transform.position + new Vector3(randomPoint.x, 0, randomPoint.y) * range);
        }
        // when the plane is ready to land
        else if (currentState == PlaneState.READY_TO_LAND)
        {
            landing(groundLevel);
        }
        // when the plane is ready to teleport to the garage
        else if(currentState == PlaneState.STOP_ON_LAND)
        {
            // TODO: goto garage
        }
        else if(currentState == PlaneState.FINISHED_TAKING_OFF)
        {
            // destroy this plane after a delay
            Destroy(this.gameObject, 5f);
        }




    }


    /// <summary>
    /// Function to check whether this plane is out of gas (using gas timer)
    /// </summary>
    private void checkGas()
    {
        // when the plane is flying and out of gas
        if(this.gasTimer > this.gas_limit_time && (currentState == PlaneState.CIRCLING || currentState == PlaneState.FOLLOWING_PATH))
        {
            // stop the current moving coroutine (let this plane falling)
            StopCoroutine(currentMovingCoroutine);
            this.currentState = PlaneState.DESTROYED;
            this.rb.useGravity = true;  // apply gravity
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
        //this.rb.velocity = targetDirection * force;

        /*
        // change the facing direction of this plane based on the current velocity
        Vector3 facingDirection = rb.velocity;
        facingDirection.y = 0;
        transform.forward = rb.velocity;
        */

    }


    /// <summary>
    /// Function to make this plane facing to a given destination
    /// </summary>
    /// <param name="destination"></param>
    private void faceTo(Vector3 destination)
    {
        transform.forward = new Vector3(destination.x, 0, destination.z);
    }    


    /// <summary>
    /// Function for this plane to perform takeOff action (along the current facing direction) until reaching the desired height
    /// </summary>
    /// <param name="desiredHeight">the desired height of this plane will reach after taking-off is done</param>
    public void takeOff(float desiredHeight)
    {
        // TODO: teleport to a run way

        currentState = PlaneState.TAKING_OFF;
        StartCoroutine(SmoothTakeOff(desiredHeight));  // start the coroutine
    }


    //Co-routine for take off function
    protected IEnumerator SmoothTakeOff(float desiredHeight)
    {
        this.rb.useGravity = true;

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
    private void landing(float groundLevel)
    {
        currentState = PlaneState.LANDING;
        StartCoroutine(SmoothLanding(groundLevel));  // start the coroutine
    }

    //Co-routine for take off function
    protected IEnumerator SmoothLanding(float groundLevel)
    {
        this.rb.useGravity = true;

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
    /// Function to make this plane circling at current height around a center point
    /// </summary>
    /// <param name="center">the center point of the circle which this plane is orbiting around</param>
    private void circling(Vector3 center)
    {
        currentState = PlaneState.CIRCLING;
        currentMovingCoroutine = StartCoroutine(SmoothCircling(center));  // start the coroutine
    }

    //Co-routine for circling function
    protected IEnumerator SmoothCircling(Vector3 center)
    {
        // disable the gravity and vertical velocity to simplify the physical calculation
        this.rb.useGravity = false;
        this.rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //float timer = 0f;  // count the time with a timer

        // while not exceed the time limit for circling
        while(true)
        {
            //timer += Time.deltaTime;  // update the timer

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

        //currentState = PlaneState.FLYING;
    }



    /// <summary>
    /// Function to make this plane following a given path. If the last point in this path is the landing point,
    /// the plane will face to the runway position for performing landing action 
    /// </summary>
    /// <param name="path">A list containing all the points in the path</param>
    /// <param name="isDestLandingPoint">Whether the last point in the path is a landing point</param>
    /// <param name="runwayPosition">The position of the center of the corresponding runway for the landing point. (will only be used when isDestLandingPoint==true)</param>
    public void followPath(List<Vector3> path, bool isDestLandingPoint, Vector3 runwayPosition)
    {
        if(path.Count == 0)
        {
            Debug.LogError("The path is empty");
        }
        else if(isDestLandingPoint && runwayPosition == Vector3.zero)
        {
            Debug.LogError("The position of the center of the corresponding runway is not given");
        }
        else
        {
            // if the plane is currently following a path or circling (post-states of FollowingPath state)
            if(currentState == PlaneState.FOLLOWING_PATH || currentState == PlaneState.CIRCLING)
            {
                // stop the current Moving coroutine
                StopCoroutine(currentMovingCoroutine);

                currentState = PlaneState.FOLLOWING_PATH;
                currentMovingCoroutine = StartCoroutine(FollowPath(path, isDestLandingPoint, runwayPosition));  // start the FollowPath coroutine
            }
        }
    }

    //Co-routine for following a path
    protected IEnumerator FollowPath(List<Vector3> path, bool isDestLandingPoint, Vector3 runwayPosition)
    {
        // disable the gravity and vertical velocity to simplify the physical calculation
        this.rb.useGravity = false;

        Vector3 direction;
        float epsilon = 1f;  // the epsilon for checking the distance

        // moving along the given path
        foreach(Vector3 destination in path)
        {
            // calculate the direction of motion
            direction = (destination - this.transform.position).normalized;

            // face to the destination
            faceTo(direction);

            // keep moving until reaching the destination
            while (Vector3.Distance(destination, this.transform.position) > epsilon)
            {
                moveInDirection(direction, flyingForce);  // move toward the destination
                yield return new WaitForFixedUpdate();
            }
            this.rb.velocity = Vector3.zero;  // reset the current velocity to 0 when turning to the next destination (for cancel previous force applied to the plane)
        }

        // if the last point in the path is a landing point, make the plane facing to the corresponding runway position
        if(isDestLandingPoint)
        {
            // calculate the direction of runway
            direction = (runwayPosition - this.transform.position).normalized;

            // face to the runway
            faceTo(direction);

            currentState = PlaneState.READY_TO_LAND;  // change the state of the plane
        }
        else
        {
            currentState = PlaneState.READY_TO_CIRCLING;  // change the state of the plane
        }

        // enable the gravity when finish following
        this.rb.useGravity = true;
    }


    /// <summary>
    /// Function to detect collision on this plane
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // when this plane is colliding with another plane
        if(collision.gameObject.tag == "Plane")
        {
            /*
            // if the plane is following a path
            if(isFollowingPath)
            {
                // stop the FollowPath coroutine
                StopCoroutine(currentFollowPathCoroutine);
            }*/
            // TODO: action after colliding: falling to the ground?


            // TODO: notify the game flow manager maybe


        }
    }
}
