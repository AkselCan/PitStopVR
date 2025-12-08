using UnityEngine;
using System.Collections.Generic;

// Attach this script to the 'New Tire' GameObject
public class NewTireMovementHandler : MonoBehaviour
{
    [Header("New Tire Path Settings")]
    [Tooltip("Path from tire rack to the car's wheel position.")]
    public List<Transform> waypointsToCar = new List<Transform>();
    public float movementSpeed = 8f;
    public float stoppingDistance = 0.1f;

    [Header("Car Position Reference")]
    [Tooltip("The final target position (the car's wheel hub).")]
    public Transform finalCarPosition; 

    private int currentWaypointIndex = 0;
    private bool hasReachedCar = false; 
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (waypointsToCar.Count == 0 || finalCarPosition == null)
        {
            Debug.LogError("New Tire Handler: Waypoints or Final Car Position missing!");
        }
        // The script starts disabled and waits for the trigger
        enabled = false; 
    }

    void Update()
    {
        if (hasReachedCar || waypointsToCar.Count == 0)
        {
            return;
        }

        Vector3 targetPosition;
        if (currentWaypointIndex < waypointsToCar.Count)
        {
            targetPosition = waypointsToCar[currentWaypointIndex].position;
        }
        else
        {
            // Final movement to the car's hub
            targetPosition = finalCarPosition.position;
        }

        // 1. Movement
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // 2. Waypoint Check (XZ Plane)
        Vector3 currentXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
        float distanceXZ = Vector3.Distance(currentXZ, targetXZ);

        if (distanceXZ < stoppingDistance)
        {
            if (currentWaypointIndex < waypointsToCar.Count)
            {
                currentWaypointIndex++;
            }
            else
            {
                NewTireReachedCar();
            }
        }
    }

    /// <summary>
    /// This public method is called directly by the OldTireMovementHandler when the old tire is disposed of.
    /// </summary>
    [ContextMenu("TEST: Start New Tire Movement")]
    public void StartNewTireMovement()
    {
        Debug.Log("New Tire Handler: Starting movement via direct call.");
        
        // Ensure Rigidbody is kinematic for transform movement
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        currentWaypointIndex = 0;
        hasReachedCar = false;
        enabled = true; 
    }

    private void NewTireReachedCar()
    {
        hasReachedCar = true;
        enabled = false;
        
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        Debug.Log("New Tire successfully attached to the car wheel position.");

        // Here you would fire the next trigger for the Pit Stop Manager
    }
}