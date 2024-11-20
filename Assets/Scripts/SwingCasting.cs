using UnityEngine;

public class SwingCasting : MonoBehaviour
{
    public Transform rodTip;
    public Transform bobber;
    public LineRenderer lineRenderer;
    public float castForceMultiplier = 2f;
    public float velocityThreshold = 1.5f;
    public Vector3 bobberOffset = new Vector3(0f, -0.5f, 0f);

    private Vector3 initialBobberPosition;
    private Rigidbody bobberRb;
    private bool isCast = false;
    private bool isInWater = false;

    void Start()
    {
        bobberRb = bobber.GetComponent<Rigidbody>();
        bobberRb.isKinematic = true;
        bobberRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        initialBobberPosition = bobber.position;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        lineRenderer.SetPosition(0, rodTip.position);
        lineRenderer.SetPosition(1, bobber.position);

        if (!isCast && !isInWater)
        {
            bobber.position = rodTip.position + bobberOffset;
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
        bobberRb.useGravity = true;
        bobberRb.constraints = RigidbodyConstraints.None;
        bobberRb.velocity = velocity * castForceMultiplier;
    }

    void ResetBobber()
    {
        isCast = false;
        isInWater = false;
        bobberRb.isKinematic = true;
        bobberRb.useGravity = false;
        bobberRb.constraints = RigidbodyConstraints.None;
        bobber.position = rodTip.position + bobberOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;

            // Stop all physics-based movement
            bobberRb.isKinematic = true;
            bobberRb.useGravity = false;
            bobberRb.velocity = Vector3.zero;
            bobberRb.angularVelocity = Vector3.zero;

            // Freeze position and rotation
            bobberRb.constraints = RigidbodyConstraints.FreezeAll;

            // Force position update
            bobber.position = other.ClosestPoint(bobber.position);

            lineRenderer.SetPosition(0, rodTip.position);
            lineRenderer.SetPosition(1, bobber.position);
        }
    }
}
