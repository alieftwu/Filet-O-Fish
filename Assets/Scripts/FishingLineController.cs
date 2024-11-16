using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLineController : MonoBehaviour
{
    public LineRenderer lineRenderer; // Assign the Line Renderer in the Inspector
    public Transform rodTip;         // The tip of the fishing rod (create an empty GameObject here)
    public Transform bobber;         // The bobber object in the water

    void Start()
    {
        // Ensure the Line Renderer has two points (start and end)
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // Update the positions dynamically
        lineRenderer.SetPosition(0, rodTip.position); // Start of the line (rod tip)
        lineRenderer.SetPosition(1, bobber.position); // End of the line (bobber position)
    }
}
