using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator;
    private GameObject player;
    private float interactDistance = 3f;  // interaction distance for the opening of door

    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = this.GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.transform.position, this.transform.position) <= interactDistance)
        {
            doorAnimator.SetBool("character_nearby", true);
        }
        else
        {
            doorAnimator.SetBool("character_nearby", false);
        }
    }
}
