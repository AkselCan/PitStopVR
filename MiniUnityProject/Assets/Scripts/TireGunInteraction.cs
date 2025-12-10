using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TireGunInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform tireCenter;
    public Transform gunTip;

    [Header("Interaction Settings")]
    public float activationRadius = 0.25f;
    public float holdDuration = 1.0f;
    public InputActionProperty triggerAction;

    [Header("Events")]
    public UnityEvent onUnscrewAndSwap;
    public UnityEvent onScrewOnAndDrive;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private float debugDistanceToTire;
    [SerializeField] private float debugTriggerValue;
    [SerializeField] private float debugHoldTimer;

    private float holdTimer = 0f;
    private bool firstStageDone = false;
    private bool waitingForRelease = false;

    private void OnEnable()
    {
        if (triggerAction != null && triggerAction.action != null)
            triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        if (triggerAction != null && triggerAction.action != null)
            triggerAction.action.Disable();
    }

    private void Update()
    {
        bool inRange = IsGunInRange();

        if (!inRange)
        {
            if (holdTimer > 0f && debugLogs)
                Debug.Log("TireGunInteraction: Left range, resetting hold timer.");

            holdTimer = 0f;
            debugHoldTimer = holdTimer;
            return;
        }

        float triggerValue = triggerAction.action != null
            ? triggerAction.action.ReadValue<float>()
            : 0f;

        debugTriggerValue = triggerValue;

        bool triggerPressed = triggerValue > 0.8f;

        if (debugLogs)
        {
            Debug.Log($"TireGunInteraction: In range. Trigger value={triggerValue}, pressed={triggerPressed}");
        }

        if (triggerPressed)
        {
            if (waitingForRelease)
            {
                holdTimer = 0f;
                debugHoldTimer = holdTimer;
                return;
            }

            holdTimer += Time.deltaTime;
            debugHoldTimer = holdTimer;

            if (holdTimer >= holdDuration)
            {
                holdTimer = 0f;
                debugHoldTimer = holdTimer;
                FireCurrentStage();
                waitingForRelease = true;
            }
        }
        else
        {
            if (holdTimer > 0f && debugLogs)
                Debug.Log("TireGunInteraction: Trigger released, resetting hold timer.");

            holdTimer = 0f;
            debugHoldTimer = holdTimer;
            waitingForRelease = false;
        }
    }

    private bool IsGunInRange()
    {
        if (gunTip == null || tireCenter == null)
        {
            debugDistanceToTire = -1f;
            return false;
        }

        float dist = Vector3.Distance(gunTip.position, tireCenter.position);
        debugDistanceToTire = dist;

        bool inRange = dist <= activationRadius;

        if (debugLogs)
        {
            Debug.Log($"TireGunInteraction: Distance={dist}, activationRadius={activationRadius}, inRange={inRange}");
        }

        return inRange;
    }

    private void FireCurrentStage()
    {
        if (!firstStageDone)
        {
            if (debugLogs)
                Debug.Log("TireGunInteraction: Stage 1 (unscrew + swap) fired.");

            onUnscrewAndSwap?.Invoke();
            firstStageDone = true;
        }
        else
        {
            if (debugLogs)
                Debug.Log("TireGunInteraction: Stage 2 (screw on + drive away) fired.");

            onScrewOnAndDrive?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (tireCenter == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tireCenter.position, activationRadius);
    }
}
