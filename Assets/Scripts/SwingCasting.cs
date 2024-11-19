using UnityEngine;

public class SwingCasting : MonoBehaviour
{
    public Transform rodTip; 
    public Transform bobber; 
    public LineRenderer lineRenderer; 
    public float castForceMultiplier = 2f;
    public float velocityThreshold = 1.5f;

    private Vector3 initialBobberPosition;
    private Rigidbody bobberRb; 
    private bool isCast = false; 
    private bool isInWater = false;

    void Start()
    {
        bobberRb = bobber.GetComponent<Rigidbody>();
        bobberRb.isKinematic = true;
        bobberRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Improved collision detection
        initialBobberPosition = bobber.position;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        lineRenderer.SetPosition(0, rodTip.position);
        lineRenderer.SetPosition(1, bobber.position);

        if (!isCast && !isInWater)
        {
            bobber.position = rodTip.position;
        }

        Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        if (!isCast && !isInWater && controllerVelocity.magnitude > velocityThreshold)
        {
            CastBobber(controllerVelocity);
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ResetBobber();
        }
    }

    void CastBobber(Vector3 velocity)
    {
        isCast = true;
        bobberRb.isKinematic = false; 
        bobberRb.velocity = velocity * castForceMultiplier; 
    }

    void ResetBobber()
    {
        isCast = false;
        isInWater = false;
        bobberRb.isKinematic = true;
        bobber.position = rodTip.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("Bobber hit the water!"); // Debugging line
            isInWater = true;
            bobberRb.isKinematic = true;
            bobberRb.velocity = Vector3.zero;
            lineRenderer.SetPosition(0, rodTip.position);
            lineRenderer.SetPosition(1, bobber.position);
        }
    }

    void FixedUpdate()
    {
        // Cap bobber velocity to prevent unrealistic speeds
        if (bobberRb.velocity.magnitude > 10f)
        {
            bobberRb.velocity = bobberRb.velocity.normalized * 10f;
        }
    }
}
