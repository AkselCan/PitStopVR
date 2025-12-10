using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Required for using Coroutines

public class CarMovementWaypoint : MonoBehaviour
{
    // Public variables configurable in the Unity Inspector
    [Header("Path Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public float fastMovementSpeed = 25f;  // Speed for waypoints 0 to 6
    public float slowMovementSpeed = 5f;    // Speed for waypoint 7 and onwards
    public float standardRotationSpeed = 5f; // Standard rotation speed
    public float stoppingDistance = 0.1f;

    [Header("Custom Settings")]
    // Increased speed for rotation between waypoint 9 and 10 (when index is 9)
    public float aggressiveRotationSpeed = 30f;

    // Minimum delay before the car starts moving (in seconds)
    [Header("Initial Arrival Delay")]
    public float minInitialDelay = 10f;
    // Maximum delay before the car starts moving (in seconds)
    public float maxInitialDelay = 20f;

    // --- REFERENCE FOR THE TIMER MANAGER ---
    // Reference to the TimerManager script instance in the scene.
    private TimerManager timerManager;

    // Private variables
    private Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);
    private int currentWaypointIndex = 0;
    // Flag to ensure the pit stop started action is only called once
    private bool pitStopInitiated = false;

    void Start()
    {
        // 1. Find the TimerManager script instance in the scene.
        timerManager = FindObjectOfType<TimerManager>();

        if (timerManager == null)
        {
            Debug.LogError("TimerManager script not found in the scene! Cannot control the timer.");
        }
        else
        {
            // Ensure the timer is stopped and reset at the start of the scene.
            timerManager.StopTimer();
        }

        // Initial check to ensure waypoints have been assigned
        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoints list is empty! Please assign waypoints in the Inspector.");
            enabled = false;
            return;
        }

        // Set the car's height to the starting waypoint's height
        if (waypoints.Count > 0)
        {
            transform.position = new Vector3(transform.position.x, waypoints[0].position.y, transform.position.z);
        }

        // **IMPORTANT CHANGE:** Disable movement immediately and start the delay coroutine.
        enabled = false;
        StartCoroutine(InitialDelayCoroutine());
    }

    void Update()
    {
        // Check if the car has reached the final waypoint (index is out of bounds)
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
    /// Coroutine to handle the random initial delay before the car starts moving.
    /// Simulates the uncertainty of the car's arrival time.
    /// </summary>
    private IEnumerator InitialDelayCoroutine()
    {
        // 1. Calculate a random delay time between the defined min and max.
        // Use Random.Range which is inclusive for float arguments.
        float initialDelayDuration = Random.Range(minInitialDelay, maxInitialDelay);

        Debug.Log($"Car is waiting for an initial arrival delay of {initialDelayDuration:F2} seconds before starting its movement.");

        // 2. Wait for the calculated random duration.
        yield return new WaitForSeconds(initialDelayDuration);

        // 3. After the delay, enable the movement script to start the path.
        enabled = true;

        Debug.Log("Initial delay completed. Car movement initiated.");
    }

    /// <summary>
    /// Called when the car reaches the final waypoint. 
    /// This acts as the "pitstop_started" trigger and STARTS THE TIMER.
    /// </summary>
    public void StartPitStop()
    {
        pitStopInitiated = true; // Set flag to prevent repeated calls
        enabled = false; // Stop this movement script immediately

        // --- KEY ACTION: START THE TIMER ---
        if (timerManager != null)
        {
            // Call the public method on the TimerManager instance.
            timerManager.StartTimer();
        }

        Debug.Log("--- TRIGGER FIRED: pitstop_started --- Timer is now running!");
    }
}