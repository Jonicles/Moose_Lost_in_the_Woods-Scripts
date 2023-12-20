using System.Collections;
using UnityEngine;

// Svante
public class SCR_CharacterEyeChanger : MonoBehaviour
{
    //Script created by Svante
    [SerializeField] Material[] eyeMaterials;
    private bool blinkTrigger;
    private float timeToBlink;

    SkinnedMeshRenderer skinnedMeshRenderer;
    Coroutine blinkRoutine;
    enum EyeType
    {
        Open,
        Closed,
        Happy,
        Mole
    }

    EyeType currentEyeType = EyeType.Open;


    private void Awake()
    {
        skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
    }
    void Start()
    {
        if (gameObject.name == "SK_Eyes_Mole")
        {
            SetEyesToMole();
        }
        else if(gameObject.name == "SK_Eyes_Olle")
        {
            SetEyesToHappy();
        }
        else
        {
            ChangeEyeType(currentEyeType);
            blinkTrigger = false;
        }
    }

    void Update()
    {
        if (!blinkTrigger)
        {
            if (timeToBlink > 0)
            {
                timeToBlink -= Time.deltaTime;
            }
            else
            {
                blinkTrigger = true;
                timeToBlink = Random.Range(4f, 10f);

                blinkRoutine = StartCoroutine(Blink());
            }
        }
    }

    private IEnumerator Blink()
    {
        ChangeEyeType(EyeType.Closed);
        yield return new WaitForSeconds(0.4f);
        ChangeEyeType(EyeType.Open);
        blinkTrigger = false;
        blinkRoutine = null;
    }
    void ChangeEyeType(EyeType newEyeType)
    {
        currentEyeType = newEyeType;
        skinnedMeshRenderer.material = eyeMaterials[(int)newEyeType];
    }

    public void SetEyesToHappy()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkTrigger = true;
        ChangeEyeType(EyeType.Happy);
    }

    public void ResetEyes()
    {
        blinkTrigger = false;
        ChangeEyeType(EyeType.Open);
    }

    public void SetEyesToMole()
    {
        blinkTrigger = true;
        ChangeEyeType(EyeType.Mole);
    }
}