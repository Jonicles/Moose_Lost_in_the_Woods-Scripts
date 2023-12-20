using System.Collections;
using UnityEngine;


// Written by Isabelle H. Heiskanen

// EVERY CHARACTER THAT WILL HAVE A SPEECHBUBBLE NEEDS THIS SCRIPT ON IT
// Set the field values in the inspector
public class SCR_SpeechBubble : MonoBehaviour
{
    // Importnant to set these in the inspector !!
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] Animator bubbleAnimator;
    bool isActive;

    Coroutine activationRoutine;
    public GameObject GetSpeechBubble()
    {
        return speechBubble;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public GameObject GetExclamationMark()
    {
        return exclamationMark;
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

        speechBubble.SetActive(true);
        bubbleAnimator.Play("AC_Bubble_Appear");
        isActive = true;
    }

    public void Bob()
    {
        bubbleAnimator.Play("AC_Bubble_Bob");
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
        speechBubble.SetActive(false);
    }


}
