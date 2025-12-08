using UnityEngine;
using System.Collections.Generic;

public class CarMovementWaypoint : MonoBehaviour
{
    // Public variables configurable in the Unity Inspector
    [Header("Path Settings")]
    public List<Transform> waypoints = new List<Transform>(); 
    public float fastMovementSpeed = 25f;    // Speed for waypoints 0 to 6
    public float slowMovementSpeed = 5f;     // Speed for waypoint 7 and onwards
    public float rotationSpeed = 5f; 
    public float stoppingDistance = 0.1f; 

    // Private variables
    private Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);
    private int currentWaypointIndex = 0;
    // Flag to ensure the pit stop started action is only called once
    private bool pitStopInitiated = false; 

    void Start()
    {
        // Initial check to ensure waypoints have been assigned
        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoints list is empty! Please assign waypoints in the Inspector.");
            enabled = false; 
        }
        
        // Set the car's height to the starting waypoint's height
        // This line assumes the first waypoint is near the car's starting X/Z position
        if (waypoints.Count > 0)
        {
            transform.position = new Vector3(transform.position.x, waypoints[0].position.y, transform.position.z);
        }
    }

    void Update()
    {
        // Check if the car has reached the final waypoint
        if (currentWaypointIndex >= waypoints.Count)
        {
            // The car is stopped. Execute the pit stop initiation only once.
            if (!pitStopInitiated)
            {
                StartPitStop();
            }
            return;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        // --- Speed Control Logic ---
        float currentSpeed;
        if (currentWaypointIndex < 7) 
        {
            currentSpeed = fastMovementSpeed;
        }
        else 
        {
            currentSpeed = slowMovementSpeed;
        }

        // 1. **Rotation:** Make the car look at the target waypoint
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0; // Ignore the Y-axis for rotation

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion correctedRotation = targetLookRotation * rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedRotation, rotationSpeed * Time.deltaTime);
        }

        // 2. **Movement:** Move the car towards the target waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // 3. **Waypoint Check:** Check if the car has reached the current waypoint
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            // Move to the next waypoint in the list
            currentWaypointIndex++;
        }
    }

    /// <summary>
    /// Called when the car reaches the final waypoint. This acts as the "pitstop_started" trigger.
    /// </summary>
    public void StartPitStop()
    {
        pitStopInitiated = true; // Set flag to prevent repeated calls
        enabled = false; // Stop this movement script immediately

        // In a real game, you would fire a C# Event here (e.g., PitStopManager.OnPitStopStarted.Invoke())
        // For now, we use a simple Debug.Log to represent the "pitstop_started" trigger.
        Debug.Log("--- TRIGGER FIRED: pitstop_started --- Car is now stopped and ready for service.");

        // NOTE: The next action (calling CarExitWaypoint.StartExitSequence) 
        // will be handled externally when 'pitstop_finished' is triggered.
    }
}