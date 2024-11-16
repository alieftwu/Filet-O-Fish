using UnityEngine;
using UnityEngine.InputSystem;

public class FishingController : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer; // Assign the Line Renderer
    public Transform rodTip;         // The tip of the fishing rod
    public Transform bobber;         // The bobber object in the water
    public Rigidbody bobberRb;       // The Rigidbody of the bobber

    [Header("Casting Settings")]
    public float castPower = 10f;    // Force applied to the bobber when casting
    public float castThreshold = 0.2f; // Minimum movement to detect a cast motion

    [Header("Input")]
    public InputActionProperty castAction; // Input action for casting

    private bool isReadyToCast = false; // Tracks if the player is ready to cast
    private Vector3 lastRodTipPosition; // Previous frame's position of the rod tip

    void Start()
    {
        // Initialize the Line Renderer with two positions (start and end of the line)
        lineRenderer.positionCount = 2;
        lastRodTipPosition = rodTip.position; // Initialize rod tip position tracking
    }

    void Update()
    {
        HandleInput();
        DetectCastingMotion();
        UpdateFishingLine();
    }

    // Handle input for preparing to cast
    void HandleInput()
    {
        // Check if the cast action is triggered
        if (castAction.action.WasPressedThisFrame())
        {
            isReadyToCast = true; // Player is ready to cast
        }
    }

    // Detect motion to cast the line
    void DetectCastingMotion()
    {
        if (isReadyToCast)
        {
            // Calculate the rod tip's movement
            Vector3 currentRodTipPosition = rodTip.position;
            Vector3 rodTipMovement = currentRodTipPosition - lastRodTipPosition;

            // Check if the motion meets the casting threshold
            if (rodTipMovement.magnitude > castThreshold && rodTip.forward.z > 0.7f)
            {
                CastLine(); // Execute casting logic
                isReadyToCast = false; // Reset the casting state
            }

            // Update the last position
            lastRodTipPosition = currentRodTipPosition;
        }
    }

    // Apply force to the bobber to simulate a cast
    void CastLine()
    {
        // Apply a forward force to the bobber
        bobberRb.velocity = rodTip.forward * castPower;
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
