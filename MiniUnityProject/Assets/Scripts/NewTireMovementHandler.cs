using UnityEngine;
using System.Collections.Generic;

// Attach this script to the 'New Tire' GameObject
public class NewTireMovementHandler : MonoBehaviour
{
    [Header("New Tire Path Settings")]
    [Tooltip("Path from tire rack to the car's wheel position (last waypoint = wheel).")]
    public List<Transform> waypointsToCar = new List<Transform>();
    public float movementSpeed = 8f;
    public float stoppingDistance = 0.05f;   // a bit larger so we actually hit it

    [Header("Rotation Settings")]
    [Tooltip("How fast the tire rotates toward the target (degrees per second).")]
    public float rotationSpeed = 720f;

    [Header("Car Parenting")]
    [Tooltip("Object that moves with the car (e.g., F1_moving). The tire will be parented here.")]
    public Transform carParent;

    private int currentWaypointIndex = 0;
    private bool hasReachedCar = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (waypointsToCar.Count == 0)
        {
            Debug.LogError("NewTireMovementHandler: No waypoints assigned!");
        }

        enabled = false; // wait for trigger
    }

    void Update()
    {
        if (hasReachedCar || waypointsToCar.Count == 0)
            return;

        // ----- 1. Choose current target waypoint -----
        if (currentWaypointIndex >= waypointsToCar.Count)
        {
            // Safety clamp
            currentWaypointIndex = waypointsToCar.Count - 1;
        }

        Transform target = waypointsToCar[currentWaypointIndex];
        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        // ----- 2. Move toward position -----
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        // ----- 3. Rotate toward rotation -----
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // ----- 4. Waypoint check (XZ plane) -----
        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetXZ = new Vector3(targetPosition.x, 0f, targetPosition.z);
        float distanceXZ = Vector3.Distance(currentXZ, targetXZ);

        if (distanceXZ < stoppingDistance)
        {
            // If we're not at the last waypoint yet, move to the next one
            if (currentWaypointIndex < waypointsToCar.Count - 1)
            {
                currentWaypointIndex++;
            }
            else
            {
                // We reached the LAST waypoint (wheel position)
                Transform finalWaypoint = waypointsToCar[waypointsToCar.Count - 1];

                // Snap exactly to it
                transform.SetPositionAndRotation(
                    finalWaypoint.position,
                    finalWaypoint.rotation
                );

                NewTireReachedCar();
            }
        }
    }

    [ContextMenu("TEST: Start New Tire Movement")]
    public void StartNewTireMovement()
    {
        Debug.Log("NewTireMovementHandler: Starting movement via direct call.");

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Parent the tire so it moves with the car
        if (carParent != null)
        {
            transform.SetParent(carParent, true);  // keep world pose
            Debug.Log($"NewTireMovementHandler: Parent set to {carParent.name}");
        }
        else
        {
            Debug.LogWarning("NewTireMovementHandler: carParent not set, tire will NOT move with car.");
        }

        Debug.Log("NewTireMovementHandler: New tire attached and parented to car.");
    }
}
