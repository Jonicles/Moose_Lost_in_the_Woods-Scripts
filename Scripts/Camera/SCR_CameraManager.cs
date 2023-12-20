using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

//John
public class SCR_CameraManager : MonoBehaviour
{
    public static SCR_CameraManager Instance;

    int basePriority = 1;
    int highPriority = 5;

    [Header("Camera Reset Settings")]
    [SerializeField] float resetTime = 2;
    [SerializeField] AnimationCurve resetCurve;
    Coroutine resetRoutine;

    [Header("FOV Settings")]
    [SerializeField] float sprintFOV;
    [SerializeField] float increaseTime;
    [SerializeField] AnimationCurve increaseCurve;
    [SerializeField] float normalFOV;
    [SerializeField] float decreaseTime;
    [SerializeField] AnimationCurve decreaseCurve;
    Coroutine fovRoutine;


    Transform playerTransform;

    CinemachineVirtualCamera worldCamera;
    [SerializeField] CinemachineVirtualCamera bounceCamera;
    CinemachineVirtualCamera currentCamera;

    SCR_CameraRotate worldRotation;
    SCR_CameraRotate bounceRotation;

    SCR_PlayerInputActions playerInputActions;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        worldCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        currentCamera = worldCamera;

        bounceRotation = bounceCamera.gameObject.GetComponent<SCR_CameraRotate>();
        worldRotation = worldCamera.gameObject.GetComponent<SCR_CameraRotate>();


