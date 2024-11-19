using UnityEngine;

public class SwingCasting : MonoBehaviour
{
    public Transform rodTip; // Reference to the rod tip object.
    public Transform bobber; // Reference to the bobber object.
    public LineRenderer lineRenderer; // Reference to the LineRenderer for the fishing line.
    public float castForceMultiplier = 2f; // Multiplier for the cast force based on controller velocity.
    public float velocityThreshold = 1.5f; // Minimum velocity to trigger the cast.

    private Vector3 initialBobberPosition; // Store the bobber's initial position.
    private Rigidbody bobberRb; // Bobber's Rigidbody.
    private bool isCast = false; // Track if the bobber is currently cast.
    private bool isInWater = false; // Track if the bobber is in the water.

    void Start()
    {
        bobberRb = bobber.GetComponent<Rigidbody>();
        bobberRb.isKinematic = true; // Keep the bobber static at the start.
        initialBobberPosition = bobber.position; // Save the bobber's initial position.
        lineRenderer.positionCount = 2; // LineRenderer should have two points.
    }

    void Update()
    {
        // Update the LineRenderer to simulate the fishing line.
        lineRenderer.SetPosition(0, rodTip.position); // Starting point at the rod tip.
        lineRenderer.SetPosition(1, bobber.position); // End point at the bobber.

        // Get the controller's velocity.
        Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        // Detect a swing motion based on velocity, and only allow casting if not already cast or in the water.
        if (!isCast && !isInWater && controllerVelocity.magnitude > velocityThreshold)
        {
            CastBobber(controllerVelocity);
        }

        // Reset bobber position if the player presses a button (optional).
        if (OVRInput.GetDown(OVRInput.Button.One)) // "A" button to reset.
        {
            ResetBobber();
        }
    }

    void CastBobber(Vector3 velocity)
    {
        isCast = true;
        bobberRb.isKinematic = false; // Enable physics for casting.
        bobberRb.velocity = velocity * castForceMultiplier; // Apply force based on controller velocity.
    }

    void ResetBobber()
    {
        // Reset everything to allow another cast.
        isCast = false;
        isInWater = false;
        bobberRb.isKinematic = true; // Stop physics interactions.
        bobber.position = initialBobberPosition; // Reset bobber position.
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bobber collides with the water.
        if (other.CompareTag("Water"))
        {
            isInWater = true; // Mark the bobber as in the water.
            bobberRb.isKinematic = true; // Stop the bobber's movement.
            bobberRb.velocity = Vector3.zero; // Reset velocity to stop it completely.
        }
    }
}
