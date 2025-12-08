using UnityEngine;
using System.Collections.Generic;

// Attach this script to the 'Old Tire' GameObject
public class OldTireMovementHandler : MonoBehaviour
{
    // *** NEW: Reference to the New Tire's movement script ***
    [Header("Inter-Script Communication")]
    [Tooltip("Drag the New Tire GameObject here. This script will directly call its movement method.")]
    public NewTireMovementHandler newTireHandler;

    // Public variables configurable in the Unity Inspector
    [Header("Tire Path Settings")]
    // ... (restano invariate) ...
    public List<Transform> waypoints = new List<Transform>();
    public float movementSpeed = 8f;
    public float stoppingDistance = 0.1f; 

    [Header("Testing and State")]
    public KeyCode activationKey = KeyCode.T;
    
    // Private variables
    private int currentWaypointIndex = 0;
    private bool isMoving = false; 
    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 

        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoints list for the old tire is empty! Please assign waypoints in the Inspector.");
        }
        
        enabled = false; 
    }

    void Update()
    {
        // ... (Update logic remains unchanged) ...
        if (Input.GetKeyDown(activationKey) && !isMoving)
        {
            Debug.Log($"--- TEST TRIGGER: Key '{activationKey}' pressed. Calling InitiateTireMovement().");
            InitiateTireMovement();
        }

        if (!isMoving || waypoints.Count == 0)
        {
            return;
        }

        if (currentWaypointIndex >= waypoints.Count)
        {
            FinalDestinationReached();
            return;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        Vector3 currentXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
        
        float distanceXZ = Vector3.Distance(currentXZ, targetXZ);

        if (distanceXZ < stoppingDistance)
        {
            currentWaypointIndex++;
        }
    }

    [ContextMenu("TRIGGER: Tyre Removed (Start Movement)")]
    public void InitiateTireMovement()
    {
        if (waypoints.Count == 0 || isMoving)
        {
            if (waypoints.Count == 0) Debug.LogError("Cannot initiate movement: Waypoints list is empty.");
            if (isMoving) Debug.LogWarning("Old Tire is already moving. Ignoring initiation call.");
            return; 
        }
        
        // Decouple the tire and handle physics
        transform.parent = null; 
        
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        transform.position = waypoints[0].position;

        Debug.Log("--- TRIGGER FIRED: tyre_removed --- Old Tire movement initiated.");
        
        currentWaypointIndex = 0; 
        isMoving = true;
        enabled = true; 
    }

    private void FinalDestinationReached()
    {
        isMoving = false; 
        enabled = false; 
        
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log("Old Tire successfully moved to the disposal point.");
        
        // *** NEW: CHIAMATA DIRETTA AL METODO DELL'ALTRO SCRIPT ***
        if (newTireHandler != null)
        {
            Debug.Log("--- CALLING METHOD: move_new_tire --- Activating new tire handler via direct call.");
            // Chiama il metodo pubblico sulla variabile 'newTireHandler'
            newTireHandler.StartNewTireMovement();
        }
    }
}