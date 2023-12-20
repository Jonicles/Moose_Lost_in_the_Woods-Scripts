using System.Collections;
using UnityEngine;
using TMPro;


// Written by Isabelle H. Heiskanen

// EVERY CHARACTER THAT WILL HAVE A THOUGHTBUBBLE NEEDS THIS SCRIPT ON IT
// Set the field values in the inspector
public class SCR_ThoughtBubble : MonoBehaviour
{
    // Importnant to set these in the inspector !!
    [Header("Thought Bubble")]
    public GameObject thoughtBubble;
    [SerializeField] private TextMeshProUGUI TextAmountHas;
    [SerializeField] private TextMeshProUGUI TextAmountNeeded;
    [SerializeField] private SpriteRenderer icon;

    [Header("Certificat if needed")]
    [SerializeField] private GameObject certificatObject;
    [SerializeField] private TextMeshProUGUI TextAmountHasCertificatBuild;

    private SCR_NPCInteractEvent NPCevent;
    [SerializeField] Animator bubbleAnimator;
    Coroutine activationRoutine;
    bool isActive; 

    private void Awake()
    {
        NPCevent = GetComponent<SCR_NPCInteractEvent>();

        if (TextAmountNeeded != null)
        {
            TextAmountNeeded.text = NPCevent.GetItemAmountNeeded().ToString();
        }

        if (icon != null)
        {
            icon.sprite = NPCevent.itemToGiveToNPC().icon;
        }

    }

    public void Activate()
    {
        if (isActive || bubbleAnimator == null)
            return;

        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            activationRoutine = null;
        }

        thoughtBubble.SetActive(true);
        bubbleAnimator.Play("AC_Bubble_Appear");
        isActive = true;
    }

    public void Deactivate()
    {
        if (!isActive || bubbleAnimator == null)
            return;

        bubbleAnimator.Play("AC_Bubble_Disappear");

        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            activationRoutine = null;
        }

        activationRoutine = StartCoroutine(SetBubbleToInactive());
        isActive = false;
    }

    IEnumerator SetBubbleToInactive()
    {
        yield return new WaitForSeconds(0.200f);
        activationRoutine = null;
        thoughtBubble.SetActive(false);
    }

    private void Update()
    {
        if (TextAmountHas != null)
        {
            TextAmountHas.text = NPCevent.ReturnAmountInInventory().ToString();
        }


        if (certificatObject != null)
        {
            certificatObject.SetActive(true);

            if (gameObject.name == "Sune")
            {
                if (SCR_Badges.hasBadge02)
                {
                    TextAmountHasCertificatBuild.text = "1";
                }
                else
                {
                    TextAmountHasCertificatBuild.text = "0";
                }
            }

        }
    }
}
