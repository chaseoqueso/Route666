using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    public enum DriftState
    {
        None = 0,
        Left = -1,
        Right = 1,
    }

    [Header("References")]
    [Tooltip("The root object of the player model.")]
    [SerializeField] private Transform model;
    [Tooltip("The head bone of the player.")]
    [SerializeField] private Transform head;

    [Header("Camera")]
    [Tooltip("The maximum horizontal viewing angles.")]
    [SerializeField] private Vector2 maxXLook;
    [Tooltip("The maximum vertical viewing angles.")]
    [SerializeField] private Vector2 maxYLook;
    [Tooltip("The camera sensitivity.")]
    public float lookSensitivity = 1;

    [Header("Controller")]
    [Tooltip("The highest slope the player can climb in degrees.")]
    [SerializeField] private float slopeLimit;

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
    [Tooltip("The speed at which the player approaches the maxTurnAngle in degrees/s^2.")]
    [SerializeField] private float maxTurnChange;
    [Tooltip("The maximum angle the model will lean.")]
    [SerializeField] private float maxLeanAngle;

    [Header("Drifting")]
    [Tooltip("The angle that the player will drift toward when drifting without pressing turning inputs.")]
    [SerializeField] private float targetDriftAngle;
    [Tooltip("The range above and below targetDriftAngle that turning inputs will range within while drifting.")]
    [SerializeField] private float driftAngleRange;
    [Tooltip("The angle to skew the model while drifting.")]
    [SerializeField] private float driftModelSkewAngle;
    [Tooltip("The range above and below the skew angle to skew the model while drifting.")]
    [SerializeField] private float driftModelSkewRange;
    [Tooltip("The speed at which the player model approaches the drift angle in degrees/s^2.")]
    [SerializeField] private float maxSkewChange;

    [Header("Gravity")]
    [Tooltip("The acceleration due to gravity.")]
    [SerializeField] private float gravityAccel;
    [Tooltip("The distance to check below the player for ground.")]
    [SerializeField] private float groundCheckDistance;

    // Input
    private PlayerInput input;  // The PlayerInput attached to this GameObject
    private Vector2 lookInput;  // The current look input
    private float driveInput;   // The current input value for accelerating or braking
    private float turnInput;    // The current input value for turning

    // Motion
    private Vector3 previousPosition;           // The position of the player last frame
    private Rigidbody rb;                       // The rigidbody that handles player movement
    private CapsuleCollider coll;               // The player collider
    private DriftState driftState;              // Whether the player is in a drift or not, and if so which direction
    private float velocity;                     // The current forward velocity
    private float verticalVelocity;             // The current vertical velocity
    private float turnAngle;                    // The current change in direction
    private float driftSkewAngle;               // The current drifting skew angle for the model
    private bool isGrounded;                    // Whether the player is on the ground or not

    // Camera
    private Quaternion headStartAngle;  // The head bone's starting angle
    private Vector2 lookAngle;          // The current look angle

    void Awake()
    {
        // Assign our components
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();

        // Get the initial head rotation
        headStartAngle = head.rotation;
    }

    // We do camera stuff in LateUpdate because that's after input is read
    private void LateUpdate() 
    {
        lookAngle += lookInput * lookSensitivity;  // Apply the look delta

        // Clamp the angle to the appropriate range
        lookAngle.x = Mathf.Clamp(lookAngle.x, maxXLook.x + driftSkewAngle, maxXLook.y + driftSkewAngle);
        lookAngle.y = Mathf.Clamp(lookAngle.y, maxYLook.x, maxYLook.y);

        head.rotation = Quaternion.LookRotation(transform.forward, model.up) *
                        Quaternion.Euler(-lookAngle.y, lookAngle.x, 0) * 
                        headStartAngle; // Apply the look rotation
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

        // Calculate the min and max angle the player can turn
        Vector2 angleRange;
        if(driftState == DriftState.None)   // If we aren't currently drifting
        {
            angleRange = new Vector2(-maxTurnAngle, maxTurnAngle);      // The angle range is the full turn range
        }
        else    // If we are drifting
        {
            float driftAngle = targetDriftAngle * (int)driftState;                                  // Calculate the center of the angle range
            angleRange = new Vector2(driftAngle - driftAngleRange, driftAngle + driftAngleRange);   // The angle range is a narrow range beyond the max turn angle
        }

        // Calculate ideal angle
        float normalizedTurnInput = Mathf.InverseLerp(-1, 1, turnInput);                            // Get the turnInput from a scale of 0 to 1
        float idealTurnAngle = Mathf.SmoothStep(angleRange.x, angleRange.y, normalizedTurnInput);   // Get the desired turn angle based on the current angle range

        // Turn towards ideal angle
        turnAngle = Mathf.MoveTowards(turnAngle, idealTurnAngle, maxTurnChange * Time.fixedDeltaTime);      // Apply the turn input to the turn angle
            
        // --------------------
        // --- GROUND CHECK ---
        // --------------------

        RaycastHit hit; 
        Vector3 capsuleCenter = transform.position + coll.center + Vector3.up * groundCheckDistance;    // The center point of the capsule
        Vector3 capsuleHalfHeight = Vector3.up * (coll.height/2 - coll.radius);                         // The vector from the center to one of the capsule points
        if(Physics.CapsuleCast(capsuleCenter - capsuleHalfHeight,
                               capsuleCenter + capsuleHalfHeight,
                               coll.radius,
                               Vector3.down,
                               out hit,
                               groundCheckDistance * 2,
                               LayerMask.GetMask("Environment"))
           && Vector3.Angle(hit.normal, Vector3.up) <= slopeLimit)   // If we're on the ground
        {
            isGrounded = true;          // Set grounded to true
            verticalVelocity = 0;       // Set our vertical velocity to 0
            rb.MovePosition(Vector3.down * hit.distance);
        }
        else    // If we're not on the ground
        {
            if(isGrounded)  // If we just left the ground this frame
            {
                verticalVelocity = (transform.position - previousPosition).y / Time.fixedDeltaTime;    // Set our velocity to the actual current velocity
                isGrounded = false;                                                                    // Set grounded to false
            }

            verticalVelocity -= gravityAccel * Time.fixedDeltaTime;  // Apply gravity
        }

        // ----------------------
        // --- PERFORM MOTION ---
        // ----------------------
                            
        rb.MoveRotation(Quaternion.Euler(0, turnAngle * Time.fixedDeltaTime, 0) * transform.rotation);  // Rotate the player around the Y-axis

        previousPosition = transform.position;                                                          // Store the previous position
        Vector3 moveVector = (transform.forward * velocity) + (Vector3.up * verticalVelocity);          // Calculate total velocity
        rb.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);                                          // Apply movement
            
        // ----------------------
        // --- MODEL ROTATION ---
        // ----------------------

        // Lean the model around the Z-axis
        float leanAngle = Mathf.Clamp(turnAngle/maxTurnAngle, -1, 1) * -maxLeanAngle;       // Calculate the angle to lean the player model
        Quaternion leanRotation = Quaternion.AngleAxis(leanAngle, Vector3.forward);         // Set the lean rotation

        // Skew the model if drifting
        float idealSkewAngle;
        if(driftState != DriftState.None)   // If drifting
        {
            float skewAngle = driftModelSkewAngle * (int)driftState;        // The middle of the skew angle range
            Vector2 skewRange = new Vector2(skewAngle - driftModelSkewRange, skewAngle + driftModelSkewRange);                      // The angle range to skew the model
            idealSkewAngle = Mathf.SmoothStep(skewRange.x, skewRange.y, Mathf.InverseLerp(angleRange.x, angleRange.y, turnAngle));  // Get the desired skew angle based on the skew angle range
        }
        else    // If not drifting
        {
            idealSkewAngle = 0;
        }
        float previousSkewAngle = driftSkewAngle;   // Record the previous angle
        driftSkewAngle = Mathf.MoveTowards(driftSkewAngle, idealSkewAngle, maxSkewChange * Time.fixedDeltaTime);    // Move the drift angle toward the ideal
        Quaternion skewRotation = Quaternion.AngleAxis(driftSkewAngle, Vector3.up);                                 // Set the skew rotation

        // Adjust the camera
        lookAngle.x += (driftSkewAngle - previousSkewAngle) / 2; // Add a portion of the skew angle to the look angle

        // Set the model rotation
        model.localRotation = skewRotation * leanRotation;
    } 

    // This gets called whenever there is a change in the input for the Drive action
    public void OnLook(CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // This gets called whenever there is a change in the input for the Drive action
    public void OnDrive(CallbackContext context)
    {
        driveInput = Mathf.Sign(context.ReadValue<float>());
    }

    // This gets called whenever there is a change in the input for the Turn action
    public void OnTurn(CallbackContext context)
    {
        turnInput = context.ReadValue<float>();
    }

    // This gets called whenever there is a change in the input for the Drift action
    public void OnDrift(CallbackContext context)
    {
        // ----------------
        // --- DRIFTING ---
        // ----------------

        float driftInput = context.ReadValue<float>();  // Read the input value
        if(driftState == DriftState.None)               // If not currently drifting
        {
            if(driftInput > 0 && Mathf.Abs(turnInput) > 0)  // If drift is pressed and a turn input is pressed
            {
                driftState = (DriftState)Mathf.Sign(turnInput);
            }
        }
        else    // If currently drifting
        {
            if(driftInput == 0)     // If drift input is released, stop drifting
            {
                driftState = DriftState.None;
            }
        }
    }

    public void OnPause(CallbackContext context)
    {
        if(PauseMenu.gameIsPaused){
            GameManager.instance.UIManager.pauseMenu.ResumeGame();
        }
        else{
            GameManager.instance.UIManager.pauseMenu.PauseGame();
        }        
    }
    
    void OnCollisionEnter(Collision other) 
    {
        Debug.Log(other.contacts[0].point);
    }
}
