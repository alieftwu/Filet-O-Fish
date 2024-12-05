using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobCollider : MonoBehaviour
{
    public delegate void BobberCollisionHandler(Collision collision);
    public event BobberCollisionHandler OnBobberCollision;
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("IN WATER"); 
        OnBobberCollision?.Invoke(other);
        //isInWater = true;
       // Debug.Log("IN WATER");
        // Stop all physics-based movement
        //isKinematic = true;
        //useGravity = false;
        //velocity = Vector3.zero;
        //angularVelocity = Vector3.zero;

        // Freeze position and rotation
        //constraints = RigidbodyConstraints.FreezeAll;

        // Force position update
        //bobber.position = other.ClosestPoint(bobber.position);

            //lineRenderer.SetPosition(0, rodTip.position);
            //lineRenderer.SetPosition(1, bobber.position);
            /*if (Random.value <= catchChance)
            {
                hasFish = true;

                //Play bobber animation
                bobbingTimer = 0;
                Debug.Log("FISH CAUGHT: ");
                //Spawn the fish object
                caughtFish = Instantiate(fishPrefab, bobber.position, Quaternion.identity);
                // Play the sound
                if (catchSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(catchSound);
                }
            }*/
    }
}
