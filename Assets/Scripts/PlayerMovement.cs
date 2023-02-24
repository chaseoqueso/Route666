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
    [Tooltip("The root object of the motorcycle's steering mechanism.")]
    [SerializeField] private Transform steering;
    [Tooltip("The point to spawn bullets.")]
    [SerializeField] private Transform bulletSpawnPoint;
    [Tooltip("The muzzle flare object.")]
    [SerializeField] private GameObject muzzleFlare;
    [Tooltip("The animator of the player.")]
    [SerializeField] private Animator anim;
    [Tooltip("The IK script on the player's hand.")]
    [SerializeField] private DitzelGames.FastIK.FastIKFabric handIK;
    [Tooltip("The bullet prefab.")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shooting")]
    [Tooltip("The number of bullets per second.")]
    [SerializeField] private float fireRate;

    [Header("Camera")]
    [Tooltip("The maximum horizontal viewing angles.")]
    [SerializeField] private Vector2 maxXLook;
    [Tooltip("The maximum vertical viewing angles.")]
    [SerializeField] private Vector2 maxYLook;
    [Tooltip("The camera sensitivity.")]
    public float lookSensitivity = 1;

    [Header("Controller")]
    [Tooltip("The highest slope in degrees the player can climb.")]
    [SerializeField] private float slopeLimit;
    [Tooltip("The highest size step the player can climb.")]
    [SerializeField] private float stepHeight;
    [Tooltip("The sharpest angle in degrees the player can hit a wall before losing speed.")]
    [SerializeField] private float wallLimit;
    [Tooltip("The speed in degrees/s at which the model tracks the designated rotation.")]
    [SerializeField] private float modelTrackingSpeed;

    [Header("Acceleration")]
    [Tooltip("The rate of acceleration in units/s^2.")]
    [SerializeField] private float acceleration;
    [Tooltip("The rate of deceleration in units/s^2 when braking.")]
    [SerializeField] private float brakeStrength;
    [Tooltip("The maximum achievable velocity.")]
    [SerializeField] private float maxVelocity;
    [Tooltip("The velocity threshold beyond which is considered high velocity for gameplay purposes.")]
    [SerializeField] private float highVelocity;

    [Header("Turning")]
    [Tooltip("The maximum change of direction in degrees/s.")]
    [SerializeField] private float maxTurnAngle;
    [Tooltip("The speed at which the player approaches the maxTurnAngle in degrees/s^2.")]
    [SerializeField] private float maxTurnChange;
    [Tooltip("The maximum angle the model will lean.")]
    [SerializeField] private float maxLeanAngle;
    [Tooltip("The angle to rotate the handlebars.")]
    [SerializeField] private float steeringTurnAngle;
    [Tooltip("The speed to rotate the handlebars.")]
    [SerializeField] private float steeringTurnSpeed;

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
    private float driftInput;   // The current input value for accelerating or braking

    // Motion
    private RaycastHit groundHit;               // The collision data of the ground
    private RaycastHit previousGroundHit;       // The collision data of the ground last frame
    private bool groundHitIsValid;              // Whether the groundHit is valid
    private Vector3 pushVelocity;               // An external velocity vector that maintains player momentum while still pushing the player
    private Vector2 angleRange;                 // The range of angles the player can turn within
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

    // Shooting
    private bool canShoot;              // Whether the player can shoot

    void Awake()
    {
        // Assign our components
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();

        // Initiate animator
        if(anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
        anim.SetFloat("FireRate", fireRate);

        headStartAngle = head.rotation;     // Get the initial head rotation

        groundHitIsValid = false;           // We haven't checked the ground yet

        canShoot = true;                    // Enable shooting

        muzzleFlare.SetActive(false);       // Disable muzzle flare
    }

    // We do camera stuff in LateUpdate because that's after input is read
    private void LateUpdate() 
    { 
        // --------------
        // --- CAMERA ---
        // --------------

        lookAngle += lookInput * lookSensitivity;  // Apply the look delta

        // Clamp the angle to the appropriate range
        lookAngle.x = Mathf.Clamp(lookAngle.x, maxXLook.x, maxXLook.y);
        lookAngle.y = Mathf.Clamp(lookAngle.y, maxYLook.x, maxYLook.y);

        head.rotation = Quaternion.LookRotation(model.forward, model.up) *
                        Quaternion.Euler(-lookAngle.y, lookAngle.x, 0) * 
                        headStartAngle;                                         // Apply the look rotation
            
        // ----------------------
        // --- MODEL ROTATION ---
        // ----------------------

        // Lean the model around the Z-axis
        float leanAngle;
        if(isGrounded)  // If the player is on the ground
        {
            leanAngle = Mathf.Clamp(turnAngle/maxTurnAngle, -1, 1) * -maxLeanAngle;       // Calculate the angle to lean the player model
        }
        else            // If the player is in the air
        {
            leanAngle = 0;  // Don't lean
        }
        Quaternion leanRotation = Quaternion.AngleAxis(leanAngle, Vector3.forward);         // Set the lean rotation

        // Skew the model if drifting
        float idealSkewAngle;
        if(driftState != DriftState.None)   // If drifting
        {
            float skewAngle = driftModelSkewAngle * (int)driftState;        // The middle of the skew angle range
            Vector2 skewRange = new Vector2(skewAngle - driftModelSkewRange, skewAngle + driftModelSkewRange);      // The angle range to skew the model
            float skewAmount = Mathf.InverseLerp(angleRange.x, angleRange.y, turnAngle);                            // Get the skew amount from 0 to 1
            idealSkewAngle = Mathf.SmoothStep(skewRange.x, skewRange.y, skewAmount);                                // Get the desired skew angle based on the skew angle range
        
            // Rotate the handlebars opposite to the drift direction
            steering.localRotation = Quaternion.RotateTowards(steering.localRotation, Quaternion.Euler(0, -(int)driftState * steeringTurnAngle * 2f, 0), steeringTurnSpeed * Time.deltaTime);
        }
        else    // If not drifting
        {
            idealSkewAngle = 0;

            // Rotate the handlebars according to the turn direction
            steering.localRotation = Quaternion.RotateTowards(steering.localRotation, Quaternion.Euler(0, turnInput * steeringTurnAngle, 0), steeringTurnSpeed * Time.deltaTime);
        }
        float previousSkewAngle = driftSkewAngle;   // Record the previous angle
        driftSkewAngle = Mathf.MoveTowards(driftSkewAngle, idealSkewAngle, maxSkewChange * Time.deltaTime);         // Move the drift angle toward the ideal
        Quaternion skewRotation = Quaternion.AngleAxis(driftSkewAngle, Vector3.up);                                 // Set the skew rotation

        // Adjust the camera
        lookAngle.x -= (driftSkewAngle - previousSkewAngle) / 2; // Subtract a portion of the skew angle from the look angle to maintain look direction

        // Set the model rotation
        model.localRotation = Quaternion.RotateTowards(model.localRotation, skewRotation * leanRotation, modelTrackingSpeed * Time.deltaTime);
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

        // If we're on the ground
        if(isGrounded)
        {
            velocity += driveInput * accel * Time.fixedDeltaTime;                // Accelerate or decelerate based on player input
            velocity -= (velocity / maxVelocity) * accel * Time.fixedDeltaTime;  // Decelerate according to drag
        }

        if(velocity < acceleration * 0.5f * Time.fixedDeltaTime)             // If velocity would be negative or very small, set velocity to 0
            velocity = 0;
            
        // ---------------
        // --- TURNING ---
        // ---------------

        // Calculate the min and max angle the player can turn
        if(driftState == DriftState.None)   // If we aren't currently drifting
        {
            angleRange = new Vector2(-maxTurnAngle, maxTurnAngle);      // The angle range is the full turn range
        }
        else    // If we are drifting
        {
            float driftAngle = targetDriftAngle * (int)driftState;                                  // Calculate the center of the angle range
            angleRange = new Vector2(driftAngle - driftAngleRange, driftAngle + driftAngleRange);   // The angle range is a narrow range beyond the max turn angle
        }

        // Convert turn input to a normalized value
        float normalizedTurnInput;
        if(isGrounded)                                                  // If the player is on the ground
            normalizedTurnInput = Mathf.InverseLerp(-1, 1, turnInput);  // Get the turnInput from a scale of 0 to 1
        else                                                            // Otherwise
            normalizedTurnInput = 0.5f;                                 // Set the input to the middle
        
        float idealTurnAngle = Mathf.SmoothStep(angleRange.x, angleRange.y, normalizedTurnInput);   // Get the desired turn angle based on the current angle range

        // Turn towards ideal angle
        turnAngle = Mathf.MoveTowards(turnAngle, idealTurnAngle, maxTurnChange * Time.fixedDeltaTime);      // Apply the turn input to the turn angle
            
        // --------------------
        // --- GROUND CHECK ---
        // --------------------

        Vector3 capsulePoint = transform.position + Vector3.up * (coll.radius);   // The center point of the capsule's lower sphere
        Vector3 groundStickVector = Vector3.zero;
        
        Vector3 groundNormal = groundHitIsValid ? groundHit.normal : Vector3.up; 

        // CapsuleCast downward using the player collider
        RaycastHit sphereHit;
        float sphereDistance = (isGrounded) ? stepHeight : 0;
        bool didSphereHit = Physics.SphereCast(capsulePoint + groundNormal * groundCheckDistance, coll.radius, -groundNormal, out sphereHit, sphereDistance + groundCheckDistance*2, LayerMask.GetMask("Environment"));

        RaycastHit rayHit;
        float rayDistance = (isGrounded) ? stepHeight : sphereHit.distance;
        bool didRayHit = Physics.Raycast(capsulePoint + (-groundNormal * (coll.radius - groundCheckDistance)), -groundNormal, out rayHit, rayDistance + groundCheckDistance*2, LayerMask.GetMask("Environment"));
        
        if(didSphereHit    // If the spherecast hit
           && didRayHit    // And the center of the collider hit
           && (Vector3.Angle(rayHit.normal, Vector3.up) <= slopeLimit)      // And we're on a ground surface
           && (sphereHit.distance < groundCheckDistance || isGrounded))     // And the hit distance is valid
        {
            if(!isGrounded && verticalVelocity <= 0)     // If the player isn't grounded but is within ground distance
            {
                isGrounded = true;          // Set grounded to true
                verticalVelocity = 0;       // Set our vertical velocity to 0
                
                Debug.Log("Land");
            }

            if(isGrounded)
            {
                // Store the groundNormal
                groundHitIsValid = true;
                previousGroundHit = groundHit;
                groundHit = rayHit;

                groundStickVector = -groundHit.normal * sphereHit.distance + Vector3.up * groundCheckDistance/2;     // Stick the player to the ground
            }
        }
        else    // If we're not on the ground
        {
            if(isGrounded)  // If we just left the ground this frame
            {
                Debug.Log("Takeoff");
                Debug.Log(didSphereHit);    // If the spherecast hit
                Debug.Log(didRayHit);       // And the center of the collider hit
                Debug.Log(Vector3.Angle(sphereHit.normal, Vector3.up) <= slopeLimit || Vector3.Angle(rayHit.normal, Vector3.up) <= slopeLimit);     // And we're on a ground surface
                Debug.Log(sphereHit.distance < groundCheckDistance || isGrounded);                                                                  // And the hit distance is valid
                
                verticalVelocity = (groundHit.point - previousGroundHit.point).y / Time.fixedDeltaTime;     // Set our velocity to the actual current velocity
                isGrounded = false;                                                                         // Set grounded to false
                groundHitIsValid = false;
            }

            verticalVelocity -= gravityAccel * Time.fixedDeltaTime;  // Apply gravity
        }

        // ----------------------
        // --- PERFORM MOTION ---
        // ----------------------

        Vector3 moveVector = (transform.forward * velocity) + (Vector3.up * verticalVelocity) + pushVelocity;   // Calculate total velocity

        // Check for collision
        Vector3 capsuleCenter = transform.position + groundStickVector + coll.center + Vector3.up * groundCheckDistance;    // The center point of the capsule
        Vector3 capsuleHalfHeight = Vector3.up * (coll.height/2 - coll.radius);                                             // The vector from the center to one of the capsule points
        if(Physics.CapsuleCast(capsuleCenter - capsuleHalfHeight, capsuleCenter + capsuleHalfHeight, coll.radius,
                               moveVector, out sphereHit, moveVector.magnitude * Time.fixedDeltaTime, LayerMask.GetMask("Environment")))   // If we hit something
        {
            List<Vector3> hitNormals = new List<Vector3>();
            hitNormals.Add(sphereHit.normal);
            OnCollision(moveVector, groundStickVector, sphereHit, hitNormals);   // Handle the collision
        }
        else
        {          
            if(isGrounded)
            {
                rb.MoveRotation(Quaternion.Euler(0, turnAngle * Time.fixedDeltaTime, 0) * transform.rotation);  // Rotate the player around the Y-axis
            }

            rb.MovePosition(transform.position + moveVector * Time.fixedDeltaTime + groundStickVector);    // Apply movement

            // If pushVelocity is nearly zero
            if(pushVelocity.magnitude < Time.fixedDeltaTime)
            {
                pushVelocity = Vector3.zero;    // Remove it
            }
            else    // Otherwise
            {
                pushVelocity = pushVelocity.normalized * (pushVelocity.magnitude - acceleration * Time.fixedDeltaTime);    // Reduce pushVelocity strength
            }
        }

        Color speedometerColor = velocity >= highVelocity ? new Color(1, 0.5f, 0, 1) : Color.white;
        GameManager.instance.UIManager.SetSpeedometer(velocity * 2.23694f, speedometerColor);
    } 

    private void OnCollision(Vector3 movement, Vector3 groundStick, RaycastHit hit, List<Vector3> hitNormals)
    {
        Vector3 currentPosition = transform.position + groundStick;

        // Split movement into vertical and horizontal
        Vector3 playerMovement = movement - pushVelocity;
        Vector3 verMovement = Vector3.Project(playerMovement, Vector3.up);
        Vector3 horMovement = Vector3.ProjectOnPlane(playerMovement, Vector3.up);
        Vector3 stepVector = Vector3.zero;
        Vector3 newMovement;

        // Create a normal vector that represents the sum of all collision vectors that have occurred this frame
        Vector3 sumHits(List<Vector3> hits)
        {
            Vector3 output = Vector3.zero;
            foreach(Vector3 v in hits)
                output += v;
            return output.normalized;
        }
        Vector3 averageHitNormal = sumHits(hitNormals);

        Vector3 secondNormal = hit.normal;
        // Try a normal raycast at the same location
        RaycastHit secondHit;
        if(Physics.Raycast(hit.point + hit.normal * groundCheckDistance + horMovement.normalized * 0.01f,
                           -hit.normal,
                           out secondHit,
                           (groundCheckDistance + horMovement.magnitude) * 2,
                           LayerMask.GetMask("Environment")))
        {
            secondNormal = secondHit.normal;
        }

        // If the hit was at a corner within step height
        float slope = Vector3.Angle(Vector3.up, averageHitNormal);
        float secondSlope = Vector3.Angle(Vector3.up, secondNormal);
        float hitDifference = hit.point.y - currentPosition.y;
        if(secondHit.normal != hit.normal && secondSlope < slopeLimit && hitDifference > 0 && hitDifference <= stepHeight)
        {
            Debug.Log("Step");
            Debug.Log(averageHitNormal);
            Debug.Log(secondNormal);
            newMovement = movement;     // Keep the movement vector the same

            stepVector = Vector3.up * Mathf.Min(hitDifference, (Quaternion.FromToRotation(Vector3.up, averageHitNormal) * horMovement).y);    // Move up to accomodate
        }
        else if(slope < slopeLimit)           // If the collision was with ground
        {
            Vector3 projectedMovement = Vector3.ProjectOnPlane(movement, averageHitNormal);   // Get the movement vector along the ground plane

            // If we were already on the ground
            if(isGrounded)
            {
                Debug.Log("Ground Angle Adjust");
                newMovement = Quaternion.FromToRotation(movement, projectedMovement) * movement;    // Rotate our movement vector to align with the new ground normal
            }
            else // If we weren't on the ground
            {
                Debug.Log("Landing Collision");
                newMovement = Vector3.ProjectOnPlane(horMovement, averageHitNormal);    // Remove any excess vertical velocity
            }
        }
        else    // If the collision was with a wall
        {
            pushVelocity = Vector3.zero;    // If there was a pushVelocity, remove it
            
            // If we're on the ground
            if(isGrounded)
            {
                Vector3 wallVector = Vector3.ProjectOnPlane(horMovement, averageHitNormal);    // Get the movement vector along the wall

                // If the horizontal angle of the collision is within the glance limit
                if(Vector3.Angle(horMovement, -averageHitNormal) > 90 - wallLimit)
                {
                    Debug.Log("Glance Collision");
                    newMovement = wallVector;   // Set movement to the movement vector along the wall
                }
                else    // If the horizontal angle of the collision is too sharp
                {
                    Debug.Log("Bounce Collision");
                    if(velocity > highVelocity)     // If the player's going fast
                    {
                        pushVelocity = Vector3.ProjectOnPlane(averageHitNormal, Vector3.up) * horMovement.magnitude;      // Add a bounce velocity away from the wall
                        newMovement = horMovement/2;                                                                // Reduce horizontal velocity
                        newMovement += Vector3.up * 2;                                                              // Add a little hop
                    }
                    else                            // If the player's going slower
                    {
                        pushVelocity = Vector3.ProjectOnPlane(averageHitNormal, Vector3.up) * horMovement.magnitude;      // Add a smaller bounce velocity away from the wall
                        newMovement = (horMovement + wallVector).normalized * horMovement.magnitude/2;              // Set the velocity more along the wall
                    }
                }

                newMovement += verMovement;     // Combine with vertical movement
            }
            else    // If we're in midair
            {
                Debug.Log("Midair Collision");
                Vector3 intoWallMovement = Vector3.Project(movement, averageHitNormal);
                Vector3 alongWallMovement = Vector3.ProjectOnPlane(movement, averageHitNormal);

                if(Vector3.Dot(intoWallMovement, averageHitNormal) < 0)     // If the collision was towards the wall, reflect the movement
                {
                    Debug.Log("Reflect");
                    intoWallMovement = Vector3.ProjectOnPlane(Vector3.Reflect(intoWallMovement, averageHitNormal), Vector3.up);
                }

                newMovement = intoWallMovement + alongWallMovement;     // Sum the components
                
                Debug.Log(intoWallMovement);
                Debug.Log(alongWallMovement);
            }
        }

        Vector3 totalVelocity = newMovement + pushVelocity;

        // Check for collision
        Vector3 capsuleCenter = currentPosition + stepVector + coll.center + Vector3.up * groundCheckDistance;  // The center point of the capsule
        Vector3 capsuleHalfHeight = Vector3.up * (coll.height/2 - coll.radius);                                 // The vector from the center to one of the capsule points
        if(Physics.CapsuleCast(capsuleCenter - capsuleHalfHeight, capsuleCenter + capsuleHalfHeight, coll.radius,
                               totalVelocity, out hit, totalVelocity.magnitude * Time.fixedDeltaTime, LayerMask.GetMask("Environment")))   // If we hit something
        {
            if(hitNormals.Count < 10)
            {
                hitNormals.Add(hit.normal);
                OnCollision(newMovement, groundStick, hit, hitNormals);   // Handle the collision
            }
        }
        else
        {
            Vector3 newHorMovement = Vector3.ProjectOnPlane(newMovement, Vector3.up);   // Isolate the horizontal motion

            rb.MovePosition(currentPosition + stepVector + totalVelocity * Time.fixedDeltaTime);                    // Apply motion
            velocity = newHorMovement.magnitude;                                                                    // Update internal velocity to match
            verticalVelocity = Vector3.Project(newMovement, Vector3.up).y;                                          // Update internal velocity to match

            if(newHorMovement.magnitude > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(newHorMovement, Vector3.up);              // Calculate the rotation
                rb.MoveRotation(rotation);                                                              // Update the player's angle

                Debug.Log("New HorVelocity");
                Debug.Log(newHorMovement);

                // Lock the model's rotation for one physics frame
                IEnumerator revertModelRotation()
                {
                    Quaternion previousRotation = model.rotation;
                    float time = Time.time;
                    while(Time.time < time + Time.fixedDeltaTime)
                    {
                        yield return new WaitForEndOfFrame();
                        model.rotation = previousRotation;
                    }
                }
                StartCoroutine(revertModelRotation()); 
            }
        }
    }

    // This gets called whenever there is a change in the input for the Look action
    public void OnLook(CallbackContext context)
    {
        if(GameIsPaused()){
            lookInput = Vector2.zero;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
    }

    // This gets called whenever there is a change in the input for the Drive action
    public void OnDrive(CallbackContext context)
    {
        // If drive input is not zero, set it to 1 or -1 based on its sign
        driveInput = context.ReadValue<float>();
        if(driveInput != 0)
        {
            driveInput = Mathf.Sign(context.ReadValue<float>());
        }
    }

    // This gets called whenever there is a change in the input for the Turn action
    public void OnTurn(CallbackContext context)
    {
        turnInput = context.ReadValue<float>();     // Read the input value
        if(driftState == DriftState.None && driftInput > 0 && Mathf.Abs(turnInput) > 0) // If drift is pressed and a turn input is pressed but not yet drifting
        {
            driftState = (DriftState)Mathf.Sign(turnInput);                             // Drift in the turn direction
        }
    }

    // This gets called whenever there is a change in the input for the Drift action
    public void OnDrift(CallbackContext context)
    {
        // ----------------
        // --- DRIFTING ---
        // ----------------

        driftInput = context.ReadValue<float>();        // Read the input value
        if(driftState == DriftState.None)               // If not currently drifting
        {
            if(driftInput > 0 && Mathf.Abs(turnInput) > 0)          // If drift is pressed and a turn input is pressed
            {
                driftState = (DriftState)Mathf.Sign(turnInput);     // Drift in the turn direction
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
        if(LevelClearUI.levelClearUIActive || DeathScreen.deathUIActive){
            return;
        }

        if(PauseMenu.gameIsPaused){
            GameManager.instance.UIManager.pauseMenu.ResumeGame();
        }
        else{
            GameManager.instance.UIManager.pauseMenu.PauseGame();
        }        
    }

    public bool GameIsPaused()
    {
        return PauseMenu.gameIsPaused || LevelClearUI.levelClearUIActive || DeathScreen.deathUIActive;
    }

    // This gets called whenever there is a change in the input for the Fire action
    public void OnFire(CallbackContext context)
    {
        // If the fire button was pressed
        if(!GameIsPaused() && canShoot && context.performed && context.ReadValue<float>() > 0)
        {
            Vector3 bulletDirection = Camera.main.transform.forward;

            // Send a raycast out from the camera in the direction the player is looking
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.transform.position,
                               Camera.main.transform.forward,
                               out hit,
                               100))                                                // If something was hit
            {
                IShootable shootScript = hit.transform.GetComponent<IShootable>();  // Try to get an IShootable component
                if(shootScript != null)                                             // If one was found
                {
                    shootScript.OnShoot();                                          // Call the shoot function
                }
                bulletDirection = hit.point - bulletSpawnPoint.position;
            }

            anim.SetTrigger("Shoot");   // Trigger the shoot animation
            Bullet bulletScript = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity).GetComponent<Bullet>();
            bulletScript.Initialize(bulletDirection.normalized * 200, 100);

            // Activate muzzle flare
            muzzleFlare.SetActive(true);
            IEnumerator DisableFlare() { yield return new WaitForSeconds(0.05f); muzzleFlare.SetActive(false); }
            StartCoroutine(DisableFlare());

            // Disable shooting temporarily
            canShoot = false;
            IEnumerator ResetShooting() { yield return new WaitForSeconds(1/fireRate); canShoot = true; }
            StartCoroutine(ResetShooting());
        }
    }
}
