using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 2f; // Movement speed
    public float directionChangeInterval = 3f; // Time between direction changes
    private Vector3 targetDirection;

    void Start()
    {
        ChangeDirection(); // Set an initial direction
        InvokeRepeating(nameof(ChangeDirection), directionChangeInterval, directionChangeInterval); // Change direction periodically
    }

    void Update()
    {
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
    }

    void ChangeDirection()
    {
        // Choose a random direction within a horizontal plane
        targetDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}
