using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingController : MonoBehaviour
{
    public Transform rodTip;         // The tip of the fishing rod
    public Rigidbody bobberRb;      // The Rigidbody on the bobber
    public float castForce = 10f;   // Adjust as needed
    
    private bool isCasting = false;
    private Vector3 lastPosition;

    void Update()
    {
        // Detect casting motion (fast forward motion)
        if (Input.GetButtonDown("Cast")) // Replace with actual VR button
        {
            isCasting = true;
            lastPosition = rodTip.position;
        }

        if (isCasting)
        {
            Vector3 movement = rodTip.position - lastPosition;
            if (movement.magnitude > 0.2f)  // Threshold for casting
            {
                bobberRb.velocity = rodTip.forward * castForce;
                isCasting = false;  // Prevent multiple casts
            }
            lastPosition = rodTip.position;
        }
    }
}
