using UnityEngine;

public class PitStopTester : MonoBehaviour
{
    // Riferimento al componente CarExitWaypoint
    private CarExitWaypoint carExitScript;
    // Riferimento al componente CarMovementWaypoint (per sapere quando l'auto si ferma)
    private CarMovementWaypoint carEntryScript;

    void Start()
    {
        // Trova la macchina (sostituisci "NomeDellaTuaMacchina" con il nome reale del GameObject)
        GameObject carObject = GameObject.Find("F1_moving");

        if (carObject != null)
        {
            carExitScript = carObject.GetComponent<CarExitWaypoint>();
            carEntryScript = carObject.GetComponent<CarMovementWaypoint>();
        }
        
        if (carExitScript == null || carEntryScript == null)
        {
            Debug.LogError("Car scripts not found on the Car GameObject! Cannot set up test trigger.");
            enabled = false;
        }
    }

    void Update()
    {
        // Controlla se l'auto è ferma
        if (carEntryScript != null && !carEntryScript.enabled)
        {
            // LOG AGGIUNTIVO: Questo messaggio appare solo quando la condizione è VERA
            Debug.Log("Car is stopped. Ready to accept P input.");

            // Trigger manuale: premi il tasto 'K' 
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("Manually triggered 'pitstop_finished'. Calling StartExitSequence.");
                carExitScript.StartExitSequence();
            }
        }
    }
}