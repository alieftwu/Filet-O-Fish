using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovementWithMotor : MonoBehaviour
{
    [Header("References")]
    public Transform motorHandle; // Reference to the motor handle GameObject
    public Rigidbody boatRigidbody; // Reference to the boat's Rigidbody

    [Header("Movement Settings")]
    public float forwardAcceleration = 5f;
    public float maxSpeed = 10f;
    public float rotationSpeed = 0.5f; // Degrees per second
    public float maxRotationSpeed = 2f;
    public float drag = 0.98f;

    private float currentSpeed = 0f;

    void FixedUpdate() // Use FixedUpdate for physics-related logic
    {
        if (motorHandle != null && boatRigidbody != null)
        {
            HandleMotorInput();
        }
        else
        {
            Debug.LogWarning("Motor Handle or Boat Rigidbody is not assigned!");
        }
    }

    void HandleMotorInput()
    {
        // Get the local rotation of the motor handle
        Vector3 handleRotation = motorHandle.localEulerAngles;

        // Normalize rotation angles (-180 to 180 range)
        if (handleRotation.x > 180f) handleRotation.x -= 360f;
        if (handleRotation.z > 180f) handleRotation.z -= 360f;

        // Forward/backward speed based on handle tilt (x-axis rotation)
        if (handleRotation.x < -90f) // Tilted down (forward)
        {
            currentSpeed += forwardAcceleration * Time.fixedDeltaTime;
        }
        else if (handleRotation.x > -90f) // Tilted up (backward)
        {
            currentSpeed -= forwardAcceleration * Time.fixedDeltaTime;

            // Prevent reversing
            if (currentSpeed < 0f)
                currentSpeed = 0f;
        }

        // Clamp speed
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        // Turning logic based on yaw (z-axis rotation)
        float yawInput = -handleRotation.z * 0.2f; // Flip rotation direction
        float turnAmount = yawInput * rotationSpeed * Time.fixedDeltaTime;
        float clampedTurnAmount = Mathf.Clamp(turnAmount, -maxRotationSpeed, maxRotationSpeed);

        // Apply rotation using MoveRotation to only rotate on the Y-axis
        Quaternion deltaRotation = Quaternion.Euler(0, clampedTurnAmount, 0);
        boatRigidbody.MoveRotation(boatRigidbody.rotation * deltaRotation);

        // Move the boat forward based on the current speed (only in the Z-direction)
        Vector3 forwardDirection = boatRigidbody.transform.TransformDirection(Vector3.forward).normalized; // Local forward direction, normalized
        boatRigidbody.velocity = forwardDirection * currentSpeed; // Only update velocity in forward direction

        // Apply drag to gradually reduce speed
        currentSpeed *= drag;

        // Stop the boat completely if speed is very low
        if (currentSpeed < 0.01f)
            currentSpeed = 0f;
    }
}
