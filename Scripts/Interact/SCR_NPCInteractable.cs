using UnityEngine;
using Cinemachine;
using System.Collections;

// Written by Isabelle H. Heiskanen

// EVERY CHARACTER THAT YOU CAN INTERACT WITH NEED THIS SCRIPT ON IT
// DONT FORGET TO SET A CONVERSATION IN THE INSPECTOR
public class SCR_NPCInteractable : MonoBehaviour, SCR_IInteractable
{
    [Header("Conversation")]
    [SerializeField] SCR_Conversation conversation;
    private GameObject exclamationMark;
    private bool hasSpoken;

    [Header("Sprites")]
    [SerializeField] Sprite spriteBubble;

    [Header("Rotation Settings")]
    [SerializeField] AnimationCurve rotationCurve;
    [SerializeField] float rotationTime;
    Coroutine rotationRoutine;    

    private void Awake()
    {
        exclamationMark = GetComponent<SCR_SpeechBubble>().GetExclamationMark();
    }

    public void Interact()
    {
        FindObjectOfType<SCR_PlayerMovement>().StopAndInteract(true, transform);
        if (conversation != null && !SCR_DialogueManager.isConversationActive)
        {
            Transform playerTransform = SCR_CameraManager.Instance.GetPlayerTransform();
            Vector3 rotationDirection = (playerTransform.position - transform.position);
            rotationDirection = new Vector3(rotationDirection.x, 0, rotationDirection.z);
            rotationDirection.Normalize();

            if (rotationRoutine != null)
            {
                StopCoroutine(rotationRoutine);
            }
            rotationRoutine = StartCoroutine(RotateTowardPlayer(rotationDirection));

            
            SCR_DialogueManager.StartConversation(conversation);
            hasSpoken = true;
        }
    }
    IEnumerator RotateTowardPlayer(Vector3 rotationDirection)
    {
        Quaternion toRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
        Quaternion currentRotation = transform.rotation;

        float elapsedTime = 0;

        while (rotationTime > elapsedTime)
        {
            transform.rotation = Quaternion.Slerp(currentRotation, toRotation, rotationCurve.Evaluate(elapsedTime / rotationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SCR_CameraManager.Instance.ChooseCamera(GetComponentsInChildren<CinemachineVirtualCamera>());
        rotationRoutine = null;
        yield return null;
    }


    private void Update()
    {
        if (exclamationMark != null)
        {
            Animator animator = GetComponent<Animator>();

            if (!hasSpoken)
            {
                exclamationMark.SetActive(true);
                if (animator != null)
                {
                    animator.SetBool("hasSpoken", false);
                }
            }
            else
            {
                exclamationMark.SetActive(false);
                if (animator != null)
                {
                    animator.SetBool("hasSpoken", false);
                }
            }
        }

    }

    public Sprite GetSpriteIcon()
    {
        return spriteBubble;
    }
}
