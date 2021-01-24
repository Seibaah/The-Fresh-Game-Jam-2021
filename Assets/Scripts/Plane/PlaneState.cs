using System;


/// <summary>
/// An enum class for the possible states of a plane
/// </summary>
public enum PlaneState
{
    FOLLOWING_PATH = 0,
    READY_TO_LAND = 1,  // when the plane is on the "landing point"
    LANDING = 2,  // when the plane is performing landing action
    STOP_ON_LAND = 3,  // post-state after landing 
    TAKING_OFF = 4,  // when the plane is performing taking off action
    FINISHED_TAKING_OFF = 5,  // post-state after taking off is done
    READY_TO_CIRCLING = 6,
    CIRCLING = 7,
    DESTROYED = 8,
    START = 9,  // initial state after the plane is generated
    PARKING = 10  // state when the plane on parking spot

}
