using UnityEngine;

public class SwingCasting : MonoBehaviour
{
    public Transform rodTip;
    public Transform bobber;
    public Transform boat; // reference to the boat for placing caught fish
    public LineRenderer lineRenderer;
    public GameObject fishPrefab; // Prefab for the fish to spawn when caught
    
    public float catchChance = 1.0f; //100% chance to catch a fish (FOR TESTING)
    public float bobbingSpeed = 2f; // Speed of bobber bobbing animation
    public float bobbingHeight = 0.2f;
    public float castForceMultiplier = 1f;
    public float velocityThreshold = 1.5f;
    public AudioClip catchSound; // Sound effect for catching a fish
    public Vector3 bobberOffset = new Vector3(0f, -0.5f, 0f);
    private Vector3 initialBobberPosition;
    private Rigidbody bobberRb;
    private GameObject caughtFish;
    private bool isCast = false;
    private bool isInWater = false;
    private bool hasFish = false;
    private float bobbingTimer;
    private AudioSource audioSource; // Reference to the AudioSource
    private BobCollider bobberCollision;


    void Start()
    {
        bobberRb = bobber.GetComponent<Rigidbody>();
        bobberRb.isKinematic = false;
        bobberRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        initialBobberPosition = bobber.position;
        lineRenderer.positionCount = 2;
        // Initialize the AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        bobberCollision = bobber.GetComponent<BobCollider>();
        if(bobberCollision != null)
        {
            bobberCollision.OnBobberCollision += HandleBobberCollision;
        }
    }

    void Update()
    {
        lineRenderer.SetPosition(0, rodTip.position);
        lineRenderer.SetPosition(1, bobber.position);

        if(isCast && isInWater && hasFish)
        {
            Debug.Log("about to play animation");
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            bobber.position += new Vector3(0, Mathf.Sin(bobbingTimer) * bobbingHeight * Time.deltaTime, 0);
        }
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
        Debug.Log("RESET BOBBER PRESSED");
        if (hasFish)
        {
            caughtFish.transform.SetParent(boat);
            caughtFish.transform.localPosition = new Vector3(0,0.5f,0);
            caughtFish = null;
        }
        isCast = false;
        isInWater = false;
        hasFish = false;
        bobbingTimer = 0;
        bobberRb.isKinematic = true;
        bobberRb.useGravity = false;
        bobberRb.constraints = RigidbodyConstraints.None;
        bobber.position = rodTip.position + bobberOffset;
    }

    private void HandleBobberCollision(Collision other)
    {
        Debug.Log("IN WATER BEFORE COMPARE TAG IS CALLED");
        if (other.gameObject.CompareTag("Water"))
        {
            isInWater = true;
            //Debug.Log("IN WATER");
            // Stop all physics-based movement
            bobberRb.isKinematic = true;
            bobberRb.useGravity = false;
            bobberRb.velocity = Vector3.zero;
            bobberRb.angularVelocity = Vector3.zero;

            // Freeze position and rotation
            bobberRb.constraints = RigidbodyConstraints.FreezeAll;

            // Force position update
            //bobber.position = other.ClosestPoint(bobber.position);

            lineRenderer.SetPosition(0, rodTip.position);
            lineRenderer.SetPosition(1, bobber.position);
            if (Random.value <= catchChance)
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
            }
        }
    }
    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        if (bobberCollision != null)
        {
            bobberCollision.OnBobberCollision -= HandleBobberCollision;
        }
    }
}
