using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovementWithMotor : MonoBehaviour
{
    [Header("References")]
    public Transform motorHandle; // Reference to the motor handle GameObject

    [Header("Movement Settings")]
    public float forwardAcceleration = 5f;
    public float maxSpeed = 20f;
    public float rotationSpeed = 50f;
    public float drag = 0.98f;

    private float currentSpeed = 0f;

    void Update()
    {
        if (motorHandle != null)
        {
            HandleMotorInput();
            MoveBoat();
        }
        else
        {
            Debug.LogWarning("Motor Handle is not assigned!");
        }
    }

    void HandleMotorInput()
    {
        // Get the rotation of the motor handle
        Vector3 handleRotation = motorHandle.localEulerAngles;

        // Normalize rotation angles (-180 to 180 range)
        if (handleRotation.x > 180f) handleRotation.x -= 360f;
        if (handleRotation.y > 180f) handleRotation.y -= 360f;

        // Determine forward/backward speed based on handle pitch (x-axis tilt)
        if (handleRotation.x > 0f) // Tilted up
        {
            currentSpeed += forwardAcceleration * Time.deltaTime;
        }
        else if (handleRotation.x < 0f) // Tilted down
        {
            currentSpeed -= forwardAcceleration * Time.deltaTime;
        }

        // Clamp speed
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Determine turning based on handle yaw (y-axis rotation)
        float rotationInput = handleRotation.y;
        transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
    }

    void MoveBoat()
    {
        // Apply forward/backward movement
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Apply drag
        currentSpeed *= drag;
    }
}
