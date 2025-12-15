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
    [Tooltip("How close the gun tip must be to the tire center.")]
    public float activationRadius = 0.25f;

    [Tooltip("How long the trigger must be held (seconds).")]
    public float holdDuration = 1.0f;

    [Tooltip("Input System action for the trigger (e.g. XRI RightHand / Activate).")]
    public InputActionProperty triggerAction;

    [Header("Events")]
    [Tooltip("Stage 1: old tire off + new tire on.")]
    public UnityEvent onUnscrewAndSwap;

    [Tooltip("Stage 2: screw on + car drive away.")]
    public UnityEvent onScrewOnAndDrive;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;
    [SerializeField] private float debugDistanceToTire;
    [SerializeField] private float debugTriggerValue;
    [SerializeField] private float debugHoldTimer;
    [SerializeField] private bool debugInRange;
    [SerializeField] private bool debugHoldSatisfied;

    private float holdTimer = 0f;
    private bool holdSatisfiedForStage = false;    // Did we hold long enough this visit?
    private bool firstStageDone = false;           // Has Stage 1 fired?
    private bool sequenceCompleted = false;        // Has Stage 2 fired?
    private bool wasInRangeLastFrame = false;
    private bool wasTriggerPressedLastFrame = false;

    // Between stages: require a release before Stage 2 can arm
    private bool hasReleasedSinceLastStage = true; // true initially so Stage 1 isn't gated

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
        debugInRange = inRange;

        float triggerValue = triggerAction.action != null
            ? triggerAction.action.ReadValue<float>()
            : 0f;

        debugTriggerValue = triggerValue;
        bool triggerPressed = triggerValue > 0.8f;

        // Are we allowed to arm a stage right now?
        // - Stage 1: allowed as long as sequence not completed.
        // - Stage 2: only allowed if Stage 1 done AND user has released since then.
        bool stageArmAllowed =
            !sequenceCompleted &&
            (!firstStageDone || (firstStageDone && hasReleasedSinceLastStage));

        // While in range, holding trigger, and allowed to arm, accumulate hold time
        if (inRange && triggerPressed && stageArmAllowed)
        {
            holdTimer += Time.deltaTime;
            debugHoldTimer = holdTimer;

            if (!holdSatisfiedForStage && holdTimer >= holdDuration)
            {
                holdSatisfiedForStage = true;
                debugHoldSatisfied = true;

                if (debugLogs)
                    Debug.Log("TireGunInteraction: Hold satisfied for current stage.");
            }
        }

        // Detect leaving the zone (inRange -> !inRange)
        if (!inRange && wasInRangeLastFrame)
        {
            if (holdSatisfiedForStage && stageArmAllowed)
            {
                // Valid hold achieved in range, and now backing out ? fire current stage
                if (debugLogs)
                    Debug.Log("TireGunInteraction: Exited zone after valid hold, firing stage.");
                FireCurrentStage();
            }

            // Reset per-visit state
            holdTimer = 0f;
            holdSatisfiedForStage = false;
            debugHoldTimer = holdTimer;
            debugHoldSatisfied = false;
        }

        // If in range and trigger released before holdDuration, cancel the charge
        if (inRange && !triggerPressed && !holdSatisfiedForStage)
        {
            holdTimer = 0f;
            debugHoldTimer = holdTimer;
        }
        // If holdSatisfiedForStage is true, we keep it armed until we leave the zone,
        // whether or not the trigger is still held.

        // Detect a release edge between stages (to allow Stage 2)
        if (!triggerPressed && wasTriggerPressedLastFrame)
        {
            if (firstStageDone && !sequenceCompleted)
            {
                hasReleasedSinceLastStage = true;
                if (debugLogs)
                    Debug.Log("TireGunInteraction: Release detected between stages; Stage 2 can arm now.");
            }
        }

        wasInRangeLastFrame = inRange;
        wasTriggerPressedLastFrame = triggerPressed;
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
            // Stage 1: unscrew + swap
            if (debugLogs)
                Debug.Log("TireGunInteraction: Stage 1 (unscrew + swap) fired.");

            onUnscrewAndSwap?.Invoke();
            firstStageDone = true;

            // After Stage 1, require a release before Stage 2 can arm
            hasReleasedSinceLastStage = false;
        }
        else if (!sequenceCompleted)
        {
            // Stage 2: screw on + drive away
            if (debugLogs)
                Debug.Log("TireGunInteraction: Stage 2 (screw on + drive) fired.");

            onScrewOnAndDrive?.Invoke();
            sequenceCompleted = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (tireCenter == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tireCenter.position, activationRadius);
    }
}
