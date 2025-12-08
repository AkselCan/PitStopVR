using UnityEngine;
using System.Collections.Generic;

public class CarMovementWaypoint : MonoBehaviour
{
    // Public variables configurable in the Unity Inspector
    [Header("Path Settings")]
    public List<Transform> waypoints = new List<Transform>(); 
    public float fastMovementSpeed = 25f; 	// Speed for waypoints 0 to 6
    public float slowMovementSpeed = 5f; 	// Speed for waypoint 7 and onwards
    public float standardRotationSpeed = 5f; // Standard rotation speed
    public float stoppingDistance = 0.1f; 

    [Header("Custom Settings")]
    // Increased speed for rotation between waypoint 9 and 10 (when index is 9)
    public float aggressiveRotationSpeed = 30f; 

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
        float currentMovementSpeed;
        if (currentWaypointIndex < 7) 
        {
            currentMovementSpeed = fastMovementSpeed;
        }
        else 
        {
            currentMovementSpeed = slowMovementSpeed;
        }

        // --- Custom Rotation Speed Logic ---
        float currentRotationSpeed;
        // The car is moving *toward* waypoint 10 when the index is 9.
        if (currentWaypointIndex == 9) 
        {
            // Apply a much faster rotation speed to straighten the car before stopping.
            currentRotationSpeed = aggressiveRotationSpeed;
        }
        else
        {
            // Use the standard rotation speed for all other movements.
            currentRotationSpeed = standardRotationSpeed;
        }

        // 1. **Rotation:** Make the car look at the target waypoint
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0; // Ignore the Y-axis for rotation

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion correctedRotation = targetLookRotation * rotationOffset;
            // Use the dynamic 'currentRotationSpeed' here
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedRotation, currentRotationSpeed * Time.deltaTime);
        }

        // 2. **Movement:** Move the car towards the target waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMovementSpeed * Time.deltaTime);

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

        // For now, we use a simple Debug.Log to represent the "pitstop_started" trigger.
        Debug.Log("--- TRIGGER FIRED: pitstop_started --- Car is now stopped and ready for service.");
    }
}