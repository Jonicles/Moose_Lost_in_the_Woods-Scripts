using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RumbleType
{
    ControllerConnect,
    Mushroom,
    Pickup,
    Build
}

[System.Serializable]
public class Rumble
{
    //Makes sure you can put values in the inspector without them being accessible from other scripts
    public string Name { get { return name; } private set { name = value; } }
    [SerializeField] string name;

    public RumbleType RumbleType { get { return rumbleType; } private set { rumbleType = value; } }
    [SerializeField] RumbleType rumbleType;

    public float LowFrequency { get { return lowFrequency; } private set { lowFrequency = value; } }
    [SerializeField] float lowFrequency;

    public float HighFrequency { get { return highFrequency; } private set { highFrequency = value; } }
    [SerializeField] float highFrequency;

    public float RumbleTime { get { return rumbleTime; } private set { rumbleTime = value; } }
    [SerializeField] float rumbleTime;

    public AnimationCurve RumbleCurve { get { return shakeCurve; } private set { shakeCurve = value; } }
    [SerializeField] AnimationCurve shakeCurve;
}
public class SCR_RumbleManager : MonoBehaviour
{
    public static SCR_RumbleManager Instance;

    [SerializeField] List<Rumble> rumbleList = new List<Rumble>();
    Dictionary<RumbleType, Rumble> rumbleTypeDictionary = new Dictionary<RumbleType, Rumble>();

    Coroutine rumbleRoutine;
    void Awake()
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
    }

    private void Start()
    {
        foreach (Rumble rumbleType in rumbleList)
        {
            rumbleTypeDictionary.Add(rumbleType.RumbleType, rumbleType);
        }
    }

    public void StartRumble(RumbleType rumbleType)
    {
        if (rumbleRoutine != null)
        {
            if (Gamepad.current != null)
                Gamepad.current.ResetHaptics();

            StopCoroutine(rumbleRoutine);
        }

        float lowFrequency = rumbleTypeDictionary[rumbleType].LowFrequency;
        float highFrequency = rumbleTypeDictionary[rumbleType].HighFrequency;
        float rumbleTime = rumbleTypeDictionary[rumbleType].RumbleTime;
        AnimationCurve curve = rumbleTypeDictionary[rumbleType].RumbleCurve;

        if (Gamepad.current != null)
            rumbleRoutine = StartCoroutine(RumbleController(lowFrequency, highFrequency, rumbleTime, curve));
    }

    IEnumerator RumbleController(float lowFrequency, float highFrequency, float rumbleTime, AnimationCurve curve)
    {
        float elapsedTime = 0;

        float startLowFrequency = lowFrequency;
        float startHighFrequency = highFrequency;

        while (rumbleTime > elapsedTime)
        {
            lowFrequency = Mathf.Lerp(startLowFrequency, 0, curve.Evaluate(elapsedTime / rumbleTime));
            highFrequency = Mathf.Lerp(startHighFrequency, 0, curve.Evaluate(elapsedTime / rumbleTime));

            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (Gamepad.current != null)
            Gamepad.current.ResetHaptics();

        rumbleRoutine = null;

        yield return null;
    }
}
