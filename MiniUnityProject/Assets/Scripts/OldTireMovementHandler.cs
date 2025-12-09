using UnityEngine;
using System.Collections.Generic;

// Attach this script to the 'Old Tire' GameObject
public class OldTireMovementHandler : MonoBehaviour
{
    [Header("Inter-Script Communication")]
    [Tooltip("Drag the New Tire GameObject (with NewTireMovementHandler) here.")]
    public NewTireMovementHandler newTireHandler;

    [Header("Old Tire Path Settings")]
    [Tooltip("Path from the car's wheel position to the disposal area.")]
    public List<Transform> waypoints = new List<Transform>();
    public float movementSpeed = 8f;
    public float stoppingDistance = 0.1f;

    [Header("Rotation Settings")]
    [Tooltip("How fast the old tire rotates to match the waypoint orientation (degrees per second).")]
    public float rotationSpeed = 720f;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (waypoints.Count == 0)
        {
            Debug.LogError("OldTireMovementHandler: Waypoints list is empty! Please assign waypoints in the Inspector.");
        }

        // Match NewTireMovementHandler: script starts disabled and waits for explicit trigger
        enabled = false;
    }

    void Update()
    {
        if (!isMoving || waypoints.Count == 0)
        {
            return;
        }

        if (currentWaypointIndex >= waypoints.Count)
        {
            FinalDestinationReached();
            return;
        }

        // ----- 1. Target position & rotation for this waypoint -----
        Transform target = waypoints[currentWaypointIndex];
        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        // ----- 2. Move toward position -----
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        // ----- 3. Rotate toward waypoint rotation (like the new tire) -----
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // ----- 4. Waypoint check (XZ plane, same style as new tire script) -----
        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetXZ = new Vector3(targetPosition.x, 0f, targetPosition.z);
        float distanceXZ = Vector3.Distance(currentXZ, targetXZ);

        if (distanceXZ < stoppingDistance)
        {
            currentWaypointIndex++;
        }
    }

    [ContextMenu("TRIGGER: Tyre Removed (Start Movement)")]
    public void InitiateTireMovement()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError("OldTireMovementHandler: Cannot initiate movement, waypoints list is empty.");
            return;
        }

        if (isMoving)
        {
            Debug.LogWarning("OldTireMovementHandler: Old tire is already moving. Ignoring initiation call.");
            return;
        }

        // ----- Decouple from the car (so it won't move with it) -----
        transform.SetParent(null);

        // Match new tire: kinematic during scripted movement
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // IMPORTANT: Do NOT teleport to waypoints[0]; start from the hub position
        currentWaypointIndex = 0;
        isMoving = true;
        enabled = true;

        Debug.Log("OldTireMovementHandler: Movement initiated.");
    }

    private void FinalDestinationReached()
    {
        isMoving = false;
        enabled = false;

        // Snap EXACTLY to the final waypoint's position + rotation
        Transform last = waypoints[waypoints.Count - 1];
        transform.SetPositionAndRotation(last.position, last.rotation);

        if (rb != null)
        {
            // Match the “attached / fixed” behavior style: keep it frozen at disposal point
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("OldTireMovementHandler: Old tire reached disposal point.");

        // Trigger the new tire, just like before
        if (newTireHandler != null)
        {
            Debug.Log("OldTireMovementHandler: Triggering new tire movement.");
            newTireHandler.StartNewTireMovement();
        }
        else
        {
            Debug.LogWarning("OldTireMovementHandler: newTireHandler reference not assigned in the Inspector.");
        }
    }
}
