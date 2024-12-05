using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 2f; // Movement speed
    public float directionChangeInterval = 3f; // Time between direction changes
    public float rotationSpeed = 5f; // Speed of rotation
    private Vector3 targetDirection;

    private Quaternion rotationOffset; // Rotation offset for initial orientation

    void Start()
    {
        // Define the rotation offset (rotate from x-axis to z-axis)
        rotationOffset = Quaternion.Euler(0, 90, 0);

        ChangeDirection(); // Set an initial direction
        InvokeRepeating(nameof(ChangeDirection), directionChangeInterval, directionChangeInterval); // Change direction periodically
    }

    void Update()
    {
        // Move the fish in the target direction
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);

        // Smoothly rotate the fish to face the target direction
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection) * rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ChangeDirection()
    {
        // Choose a random direction within a horizontal plane
        targetDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}
