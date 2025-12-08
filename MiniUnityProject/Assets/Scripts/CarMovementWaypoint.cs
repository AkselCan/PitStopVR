using UnityEngine;
using System.Collections.Generic;

public class CarMovementWaypoint : MonoBehaviour
{
    // Public variables configurable in the Unity Inspector
    [Header("Path Settings")]
    public List<Transform> waypoints = new List<Transform>(); 
    public float fastMovementSpeed = 25f;    // Speed for waypoints 0 to 6
    public float slowMovementSpeed = 5f;     // NEW: Speed for waypoint 7 and onwards
    public float rotationSpeed = 5f; 
    public float stoppingDistance = 0.1f; 

    // Private variable for the model orientation correction
    private Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);

    private int currentWaypointIndex = 0;

    void Start()
    {
        // Initial check to ensure waypoints have been assigned
        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoints list is empty! Please assign waypoints in the Inspector.");
            enabled = false; 
        }
        
        // Set the car's height to the starting waypoint's height
        transform.position = new Vector3(transform.position.x, waypoints[0].position.y, transform.position.z);
    }

    void Update()
    {
        // Check if we have processed all waypoints
        if (currentWaypointIndex >= waypoints.Count)
        {
            Debug.Log("Car has reached the final waypoint.");
            enabled = false; // Stop the movement script
            return;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // --- NEW LOGIC: Speed Control ---
        // Waypoint indices start from 0. To start slowing down at waypoint 7, 
        // we check if the index is 6 or less (for fast speed), or 7 and greater (for slow speed).
        float currentSpeed;
        
        // If the current index is less than 7 (meaning waypoints 0 through 6)
        if (currentWaypointIndex < 7) 
        {
            currentSpeed = fastMovementSpeed;
        }
        else // If the current index is 7 or greater (meaning waypoint 7, 8, 9, etc.)
        {
            currentSpeed = slowMovementSpeed;
        }
        // --- END NEW LOGIC ---

        // 1. **Rotation:** Make the car look at the target waypoint
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0; // Ignore the Y-axis for rotation

        if (directionToTarget != Vector3.zero)
        {
            // Calculate the required rotation to look at the waypoint
            Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);
            
            // Apply the 180-degree offset to correct the model orientation
            Quaternion correctedRotation = targetLookRotation * rotationOffset;
            
            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedRotation, rotationSpeed * Time.deltaTime);
        }

        // 2. **Movement:** Move the car towards the target waypoint
        // NOTE: We now use the 'currentSpeed' variable for movement
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // 3. **Waypoint Check:** Check if the car has reached the current waypoint
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            // Move to the next waypoint in the list
            currentWaypointIndex++;
        }
    }
}