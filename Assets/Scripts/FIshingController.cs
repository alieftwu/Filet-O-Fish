using UnityEngine;

public class FishingController : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer; // Assign the Line Renderer
    public Transform rodTip;         // The tip of the fishing rod
    public Transform bobber;         // The bobber object in the water
    public Rigidbody bobberRb;       // The Rigidbody of the bobber

    [Header("Casting Settings")]
    public float castPower = 5f;     // Force applied to the bobber when casting
    public float castThreshold = 0.2f; // Minimum forward movement to detect a cast motion

    private Vector3 lastRodTipPosition; // Previous frame's position of the rod tip
    private bool isCasting = false;    // Prevents repeated casts
    private bool isGrounded;

    void Start()
    {
        // Initialize the Line Renderer with two positions (start and end of the line)
        lineRenderer.positionCount = 2;
        lastRodTipPosition = rodTip.position; // Initialize rod tip position tracking
    }

    void Update()
    {
        DetectCastingMotion();
        UpdateFishingLine();
        isGrounded = CheckIfBobberIsGrounded();  // Use a method to check if the bobber is grounded
    }

    // Check if the bobber is touching the ground
    bool CheckIfBobberIsGrounded()
    {
        // Raycast downwards from the bobber to detect if it's on the ground
        RaycastHit hit;
        if (Physics.Raycast(bobber.position, Vector3.down, out hit, 1f))
        {
            return true; // Bobber is grounded
        }
        return false; // Bobber is not grounded
    }

    // Detect motion to cast the line
    void DetectCastingMotion()
    {
        // Calculate the rod tip's movement
        Vector3 currentRodTipPosition = rodTip.position;
        Vector3 rodTipMovement = currentRodTipPosition - lastRodTipPosition;

        Debug.Log($"Rod Tip Movement: {rodTipMovement.magnitude}, Forward Component: {rodTipMovement.z}");

        // Check if the rod tip moved forward sufficiently
        if (!isCasting && rodTipMovement.z > castThreshold)
        {
            Debug.Log("Casting motion detected!");
            if (isGrounded)  // Check if the bobber is grounded before casting
            {
                CastLine();
            }
            isCasting = true; // Prevent multiple casts
        }

        // Reset casting state when the rod tip stabilizes
        if (isCasting && rodTipMovement.magnitude < 0.01f)
        {
            isCasting = false; // Ready for the next cast
        }

        // Update the last position
        lastRodTipPosition = currentRodTipPosition;
    }

    // Apply force to the bobber to simulate a cast
    void CastLine()
    {
        // Apply a forward force to the bobber
        bobberRb.velocity = rodTip.forward * castPower * 0.75f;
        Debug.Log($"Bobber Velocity: {bobberRb.velocity}");
        Debug.Log("Line cast!");
    }

    // Dynamically update the fishing line's position
    void UpdateFishingLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, rodTip.position); // Start of the line (rod tip)
            lineRenderer.SetPosition(1, bobber.position); // End of the line (bobber)
        }
    }
}