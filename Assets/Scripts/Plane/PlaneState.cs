using System;


/// <summary>
/// An enum class for the possible states of a plane
/// </summary>
public enum PlaneState
{
    FLYING = 0,  
    LANDING = 1,  // when the plane is performing landing action
    STOP_ON_LAND = 2,  // post-state after landing 
    TAKING_OFF = 3,  // when the plane is performing taking off action
    FINISHED_TAKING_OFF = 4,  // post-state after taking off is done
    CIRCLING = 5,
    DESTROYED

}
