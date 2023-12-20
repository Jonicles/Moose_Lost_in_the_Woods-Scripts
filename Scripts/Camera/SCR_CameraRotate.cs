using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//John 
public class SCR_CameraRotate : MonoBehaviour
{
    [Header("Rotation settings")]
    [SerializeField] float deadzoneSize = 0.2f;
    [SerializeField] float rotationSpeed = 20;

    [Header("Rotation Curves")]
    [SerializeField] AnimationCurve endRotationCurve;
    [SerializeField] float endRotationSmoothingTime = 0.4f;
    [SerializeField] AnimationCurve startRotationCurve;
    [SerializeField] float startRotationSmoothingTime = 0.4f;

    //Other variables we need to track and change
    float horizontalInput;
    float rotationDirection;
    float currentRotationSpeed;
    bool isRotating; 
    Coroutine smoothingRoutine;
    SCR_PlayerInputActions playerInputActions;
    Transform playerTransform;

    void Awake()
    {
        playerInputActions = SCR_PlayerInputManager.Instance;
        playerTransform = GameObject.Find("P_Player").transform;
        SceneManager.sceneLoaded += FindPlayerTransform;
        Scene tempScene = new Scene();
        FindPlayerTransform(tempScene, LoadSceneMode.Additive);
    }

    private void FindPlayerTransform(Scene arg0, LoadSceneMode arg1)
    {
        if (FindObjectOfType<SCR_PlayerMovement>() != null)
        {
            playerTransform = GameObject.Find("P_Player").transform;
        }
    }

    private void OnEnable()
    {
        //playerInputActions.Player.Look.performed += RotationInput;
        playerInputActions.Player.Look.canceled += ResetRotation;
    }

    private void OnDisable()
    {
        //playerInputActions.Player.Look.performed -= RotationInput;
        playerInputActions.Player.Look.canceled -= ResetRotation;
    }

    private void Update()
    {
        float previousInput = horizontalInput;
        Vector2 input = playerInputActions.Player.Look.ReadValue<Vector2>();
        input.Normalize();
        horizontalInput = input.x;


        if ((previousInput <= 0 && horizontalInput > 0) || (previousInput >= 0 && horizontalInput < 0))
        {
            isRotating = false;
        }

        //Own coded "Deadzone" check, inputs strength must go over a certain threshold before even regestrering
        if (!isRotating && Math.Abs(horizontalInput) > deadzoneSize)
        {
            isRotating = true;
            StartRotation();

            if (horizontalInput > 0)
            {
                rotationDirection = 1;
            }
            else
                rotationDirection = -1;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
            RotateCamera();
    }

    //private void RotationInput(InputAction.CallbackContext context)
    //{
       
    //}
    void RotateCamera()
    {
        if (currentRotationSpeed > 0.05)
        {
            transform.RotateAround(playerTransform.position, new Vector3(0, rotationDirection), currentRotationSpeed * Time.deltaTime);
        }
    }

    private void StartRotation()
    {
        if (smoothingRoutine != null)
        {
            StopCoroutine(smoothingRoutine);
        }
        smoothingRoutine = StartCoroutine(IncreaseRotationSpeed());
    }
    IEnumerator DecreaseRotationSpeed()
    {
        //Decreases the rotation speed overtime so camera smoothly comes to a stop and not abruptly
        float elapsedTime = 0;
        float startRotationSpeed = currentRotationSpeed;

        while (endRotationSmoothingTime > elapsedTime)
        {
            currentRotationSpeed = Mathf.Lerp(startRotationSpeed, 0, endRotationCurve.Evaluate(elapsedTime / endRotationSmoothingTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        smoothingRoutine = null;
        currentRotationSpeed = 0;
        rotationDirection = 0;
    }
    private void ResetRotation(InputAction.CallbackContext context)
    {
        isRotating = false;
        horizontalInput = 0;

        if (smoothingRoutine != null)
        {
            StopCoroutine(smoothingRoutine);
        }
        smoothingRoutine = StartCoroutine(DecreaseRotationSpeed());
    }
    IEnumerator IncreaseRotationSpeed()
    {
        //Decreases the rotation speed overtime so camera smoothly comes to a stop and not abruptly
        float elapsedTime = 0;
        float startRotationSpeed = currentRotationSpeed;

        while (startRotationSmoothingTime > elapsedTime)
        {
            currentRotationSpeed = Mathf.Lerp(startRotationSpeed, rotationSpeed, startRotationCurve.Evaluate(elapsedTime / startRotationSmoothingTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentRotationSpeed = rotationSpeed;
        smoothingRoutine = null;
    }

    public void TurnOffRotation()
    {
        if (smoothingRoutine != null)
            StopCoroutine(smoothingRoutine);

        currentRotationSpeed = 0;
        rotationDirection = 0;
    }
}
