using UnityEngine;
using System.Collections.Generic;

public class CarExitWaypoint : MonoBehaviour
{
    [Header("Exit Path Settings")]
    public List<Transform> exitWaypoints = new List<Transform>(); 
    public float exitSpeed = 20f;       
    public float rotationSpeed = 5f; 
    public float stoppingDistance = 0.1f; 

    private Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);
    private int currentWaypointIndex = 0;

    void Start()
    {
        // IMPORTANT: By default, disable the script until the pit stop is finished.
        enabled = false; 

        if (exitWaypoints.Count == 0)
        {
            Debug.LogError("Exit Waypoints list is empty! Please assign waypoints in the Inspector.");
        }
    }

    void Update()
    {
        // Check if the exit path is completed
        if (currentWaypointIndex >= exitWaypoints.Count)
        {
            Debug.Log("Car has successfully exited the pit lane and is back on track.");
            enabled = false; 
            return;
        }

        Vector3 targetPosition = exitWaypoints[currentWaypointIndex].position;

        // 1. Rotation 
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0; 

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion correctedRotation = targetLookRotation * rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedRotation, rotationSpeed * Time.deltaTime);
        }

        // 2. Movement 
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, exitSpeed * Time.deltaTime);

        // 3. Waypoint Check
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            currentWaypointIndex++;
        }
    }

    /// <summary>
    /// This method is the "pitstop_finished" trigger listener.
    /// It must be called externally (e.g., from PitStopTester or PitStopManager) when the service is complete.
    /// </summary>
    [ContextMenu("TRIGGER: Pit Stop Finished")]
    public void StartExitSequence()
    {
        // Ensure the car is exactly at the start position of the exit path (last entry waypoint)
        if (exitWaypoints.Count > 0)
        {
            // Nota: se i waypoint di ingresso e uscita condividono la posizione finale/iniziale,
            // questo assicura che l'auto sia nel punto esatto.
            transform.position = exitWaypoints[0].position;
        }

        // Reset index to start from the first exit waypoint
        currentWaypointIndex = 0; 
        
        // Enable the script to start the Update loop and movement
        enabled = true; 
        Debug.Log("--- LISTENER ACTIVATED: pitstop_finished --- Car is leaving the pit stop!");
    }
}