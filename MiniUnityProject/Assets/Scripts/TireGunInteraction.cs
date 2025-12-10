using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TireGunInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Point at the center of the wheel you're servicing.")]
    public Transform tireCenter;

    [Tooltip("Tip of the tire gun (child transform on the gun).")]
    public Transform gunTip;

    [Header("Interaction Settings")]
    [Tooltip("How close the gun tip must be to the tire center.")]
    public float activationRadius = 0.25f;

    [Tooltip("How long the trigger must be held (seconds).")]
    public float holdDuration = 1.0f;

    [Tooltip("Input System action for the trigger (e.g. XRI RightHand / Activate).")]
    public InputActionProperty triggerAction;

    [Header("Events")]
    [Tooltip("Fires the FIRST time you successfully hold: unscrew + tire swap.")]
    public UnityEvent onUnscrewAndSwap;

    [Tooltip("Fires the SECOND time you successfully hold: screw on + car exit.")]
    public UnityEvent onScrewOnAndDrive;

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
        if (!IsGunInRange())
        {
            holdTimer = 0f;
            return;
        }

        float triggerValue = triggerAction.action != null
            ? triggerAction.action.ReadValue<float>()
            : 0f;

        bool triggerPressed = triggerValue > 0.8f;

        if (triggerPressed)
        {
            if (waitingForRelease)
            {
                // Already fired a stage; must release once before next
                holdTimer = 0f;
                return;
            }

            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                holdTimer = 0f;
                FireCurrentStage();
                waitingForRelease = true;   // forces a let-go between stages
            }
        }
        else
        {
            holdTimer = 0f;
            waitingForRelease = false;
        }
    }

    private bool IsGunInRange()
    {
        if (gunTip == null || tireCenter == null) return false;
        return Vector3.Distance(gunTip.position, tireCenter.position) <= activationRadius;
    }

    private void FireCurrentStage()
    {
        if (!firstStageDone)
        {
            // Stage 1: unscrew + swap tires
            onUnscrewAndSwap?.Invoke();
            firstStageDone = true;
            Debug.Log("TireGunInteraction: Stage 1 (unscrew + swap) fired.");
        }
        else
        {
            // Stage 2: screw on + car drive away
            onScrewOnAndDrive?.Invoke();
            Debug.Log("TireGunInteraction: Stage 2 (screw on + drive away) fired.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (tireCenter == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tireCenter.position, activationRadius);
    }
}

