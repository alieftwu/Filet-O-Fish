using UnityEngine;
using System.Collections;

public class SwingCasting : MonoBehaviour
{
    public Transform rodTip;
    public Transform bobber;
    public Transform boat; // Reference to the boat for placing caught fish
    public LineRenderer lineRenderer;
    public GameObject fishPrefab; // Prefab for the fish to spawn when caught
    public GameObject splashEffectPrefab; // Prefab for the splash effect

    //public float catchChance = 1.0f; // 100% chance to catch a fish (FOR TESTING)
    public float bobbingSpeed = 2f; // Speed of bobber bobbing animation
    public float bobbingHeight = 0.2f;
    public float castForceMultiplier = 1f;
    public float velocityThreshold = 1.5f;
    public AudioClip catchSound; // Sound effect for catching a fish
    public Vector3 bobberOffset = new Vector3(0f, -0.5f, 0f);
    public float sinkDepth = -0.1f; // How far the bobber sinks on collision
    public float sinkDuration = 0.2f; // How long the sinking animation lasts

    private Vector3 initialBobberPosition;
    private Rigidbody bobberRb;
    private GameObject caughtFish;
    private bool isCast = false;
    private bool isInWater = false;
    private bool hasFish = false;
    private float bobbingTimer;
    private AudioSource audioSource; // Reference to the AudioSource
    private BobCollider bobberCollision;
    private float offset = 0f;

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
        if (bobberCollision != null)
        {
            bobberCollision.OnBobberCollision += HandleBobberCollision;
        }
    }

    void Update()
    {
        lineRenderer.SetPosition(0, rodTip.position);
        lineRenderer.SetPosition(1, bobber.position);

        if (isCast && isInWater)
        {
            // Bobbing animation
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingHeight * Time.deltaTime;
            bobber.position += new Vector3(0, bobbingOffset, 0);

            // Make the fish follow the bobber's bobbing
            if (hasFish && caughtFish != null)
            {
                caughtFish.transform.position = bobber.position + new Vector3(0, -0.2f, 0); // Keep fish slightly below the bobber
            }
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

    private int fishCatchCount = 0; // Counter for the number of times the block executes
    private const int maxFishCount = 3; // Maximum number of fish that can be processed

    void ResetBobber()
    {
        if (hasFish)
        {
            if (fishCatchCount < maxFishCount)
            {
                caughtFish.transform.SetParent(boat);
                caughtFish.transform.localPosition = new Vector3(0f + offset, 0.4f, 0);
                caughtFish.transform.localRotation = Quaternion.Euler(90f, 0f, 90f);
                offset -= 0.3f;
            }
            else
            {
                Destroy(caughtFish); // Delete the fish if the limit is exceeded
            }

            caughtFish = null; // Clear the reference to the caught fish
            fishCatchCount++; // Increment the counter
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
        if (other.gameObject.CompareTag("Water"))
        {
            isInWater = true;

            // Stop all physics-based movement
            bobberRb.isKinematic = true;
            bobberRb.useGravity = false;
            bobberRb.velocity = Vector3.zero;
            bobberRb.angularVelocity = Vector3.zero;

            // Freeze position and rotation
            bobberRb.constraints = RigidbodyConstraints.FreezeAll;

            // Smooth sinking effect for bobber
            Vector3 bobberSinkPosition = bobber.position + new Vector3(0, sinkDepth, 0);
            StartCoroutine(SinkBobber(bobberSinkPosition, sinkDuration));

            // Instantiate splash particle effect
            if (splashEffectPrefab != null)
            {
                GameObject splashEffect = Instantiate(splashEffectPrefab, bobber.position, Quaternion.identity);
                Destroy(splashEffect, 0.5f); // Adjust duration as necessary
            }

            StartCoroutine(WaitForCatch());
        }
    }
    private IEnumerator WaitForCatch()
    {
        //Wait between 10-45 seconds for catch
        float waitTime = Random.Range(25f, 50f);
        yield return new WaitForSeconds(waitTime);

        
        hasFish = true;

        // Spawn the fish object slightly below the water
        Vector3 fishSpawnPosition = bobber.position + new Vector3(0, -0.2f, 0); // Adjust -0.2f for desired sink depth
        caughtFish = Instantiate(fishPrefab, fishSpawnPosition, Quaternion.identity);

        // Play the catch sound
        if (catchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(catchSound);
        }
    }
    

    private IEnumerator SinkBobber(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = bobber.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bobber.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        bobber.position = targetPosition; // Ensure it finishes at the exact target position
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