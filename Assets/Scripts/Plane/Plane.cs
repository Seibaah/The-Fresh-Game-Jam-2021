using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    // Public fields:
    [Header("Plane Motion Settings")]
    public float flyingSpeed = 40f;  // the speed of this plane when flying
    public float flyingForce = 20f;  // the force apply to this plane when landing and taking off
    public float slidingForce = 12f;  // the force applyig to this plane when sliding on the ground
    public float slidingDistance = 30f;  // the distance of sliding
    public float circlingSpeed = 40f;   // the angular speed of circling (degrees/second).

    [Header("Test")]
    public bool testMode = false;  // turn on/off the test mode for landing and taking-off

    public PlaneState currentState { get; set; }  // the current state of this plane
    public float gas_limit_time { get; set; }  // the time (in seconds) before this plane becomes out of gas

    [HideInInspector]
    public float gasTimer = 0;  // the timer for counting the gas of this plane

    // Private fields:
    private Rigidbody rb;  // the rigidbody component of this plane
    private ParkingSpotManager psManager;  // the instance of ParkingSpotManager script
    private TaskScheduler taskScheduler;  // the instance of TaskScheduler script
    private PlaneTask planeTask;

    // Variable for the plane motion:
    private Vector3 prevPosition;  // used for adjusting facing direction
    private Coroutine currentMovingCoroutine;  // the current FollowPath coroutine or the Circling coroutine
    
    

    // Just for testing
    private float desiredHeight = 100f;  // the desired height for take off action
    private float groundLevel = 6f;  // the ground level for landing action



    // Start is called before the first frame update
    void Start()
    {
        // initialize the variable fields
        rb = this.GetComponent<Rigidbody>();
        psManager = GameObject.Find("ParkingSystem").GetComponent<ParkingSpotManager>();  // Find instance of ParkingSpotManager script
        taskScheduler = GameObject.Find("TaskScheduler").GetComponent<TaskScheduler>();
        planeTask = this.gameObject.GetComponent<PlaneTask>();

        currentState = PlaneState.START;  // initial state
        gas_limit_time = planeTask.gasTime;  // get the gas time from the task script
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        // Update gas timer
        gasTimer += Time.deltaTime;
        checkGas();  // check the gas timer

        // -------------------------------------
        // The State Machine of this plane
        // -------------------------------------
        // when the plane is just generated
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
        // when the plane is ready to teleport to the parking spot
        else if(currentState == PlaneState.STOP_ON_LAND)
        {
            // Goto parking spot
            Vector3 parkingSpotPosition = psManager.findParkingSpot();

            // findParkingSpot() will return Vector3.zero when no available parking spot
            if (parkingSpotPosition != Vector3.zero)
            {
                this.transform.position = parkingSpotPosition;
                currentState = PlaneState.PARKING;  // change the state of plane to PARKING
            }
        }
        else if(currentState == PlaneState.FINISHED_TAKING_OFF)
        {
            float delay = 4f;

            // destroy this plane after a delay
            Destroy(this.gameObject, delay);
        }
        // -------------------------------------
        // -------------------------------------

        #region Landing&Taking-off test
        if(testMode)
        {
            // for testing landing
            if (gasTimer >= 5)
            {
                Vector3 landingPoint = new Vector3(206, 82, -113.5f);
                Vector3 runway1Position = new Vector3(-4.2f, 2, -17.8f);

                List<Vector3> path = new List<Vector3>();
                path.Add(landingPoint);  // let the end point be a landing point
                followPath(path, true, runway1Position);
            }

            // for testing taking off
            if (gasTimer >= 400 && currentState == PlaneState.PARKING)
            {
                Vector3 startPoint1 = new Vector3(122, 2, -73);
                Vector3 runway1Position = new Vector3(-4.2f, 2, -17.8f);
                takeOff(desiredHeight, startPoint1, runway1Position);
            }
        }
        #endregion

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

            // Notify the task scheduler
            taskScheduler.planeCrashes();
        }
    }

    /// <summary>
    /// Funtion for this plane to move toward a given direction with a given force 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    private void moveInDirectionWithForce(Vector3 targetDirection, float force)
    {
        this.rb.AddForce(targetDirection * force);  // apply force of x-z direction
    }

    /// <summary>
    /// Funtion for this plane to move toward a given direction with a given speed 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    private void moveInDirectionWithSpeed(Vector3 targetDirection, float speed)
    {
        this.rb.velocity = targetDirection * speed;
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
    /// <param name="startPoint">the start point of the runway for takingoff</param>
    /// <param name="runwayPosition">The position of the center of the corresponding runway for the landing point.</param>
    public void takeOff(float desiredHeight, Vector3 startPoint, Vector3 runwayPosition)
    {
        // can only perform take off when the plane is parking
        if(currentState == PlaneState.PARKING)
        {
            this.gasTimer = -Mathf.Infinity; // reset the timer to -infinity, since we don't care about the gas after the plane has taken off

            // teleport to a run way
            this.transform.position = startPoint;
            // calculate the direction of the runway
            Vector3 direction = (runwayPosition - this.transform.position).normalized;
            // face to the runway
            faceTo(direction);

            currentState = PlaneState.TAKING_OFF;
            currentMovingCoroutine = StartCoroutine(SmoothTakeOff(desiredHeight));  // start the coroutine
        }
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
            moveInDirectionWithForce(targetDirection, slidingForce);

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

            moveInDirectionWithForce(targetDirection, flyingForce);

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
        currentMovingCoroutine = StartCoroutine(SmoothLanding(groundLevel));  // start the coroutine
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
            moveInDirectionWithForce(targetDirection.normalized, currentFlyingForce);

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
            moveInDirectionWithForce(targetDirection, slidingForce);

            slidedDistance += (this.transform.position - currentPos).magnitude;  // update the slided distance
            currentPos = this.transform.position;

            // Yielding of any type, including null, results in the execution coming back on a later frame (next frame)
            //yield return null;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() => this.rb.velocity == Vector3.zero); // wait until the plane is fully stopped

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

            // make plane circle around the center by a circlingSpeed
            transform.RotateAround(center, Vector3.up, circlingSpeed * Time.deltaTime);

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
        //this.rb.useGravity = true;
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
            else if(currentState == PlaneState.START)  // when the plane is just generated
            {
                currentState = PlaneState.FOLLOWING_PATH;
                currentMovingCoroutine = StartCoroutine(FollowPath(path, isDestLandingPoint, runwayPosition));  // start the FollowPath coroutine
            }

            // otherwise (plane is in other states), do nothing
        }
    }

    //Co-routine for following a path
    protected IEnumerator FollowPath(List<Vector3> path, bool isDestLandingPoint, Vector3 runwayPosition)
    {
        // disable the gravity and vertical velocity to simplify the physical calculation
        this.rb.useGravity = false;

        Vector3 direction;
        float epsilon = 0.8f;  // the epsilon for checking the distance

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
                moveInDirectionWithSpeed(direction, flyingSpeed);  // move toward the destination
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
        if(collision.gameObject.tag == "Plane" && this.currentState != PlaneState.DESTROYED)
        {
            // Stop the FollowPath coroutine
            StopCoroutine(currentMovingCoroutine);

            // Let the plane falling to the ground
            this.currentState = PlaneState.DESTROYED;  // change the plane state
            this.rb.useGravity = true;  // apply gravity

            // Notify the task scheduler
            taskScheduler.planeCrashes();
        }
    }
}
