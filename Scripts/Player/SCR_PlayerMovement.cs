using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//John
public class SCR_PlayerMovement : MonoBehaviour
{
    Rigidbody playerRigidbody;
    SCR_PlayerInputActions playerInputActions;
    [SerializeField] SCR_CharacterEyeChanger eyeChanger;

    enum PlayerState
    {
        Grounded,
        Airborne,
        Swimming
    }

    [Header("Player Settings")]
    [SerializeField] PlayerState currentState = PlayerState.Grounded;
    [SerializeField] float maxWalkSpeed = 10;
    [SerializeField] float sprintMultiplier = 2;
    [SerializeField] float jumpForce = 1;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] float jumpBufferTime = 0.3f;
    [SerializeField] float coyoteTime;
    [Range(0, 2)]
    //This is only to prevent "Super jumping"
    [SerializeField] float jumpCooldownTime = 0.2f;

    [Header("Acceleration Settings")]
    [SerializeField] float accelerationTime;
    [SerializeField] AnimationCurve accelerationCurve;

    [SerializeField] float decelerationTime;
    [SerializeField] AnimationCurve decelerationCurve;

    [Header("Gravity Settings")]
    [SerializeField] float globalGravity = -9.81f;
    [SerializeField] float gravityMultiplier = 1.0f;
    [SerializeField] float fallGravityMultiplier = 5;

    [Header("Water Settings")]
    [SerializeField] float swimmingMultiplier;
    [SerializeField] float buoyancyStrength;
    [SerializeField] float waterViscosity;
    [SerializeField] float depth;
    [SerializeField] float waterDecelerationTime;
    [SerializeField] AnimationCurve waterDecelerationCurve;

    [Header("Particles")]
    [SerializeField] ParticleSystem waterParticles;
    [SerializeField] ParticleSystem waterTrailParticles;

    [Header("Other Settings")]
    [SerializeField] float groundRayLength;
    [SerializeField] float groundRayThickness;
    [SerializeField] [Range(0, 1)] float slopeRayMaxSlope;
    [SerializeField] float slopeRayThickness;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float collectRotationSpeed;
    [SerializeField] AnimationCurve collectRotationCurve;



    bool isBouncing = false;
    bool isSprinting = false;
    bool isInDanceMode = false;
    bool isOnSlope = false;
    public bool IsSprinting { get { return isSprinting; } private set { value = isSprinting; } }
    bool hasJumped = false;

    float timeSinceInput = 0;
    float timeSinceLastJump = 0;
    float timeSinceLastPlatform = 0;
    float currentSpeed;
    Vector3 latestMoveDirection;
    Coroutine accelerationRoutine;
    Coroutine collectingRoutine;
    Coroutine interactRotationRoutine;
    Transform currentWater;

    //Animator strings
    const string jumpingString = "isJumping";
    const string fallingString = "isFalling";
    const string groundedString = "isGrounded";
    const string swimmingString = "isSwimming";
    const string collectingString = "isCollecting";
    const string speedString = "currentSpeed";

    //Tag Strings
    const string waterTag = "Water";
    const string bounceTag = "Bounce";
    const string coneTag = "Cone";

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerInputActions = SCR_PlayerInputManager.Instance;
        //Makes sure all the other action maps are disabled


        playerInputActions.Player.Jump.performed += JumpPress;
        playerInputActions.Player.Jump.canceled += JumpDragDown;
        playerInputActions.Player.Move.started += AccelerationStart;
        playerInputActions.Player.Move.canceled += DecelerationStart;

        playerInputActions.Player.Inventory.started += SprintCancel;
        playerInputActions.Player.Inventory.started += InventoryPressed;
        playerInputActions.Player.Sprint.started += SprintStart;
        playerInputActions.Player.Sprint.canceled += SprintCancel;
    }

    public void StopAndInteract(bool speakingInteract, Transform objectTransform)
    {
        StopSprinting();
        latestMoveDirection = Vector3.zero;

        if (speakingInteract)
        {
            if (interactRotationRoutine != null)
                StopCoroutine(interactRotationRoutine);

            interactRotationRoutine = StartCoroutine(RotateTowardObject(objectTransform.position));
            return;
        }

        if (collectingRoutine == null)
        {
            playerAnimator.SetBool(collectingString, true);
            collectingRoutine = StartCoroutine(CollectInteractable(objectTransform));
        }
    }

    IEnumerator CollectInteractable(Transform objectTransform)
    {
        if (interactRotationRoutine != null)
            StopCoroutine(interactRotationRoutine);

        eyeChanger.SetEyesToHappy();

        interactRotationRoutine = StartCoroutine(RotateTowardObject(objectTransform.position));
        playerInputActions.Player.Disable();

        yield return new WaitForSeconds(0.3f);
        playerAnimator.SetBool(collectingString, false);
        yield return new WaitForSeconds(0.4f);
        eyeChanger.ResetEyes();
        playerInputActions.Player.Enable();
        collectingRoutine = null;
    }

    IEnumerator RotateTowardObject(Vector3 objectPosition)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion desiredRotation = Quaternion.LookRotation(objectPosition - transform.position);
        desiredRotation.x = startRotation.x;
        desiredRotation.z = startRotation.z;

        float elapsedTime = 0;

        while (collectRotationSpeed > elapsedTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, desiredRotation, collectRotationCurve.Evaluate(elapsedTime / collectRotationSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = desiredRotation;

        interactRotationRoutine = null;
    }

    private void InventoryPressed(InputAction.CallbackContext obj)
    {
        SetSpeedToZero();
    }

    public void SetSpeedToZero()
    {
        currentSpeed = 0;
    }

    private void SprintCancel(InputAction.CallbackContext context)
    {
        StopSprinting();
    }

    private void StopSprinting()
    {
        SCR_CameraManager.Instance.ChangeFOV(false);
        isSprinting = false;
    }

    private void SprintStart(InputAction.CallbackContext context)
    {
        StartSprinting();
    }

    private void StartSprinting()
    {
        if (currentState != PlayerState.Swimming && currentState != PlayerState.Airborne)
        {
            SCR_CameraManager.Instance.ChangeFOV(true);
            isSprinting = true;
        }
    }

    private void Start()
    {
        timeSinceInput = jumpBufferTime;
        SCR_CameraManager.Instance.ResetOnSceneLoad();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Jump.performed -= JumpPress;
        playerInputActions.Player.Jump.canceled -= JumpDragDown;
        playerInputActions.Player.Move.started -= AccelerationStart;
        playerInputActions.Player.Move.canceled -= DecelerationStart;

        playerInputActions.Player.Inventory.started -= SprintCancel;
        playerInputActions.Player.Inventory.started -= InventoryPressed;
        playerInputActions.Player.Sprint.started -= SprintStart;
        playerInputActions.Player.Sprint.canceled -= SprintCancel;
    }


    private void DecelerationStart(InputAction.CallbackContext context)
    {
        if (accelerationRoutine != null)
        {
            StopCoroutine(accelerationRoutine);
        }

        accelerationRoutine = StartCoroutine(Decelerate(0.0f));
    }
    IEnumerator Decelerate(float desiredSpeed)
    {
        float currentDecelerationTime = decelerationTime;
        //AnimationCurve currentDecelerationCurve = decelerationCurve;

        if (currentState == PlayerState.Swimming)
        {
            currentDecelerationTime = waterDecelerationTime;
            //currentDecelerationCurve = waterDecelerationCurve;
        }
        float elapsedTime = 0;
        float startSpeed = currentSpeed;

        while (currentDecelerationTime > elapsedTime)
        {
            currentSpeed = Mathf.Lerp(startSpeed, desiredSpeed, decelerationCurve.Evaluate(elapsedTime / currentDecelerationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetSpeedToZero();
        accelerationRoutine = null;
    }

    private void AccelerationStart(InputAction.CallbackContext context)
    {
        latestMoveDirection = Vector3.zero;
        if (accelerationRoutine != null)
        {
            StopCoroutine(accelerationRoutine);
        }

        accelerationRoutine = StartCoroutine(Accelerate(maxWalkSpeed));
    }
    IEnumerator Accelerate(float desiredSpeed)
    {
        float elapsedTime = 0;
        float startSpeed = currentSpeed;

        while (accelerationTime > elapsedTime)
        {
            currentSpeed = Mathf.Lerp(startSpeed, desiredSpeed, accelerationCurve.Evaluate(elapsedTime / accelerationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = desiredSpeed;
        accelerationRoutine = null;
    }

    private void Update()
    {
        if (timeSinceInput < jumpBufferTime)
        {
            timeSinceInput += Time.deltaTime;
        }
        if (timeSinceLastJump < jumpCooldownTime)
        {
            timeSinceLastJump += Time.deltaTime;
        }

        UpdateState();

        float currentMultiplier = isSprinting ? sprintMultiplier : 1;

        if (currentSpeed > 0.5f)
        {
            playerAnimator.SetFloat(speedString, latestMoveDirection.magnitude * currentMultiplier);
        }
        else
        {
            playerAnimator.SetFloat(speedString, 0);
        }
    }
    private void FixedUpdate()
    {
        if (timeSinceLastPlatform <= coyoteTime)
        {
            timeSinceLastPlatform += Time.deltaTime;
        }

        float currentScale = 1;

        if (currentState != PlayerState.Swimming)
        {
            if (playerRigidbody.velocity.y <= 0 && currentState == PlayerState.Airborne)
            {
                currentScale = fallGravityMultiplier;
                TurnOffBools(fallingString);
                playerAnimator.SetBool(fallingString, true);
            }
            else if (playerRigidbody.velocity.y > 0 && currentState == PlayerState.Airborne)
            {
                playerAnimator.SetBool(jumpingString, true);
                TurnOffBools(jumpingString);
                currentScale = gravityMultiplier;
            }
            else
            {
                currentScale = gravityMultiplier;
            }
        }
        else
        {
            if (currentWater != null)
            {
                if (transform.position.y < currentWater.position.y - depth)
                {
                    //Water Physics being applied while in water
                    //Buoyoancy
                    playerRigidbody.AddForce(new Vector3(0, buoyancyStrength, 0), ForceMode.Acceleration);
                    //Water drag
                    playerRigidbody.AddForce(playerRigidbody.velocity * -1 * waterViscosity);
                }
            }
        }

        //Faux gravity, increased gravity when in air
        Vector3 gravity = globalGravity * currentScale * Vector3.up;
        playerRigidbody.AddForce(gravity, ForceMode.Acceleration);

        latestMoveDirection = ReadMovementInput();
        MovePlayer(latestMoveDirection);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!hasJumped)
        {
            timeSinceLastPlatform = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasJumped = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(waterTag))
        {
            if (IsSprinting)
                StopSprinting();

            if (isBouncing)
            {
                isBouncing = false;
                SCR_CameraManager.Instance.ResetToWorldCamera();
            }

            //Different sound depending on if you jump or walk into the water
            waterTrailParticles.Play();
            if (currentState == PlayerState.Airborne)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Player_Water_Enter", gameObject);
                waterParticles.Play();
            }
            else
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Player_Water_Exit_Walk", gameObject);

            playerAnimator.SetBool(swimmingString, true);
            TurnOffBools(swimmingString);
            ChangePlayerState(PlayerState.Swimming);
            currentWater = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(waterTag))
        {
            ChangePlayerState(PlayerState.Airborne);
            currentWater = null;
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Player_Water_Exit_Walk", gameObject);
            waterTrailParticles.Stop();
        }
    }

    private void UpdateState()
    {
        if (currentState != PlayerState.Swimming)
        {
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, groundRayThickness, Vector3.down, out hitInfo, groundRayLength);
            RaycastHit hitInfo2;
            //Physics.Raycast(transform.position, Vector3.down, out hitInfo2, groundRayLength);
            Physics.SphereCast(transform.position, slopeRayThickness, Vector3.down, out hitInfo2, groundRayLength);

            if (hitInfo2.normal.y < slopeRayMaxSlope)
            {
                isOnSlope = true;
            }
            else
                isOnSlope = false;

            if (hitInfo.collider == null || hitInfo.collider.tag == coneTag)
            {
                ChangePlayerState(PlayerState.Airborne);
                isOnSlope = false;
                return;
            }
            if (hitInfo.collider.CompareTag(waterTag))
            {
                //Makes sure we don't accidentally make the player grounded while they are in water
                return;
            }
            if (hitInfo.collider.CompareTag(bounceTag))
            {
                //Makes sure the player never becomes grounded when bouncing on a surface
                if (!isBouncing)
                {
                    SCR_CameraManager.Instance.ActivateBounceCamera();
                }
                isBouncing = true;
                return;
            }
            //If you are past this point that means the raycast has hit the ground
            //Executes jump if you pressed the jump button just before landing, this is to make sure it doesn't feel like the game eats our inputs

            if (currentState != PlayerState.Grounded)
            {
                hasJumped = false;
                isBouncing = false;

                if (playerInputActions.Player.Move.enabled)
                    SCR_CameraManager.Instance.ResetToWorldCamera();

                playerAnimator.SetBool(groundedString, true);
                TurnOffBools(groundedString);
                ChangePlayerState(PlayerState.Grounded);
            }

            if (timeSinceInput < jumpBufferTime)
            {
                JumpExecute();
            }
        }
    }

    private void ChangePlayerState(PlayerState newState)
    {
        if (newState == currentState)
            return;

        currentState = newState;
    }

    private Vector3 ReadMovementInput()
    {
        Vector2 inputDirection = playerInputActions.Player.Move.ReadValue<Vector2>();
        //Wont do any further calculation if the player is not currently inputting anything
        if (inputDirection.magnitude < 0.4f)
        {
            return latestMoveDirection;
        }

        float horizontalInput = inputDirection.x;
        float verticalInput = inputDirection.y;

        Vector3 movementDirection = GetRelativeCameraVector(horizontalInput, verticalInput);
        return movementDirection;
    }

    private void MovePlayer(Vector3 movementDirection)
    {
        //By using Addforce we never have to be concerned that putting the direction in the y axis to 0 will affect our jump, this only means that no force will be applied in the relative y axis.
        float moveMultiplier = 1;
        if (isSprinting)
        {
            moveMultiplier = sprintMultiplier;
        }
        if (currentState == PlayerState.Swimming)
        {
            moveMultiplier = swimmingMultiplier;
        }

        playerRigidbody.AddForce(movementDirection * currentSpeed * moveMultiplier, ForceMode.Force);
    }

    private Vector3 GetRelativeCameraVector(float horizontalInput, float verticalInput)
    {
        //This method is taken from iHeartGameDev, I only figured out how to translate the player inputs into vectors and then applying the system to ridigbody.Addforce()
        Vector3 cameraForwardDirection = Camera.main.transform.forward;
        Vector3 cameraRightDirection = Camera.main.transform.right;

        //This is to make sure the player does not start walking into the sky when the camera is facing the sky, movement on y axis will always be dictated by gravity
        cameraForwardDirection.y = 0;
        cameraRightDirection.y = 0;

        //The values are normalized so are force will not be dependent on the angle of the camera
        cameraForwardDirection = cameraForwardDirection.normalized;
        cameraRightDirection = cameraRightDirection.normalized;

        Vector3 forwardRelativeInput = verticalInput * cameraForwardDirection;
        Vector3 rightRelativeInput = horizontalInput * cameraRightDirection;

        Vector3 cameraRelativeMovementDirection = forwardRelativeInput + rightRelativeInput;
        RotateTowardMovementDirection(cameraRelativeMovementDirection);
        return cameraRelativeMovementDirection;
    }
    private void RotateTowardMovementDirection(Vector3 cameraRelativeMovementDirection)
    {
        float multiplier = 1;

        if (isSprinting)
            multiplier = sprintMultiplier;

        Quaternion toRotation = Quaternion.LookRotation(cameraRelativeMovementDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * multiplier * Time.deltaTime);
    }
    void JumpPress(InputAction.CallbackContext context)
    {
        if (currentState == PlayerState.Airborne)
        {
            timeSinceInput = 0;
        }

        if (!hasJumped && timeSinceLastPlatform < coyoteTime)
        {
            JumpExecute();
            return;
        }

        if (currentState != PlayerState.Grounded && currentState != PlayerState.Swimming)
        {
            return;
        }

        //This is to prevent "Super jumping", or the jump executing multiple times cause you stay grounded too long
        if (timeSinceLastJump > jumpCooldownTime)
        {
            JumpExecute();
        }
    }
    void JumpExecute()
    {
        if (((isOnSlope || hasJumped) && currentState != PlayerState.Swimming) || isBouncing)
        {
            return;
        }

        hasJumped = true;
        timeSinceLastPlatform = coyoteTime;
        timeSinceLastJump = 0;
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);

        //Actual jump force being applied
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        //Different sound depending on if you jump in water or on land
        if (currentState != PlayerState.Swimming)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Player_Jump", gameObject);
        }
        else
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Player_Water_Exit_Jump", gameObject);

        timeSinceInput = jumpBufferTime;
    }
    private void JumpDragDown(InputAction.CallbackContext context)
    {
        if (currentState != PlayerState.Swimming && isBouncing == false)
        {
            //Drag the player down, the jump becomes more analog thanks to this method
            playerRigidbody.AddForce(Vector3.down * (jumpForce * 0.5f), ForceMode.Impulse);
        }
    }
    private void TurnOffBools(string newState)
    {
        if (newState == jumpingString)
        {
            playerAnimator.SetBool(groundedString, false);
            playerAnimator.SetBool(fallingString, false);
            playerAnimator.SetBool(swimmingString, false);
            return;
        }
        if (newState == fallingString)
        {
            playerAnimator.SetBool(groundedString, false);
            playerAnimator.SetBool(jumpingString, false);
            playerAnimator.SetBool(swimmingString, false);
            return;
        }
        if (newState == groundedString)
        {
            playerAnimator.SetBool(jumpingString, false);
            playerAnimator.SetBool(fallingString, false);
            playerAnimator.SetBool(swimmingString, false);
            return;
        }
        if (newState == swimmingString)
        {
            playerAnimator.SetBool(jumpingString, false);
            playerAnimator.SetBool(fallingString, false);
            playerAnimator.SetBool(groundedString, false);
            return;
        }
    }

    public void SwitchDanceMode()
    {
        isInDanceMode = !isInDanceMode;
        playerAnimator.SetBool("isInDanceMode", isInDanceMode);
    }
    //Debugging methods
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundRayLength);
        Gizmos.DrawWireSphere(transform.position, groundRayThickness);
    }
}
