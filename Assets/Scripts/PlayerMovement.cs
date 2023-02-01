using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The root object of the player model.")]
    [SerializeField] private GameObject model;

    [Header("Acceleration")]
    [Tooltip("The rate of acceleration in units/s^2.")]
    [SerializeField] private float acceleration;
    [Tooltip("The rate of deceleration in units/s^2 when braking.")]
    [SerializeField] private float brakeStrength;
    [Tooltip("The maximum achievable velocity.")]
    [SerializeField] private float maxVelocity;

    [Header("Turning")]
    [Tooltip("The maximum change of direction in degrees/s.")]
    [SerializeField] private float maxTurnAngle;
    [Tooltip("The maximum speed the player approaches the maxTurnAngle in degrees/s^2.")]
    [SerializeField] private float maxTurnChange;
    [Tooltip("The maximum angle the model will lean.")]
    [SerializeField] private float maxLeanAngle;

    // Input
    private PlayerInput input;  // The PlayerInput attached to this GameObject
    private float driveInput;   // The current input value for accelerating or braking
    private float turnInput;    // The current input value for turning
    private float driftInput;   // The current input value for drifting

    // Motion
    private CharacterController controller;     // The CharacterController that handles player movement
    private float velocity;                     // The current forward velocity
    private float turnAngle;                    // The current change in direction

    void Start()
    {
        // Assign our components
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    // We do our physics in FixedUpdate because Unity likes that
    void FixedUpdate() 
    {
        // Velocity is determined realistically by a competing acceleration and drag force. Since the maximum speed is the point at which 
        // drag and acceleration cancel each other out, we can calculate the drag as -acceleration * the percent of current velocity to max velocity

        // --------------------
        // --- ACCELERATION ---
        // --------------------

        // Determine the acceleration strength based on whether we're accelerating or braking
        float accel;                
        if(driveInput > 0)
            accel = acceleration;
        else
            accel = brakeStrength;

        // Apply acceleration
        velocity += driveInput * accel * Time.fixedDeltaTime;                // Accelerate or decelerate based on player input
        velocity -= (velocity / maxVelocity) * accel * Time.fixedDeltaTime;  // Decelerate according to drag
        if(velocity < acceleration * 0.5f * Time.fixedDeltaTime)             // If velocity would be negative or very small, set velocity to 0
            velocity = 0;
            
        // ---------------
        // --- TURNING ---
        // ---------------

        // Apply turning input
        turnAngle = Mathf.MoveTowards(turnAngle, turnInput * maxTurnAngle, maxTurnChange * Time.fixedDeltaTime);            // Apply the turn input to the turn angle
        transform.Rotate(0, turnAngle * Time.fixedDeltaTime, 0);                                                            // Rotate the player around the Y-axis
        model.transform.localRotation = Quaternion.AngleAxis((turnAngle/maxTurnAngle) * -maxLeanAngle, Vector3.forward);    // Lean the model around the Z-axis

        controller.Move(transform.forward * velocity * Time.fixedDeltaTime);        // Move the player
    } 

    // This gets called whenever there is a change in the input for the Drive action
    public void OnDrive(CallbackContext context)
    {
        driveInput = context.ReadValue<float>();
    }

    // This gets called whenever there is a change in the input for the Turn action
    public void OnTurn(CallbackContext context)
    {
        turnInput = context.ReadValue<float>();
    }

    // This gets called whenever there is a change in the input for the Drift action
    public void OnDrift(CallbackContext context)
    {
        driftInput = context.ReadValue<float>();
    }
}