        SceneManager.sceneLoaded += FindPlayerTransform;
        SceneManager.sceneLoaded += StopCameraRoutine;
        Scene tempScene = new Scene();
        FindPlayerTransform(tempScene, LoadSceneMode.Additive);
    }

    private void StopCameraRoutine(Scene arg0, LoadSceneMode arg1)
    {
        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
        }
    }

    private void Start()
    {
        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.Player.ResetCamera.started += ResetCamera;
    }

    private void ResetCamera(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (resetRoutine == null)
        {
            Transform worldCameraStartPosition = playerTransform.Find("World Camera Start").transform;
            Transform bounceCameraStartPosition = playerTransform.Find("Bounce Camera Start").transform;

            playerInputActions.Player.Disable();
            bounceRotation.TurnOffRotation();
            worldRotation.TurnOffRotation();
            resetRoutine = StartCoroutine(CameraMove(worldCameraStartPosition, bounceCameraStartPosition));
        }
    }

    public void ResetOnSceneLoad()
    {
        Transform worldCameraStartPosition = playerTransform.Find("World Camera Start").transform;
        Transform bounceCameraStartPosition = playerTransform.Find("Bounce Camera Start").transform;
        resetRoutine = StartCoroutine(CameraMoveBeginning(worldCameraStartPosition, bounceCameraStartPosition));
    }

    IEnumerator CameraMoveBeginning(Transform worldTransform, Transform bounceTransform)
    {
        float elapsedTime = 0;
        Vector3 worldCameraStartPosition = worldCamera.transform.position;
        Vector3 bounceCameraStartPosition = bounceCamera.transform.position;

        Quaternion worldStartRotation = worldCamera.transform.rotation;
        Quaternion bounceStartRotation = bounceCamera.transform.rotation;


        while (resetTime > elapsedTime)
        {
            worldCamera.transform.position = Vector3.Lerp(worldCameraStartPosition, worldTransform.position, resetCurve.Evaluate(elapsedTime / resetTime));
            bounceCamera.transform.position = Vector3.Lerp(bounceCameraStartPosition, bounceTransform.position, resetCurve.Evaluate(elapsedTime / resetTime));

            worldCamera.transform.rotation = Quaternion.Lerp(worldStartRotation, worldTransform.rotation, resetCurve.Evaluate(elapsedTime / resetTime));
            bounceCamera.transform.rotation = Quaternion.Lerp(bounceStartRotation, bounceTransform.rotation, resetCurve.Evaluate(elapsedTime / resetTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        worldCamera.transform.position = worldTransform.position;
        bounceCamera.transform.position = bounceTransform.position;

        worldCamera.transform.rotation = worldTransform.rotation;
        bounceCamera.transform.rotation = bounceTransform.rotation;

        resetRoutine = null;
    }

    IEnumerator CameraMove(Transform worldTransform, Transform bounceTransform)
    {
        float elapsedTime = 0;
        Vector3 worldCameraStartPosition = worldCamera.transform.position;
        Vector3 bounceCameraStartPosition = bounceCamera.transform.position;

        Quaternion worldStartRotation = worldCamera.transform.rotation;
        Quaternion bounceStartRotation = bounceCamera.transform.rotation;


        while (resetTime > elapsedTime)
        {
            worldCamera.transform.position = Vector3.Lerp(worldCameraStartPosition, worldTransform.position, resetCurve.Evaluate(elapsedTime / resetTime));
            bounceCamera.transform.position = Vector3.Lerp(bounceCameraStartPosition, bounceTransform.position, resetCurve.Evaluate(elapsedTime / resetTime));

            worldCamera.transform.rotation = Quaternion.Lerp(worldStartRotation, worldTransform.rotation, resetCurve.Evaluate(elapsedTime / resetTime));
            bounceCamera.transform.rotation = Quaternion.Lerp(bounceStartRotation, bounceTransform.rotation, resetCurve.Evaluate(elapsedTime / resetTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        worldCamera.transform.position = worldTransform.position;
        bounceCamera.transform.position = bounceTransform.position;

        worldCamera.transform.rotation = worldTransform.rotation;
        bounceCamera.transform.rotation = bounceTransform.rotation;

        resetRoutine = null;
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FindPlayerTransform;
        SceneManager.sceneLoaded -= StopCameraRoutine;

    }

    private void FindPlayerTransform(Scene arg0, LoadSceneMode arg1)
    {
        if (FindObjectOfType<SCR_PlayerMovement>() != null)
        {
            playerTransform = GameObject.Find("P_Player").transform;
            worldCamera.m_Follow = playerTransform;
            worldCamera.m_LookAt = playerTransform;
            bounceCamera.m_Follow = playerTransform;
            bounceCamera.LookAt = playerTransform;
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public void ActivateBounceCamera()
    {
        ActivateCamera(bounceCamera);
    }

    public void ResetToWorldCamera()
    {
        ActivateCamera(worldCamera);
        SCR_PlayerInputManager.Instance.Player.Look.Enable();
    }

    public void ChooseCamera(CinemachineVirtualCamera[] possibleCameras)
    {
        //Set to world camera to safe proof
        CinemachineVirtualCamera cameraToActivate = worldCamera;
        float currentShortestDistance = 500;
        for (int i = 0; i < possibleCameras.Length; i++)
        {
            if (Vector3.Distance(possibleCameras[i].transform.position, worldCamera.transform.position) < currentShortestDistance)
            {
                currentShortestDistance = Vector3.Distance(possibleCameras[i].transform.position, worldCamera.transform.position);
                cameraToActivate = possibleCameras[i];
            }
        }
        ActivateCamera(cameraToActivate);
    }

    public void ActivateCamera(CinemachineVirtualCamera cameraToActivate)
    {
        //Safe proof so priority in cameras is not messed with 
        if (cameraToActivate == currentCamera)
            return;
        if (cameraToActivate != bounceCamera)
            SCR_PlayerInputManager.Instance.Player.Look.Disable();

        cameraToActivate.m_Priority = highPriority;
        DeactivateCamera(currentCamera);
        currentCamera = cameraToActivate;
    }
    public void DeactivateCamera(CinemachineVirtualCamera cameraToDeactivate)
    {
        cameraToDeactivate.m_Priority = basePriority;
    }

    public void ChangeFOV(bool isIncreasing)
    {
        if (fovRoutine != null)
        {
            StopCoroutine(fovRoutine);
        }

        if (isIncreasing)
        {
            fovRoutine = StartCoroutine(FOV(sprintFOV, increaseTime, increaseCurve));
        }
        else
        {
            fovRoutine = StartCoroutine(FOV(normalFOV, decreaseTime, decreaseCurve));

        }
    }

    IEnumerator FOV(float desiredFOV, float time, AnimationCurve animationCurve)
    {
        float elapsedTime = 0;
        float startFOV = worldCamera.m_Lens.FieldOfView;

        while (resetTime > elapsedTime)
        {
            worldCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, desiredFOV, animationCurve.Evaluate(elapsedTime / time));
            bounceCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, desiredFOV, animationCurve.Evaluate(elapsedTime / time));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        worldCamera.m_Lens.FieldOfView = desiredFOV;
        bounceCamera.m_Lens.FieldOfView = desiredFOV;

        fovRoutine = null;
    }
}
