using UnityEngine;
using System.Collections.Generic;

public class CarExitWaypoint : MonoBehaviour
{
    [Header("Exit Path Settings")]
    public List<Transform> exitWaypoints = new List<Transform>();
    public float exitSpeed = 20f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.1f;

    [Header("Exit Audio Settings")]
    public AudioClip exitAudioClip;   // new audio to play once when exit begins

    private AudioSource audioSource;
    private TimerManager timerManager;

    private Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);
    private int currentWaypointIndex = 0;

    void Start()
    {
        // Get the AudioSource on the car
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on the car!");
        }

        // Find TimerManager in scene
        timerManager = FindObjectOfType<TimerManager>();
        if (timerManager == null)
        {
            Debug.LogError("TimerManager script not found in the scene! Cannot control the timer.");
        }

        // Disable movement until pitstop is finished
        enabled = false;

        if (exitWaypoints.Count == 0)
        {
            Debug.LogError("Exit Waypoints list is empty! Please assign waypoints in the Inspector.");
        }
    }

    void Update()
    {
        if (currentWaypointIndex >= exitWaypoints.Count)
        {
            Debug.Log("Car has successfully exited the pit lane and is back on track.");
            enabled = false;
            return;
        }

        Vector3 targetPosition = exitWaypoints[currentWaypointIndex].position;

        // Rotation
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion correctedRotation = targetLookRotation * rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedRotation, rotationSpeed * Time.deltaTime);
        }

        // Movement
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, exitSpeed * Time.deltaTime);

        // Waypoint Check
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            currentWaypointIndex++;
        }
    }

    /// <summary>
    /// Triggered externally when the pit stop service is completed.
    /// Stops timer, changes audio, and starts exit movement.
    /// </summary>
    [ContextMenu("TRIGGER: Pit Stop Finished")]
    public void StartExitSequence()
    {
        // --- PLAY EXIT AUDIO ---
        if (audioSource != null && exitAudioClip != null)
        {
            audioSource.loop = false;             // play once
            audioSource.clip = exitAudioClip;     // assign new clip
            audioSource.Play();                   // play immediately
        }

        // --- STOP THE TIMER ---
        if (timerManager != null)
        {
            timerManager.StopTimer();
        }

        // Position at first exit point
        if (exitWaypoints.Count > 0)
        {
            transform.position = exitWaypoints[0].position;
        }

        currentWaypointIndex = 0;

        // Enable movement
        enabled = true;

        Debug.Log("--- LISTENER ACTIVATED: pitstop_finished --- Car is leaving the pit stop, timer stopped!");
    }
}
