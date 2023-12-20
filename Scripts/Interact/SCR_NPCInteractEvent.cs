using System.Collections;
using UnityEngine;
using Cinemachine;

// Written by Isabelle H. Heiskanen
// SITS ON THE NPC THAT WILL DO AN EVENT
public class SCR_NPCInteractEvent : MonoBehaviour, SCR_IInteractable
{
    [Header("Conversations")]
    [SerializeField] SCR_Conversation conversation;         // MUST HAVE
    [SerializeField] SCR_Conversation eventConversation;    // MUST HAVE
    [SerializeField] SCR_Conversation GiveItemConversation; // DONT NEED WHEN TO TRIGGER EVENT EARLY
    [SerializeField] SCR_Conversation lastConversation;     // DONT NEED WHEN TO TRIGGER EVENT EARLY

    [Header("Event information")]
    [SerializeField] int itemAmount;                        // DONT NEED IF NO ITEM IS TO BE GIVEN TO NPC
    [SerializeField] SCR_ItemData itemDataToGive;           // DONT NEED IF NO ITEM IS TO BE GIVEN TO NPC            
    [SerializeField] SCR_ItemData[] itemDataToRecieve;      // DONT NEED IF ITEM SHOULD NOT BE GIVEN
    [SerializeField] GameObject[] eventObjectToSetActive;   // DONT NEED IF EVENT IS NOT NEEDED
    [SerializeField] GameObject[] eventObjectToHide;        // DONT NEED IF EVENT IS NOT NEEDED
    [SerializeField] GameObject badgeObject;                // DONT NEED IF NO BADGE SHOULD BE GIVEN
    [SerializeField] int timerToStartEvent;                 // DONT NEED IF NO TIMER IS NEEDED
    [SerializeField] GameObject teleportSpot;               // DONT NEED IF NO TELEPORT IS NEEDED
    private SCR_Inventory inventory;
    private SCR_TeleportPlayer teleport;
    private GameObject exclamationMark;
    private SCR_CharacterEyeChanger eyeChanger;

    private bool hasSpoken;
    private bool hasSpokenFridaAndFrog;
    private bool hasSpokenToOlle;
    private bool turnOfExclamationMarkOlle;
    private bool hasDoneEvent;
    
    //These bools are only to prevent EyeChanger being called multiple times in update
    bool hasSetEyesToHappy;
    bool hasResetEyes;

    [Header("Sprites")]
    [SerializeField] Sprite spriteBubble;                   // NEEDS TO NOT HAVE AN EMPTY SPRITE WHEN PLAYER IN RANGE TO TALK TO NPC

    [Header("Rotation Settings")]
    [SerializeField] AnimationCurve rotationCurve;          // DONT NEED IF NO CURVE IS TO BE SET
    [SerializeField] float rotationTime;                    // DONT NEED IF NO TIME IS TO BE SET
    Coroutine rotationRoutine;

    private void Awake()
    {
        inventory = FindObjectOfType<SCR_Inventory>();
        teleport = GetComponent<SCR_TeleportPlayer>();
        exclamationMark = GetComponent<SCR_SpeechBubble>().GetExclamationMark();
        eyeChanger = GetComponentInChildren<SCR_CharacterEyeChanger>();
    }

    private void Start()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(true);
        }
    }

    public void Interact()
    {
        FindObjectOfType<SCR_PlayerMovement>().StopAndInteract(true, transform);

        // Conversation to trigger event after first conversation
        if (eventConversation == null && GiveItemConversation == null && lastConversation == null && !SCR_DialogueManager.isConversationActive && !hasSpoken)
        {
            Rotate();
            if (gameObject.name == "Frida" || gameObject.name == "Frog" || gameObject.name == "Frida02")
            {
                hasSpokenFridaAndFrog = true;                
            }
            SCR_DialogueManager.StartConversation(conversation);
            StartCoroutine(TimerToStartEvent());
        }

        // Conversation to trigger event earlier
        if (GiveItemConversation == null && lastConversation == null && !SCR_DialogueManager.isConversationActive && hasSpoken)
        {
            Rotate();
            if (gameObject.name == "Olle")
            {
                turnOfExclamationMarkOlle = true;
            }            
            SCR_DialogueManager.StartConversation(eventConversation);
            StartCoroutine(TimerToStartEvent());
        }


        // Conversation when the player has the right amount of cones in inventory
        if (GiveItemConversation != null && !SCR_DialogueManager.isConversationActive && !hasDoneEvent && hasSpoken && GetItemAmount())
        {
            Rotate();
            SCR_DialogueManager.StartConversation(GiveItemConversation);
            inventory.Remove(itemDataToGive, itemAmount);
            StartCoroutine(TimerToStartEvent());
            hasDoneEvent = true;
        }

        // Last conversation after the player has gotten the hat
        if (hasDoneEvent && !SCR_DialogueManager.isConversationActive)
        {
            Rotate();
            SCR_DialogueManager.StartConversation(lastConversation);
        }

        // Conversation when the player has spoken with the NPC but does not have the right amount of cones in the inventory
        if (eventConversation != null && !SCR_DialogueManager.isConversationActive && !hasDoneEvent && hasSpoken && !GetItemAmount())
        {
            Rotate();
            SCR_DialogueManager.StartConversation(eventConversation);
        }

        // Start conversation with NPC
        if (conversation != null && !SCR_DialogueManager.isConversationActive && !hasDoneEvent && !hasSpoken)
        {
            Rotate();
            SCR_DialogueManager.StartConversation(conversation);
            hasSpoken = true;
            if (gameObject.name == "Olle03")
            {
                gameObject.GetComponent<Animator>().SetBool("hasSpoken", true);
                gameObject.GetComponentInChildren<SCR_CharacterEyeChanger>().ResetEyes();
                StartCoroutine(TimerToStartEvent());
            }
            if (gameObject.name == "Olle")
            {
                hasSpokenToOlle = true;
            }
        }

        // Last convo when only start and last excist
        if (eventConversation == null && GiveItemConversation == null && !SCR_DialogueManager.isConversationActive && hasSpoken)
        {
            Rotate();
            SCR_DialogueManager.StartConversation(lastConversation);
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


    // Method to check if the player has the right amount of cones in inventory
    public bool GetItemAmount()
    {
        if (inventory.HasAmountInInventory(itemDataToGive, itemAmount))
            return true;

        else
            return false;
    }

    private void DoEvent()
    {
        if (eventObjectToSetActive != null)
        {
            for (int i = 0; i < eventObjectToSetActive.Length; i++)
            {
                eventObjectToSetActive[i].SetActive(true);
            }

        }

        if (eventObjectToHide != null)
        {
            for (int i = 0; i < eventObjectToHide.Length; i++)
            {
                eventObjectToHide[i].SetActive(false);
            }

        }

        if (itemDataToRecieve != null)
        {

            for (int i = 0; i < itemDataToRecieve.Length; i++)
            {
                inventory.Add(itemDataToRecieve[i]);
            }

        }

        // Set the badge to correct badge when event is done
        if (badgeObject != null)
        {
            if (badgeObject.name == "Badge01")
            {
                SCR_Badges.hasBadge01 = true;
                badgeObject.SetActive(true);
            }
            if (badgeObject.name == "Badge02")
            {
                SCR_Badges.hasBadge02 = true;
                badgeObject.SetActive(true);
            }
            if (badgeObject.name == "Badge03")
            {
                SCR_Badges.hasBadge03 = true;
                badgeObject.SetActive(true);
            }

        }
    }

    IEnumerator TimerToStartEvent()
    {

        if (teleport != null)
        {
            teleport.TeleportPlayer(teleportSpot);
        }

        yield return new WaitForSeconds(timerToStartEvent);
        DoEvent();

        if (gameObject.name == "Olle")
        {
            SCR_DialogueManager.ResetConversation();            
        }
    }

    private void Update()
    {
        if (exclamationMark != null)
        {
            if (!hasSpoken)
            {
                exclamationMark.SetActive(true);
            }
            else
            {
                exclamationMark.SetActive(false);
            }
        }

        if (exclamationMark != null && (gameObject.name == "Frida" || gameObject.name == "Frog" || gameObject.name == "Frida02"))
        {
            if (!hasSpokenFridaAndFrog)
            {
                exclamationMark.SetActive(true);
            }
            else
            {
                exclamationMark.SetActive(false);
            }
        }

        if (itemDataToGive != null)
        {
            if (!GetItemAmount())
            {
                Animator animator = GetComponent<Animator>();
                if (animator != null && gameObject.name != "Sune")
                {
                    animator.SetBool("hasItems", false);
                }
            }

            if (GetItemAmount())
            {
                Animator animator = GetComponent<Animator>();
                if (animator != null && gameObject.name != "Sune")
                {
                    animator.SetBool("hasItems", true);
                }

                if (animator != null && gameObject.name == "Sune" && SCR_Badges.hasBadge02)
                {
                    animator.SetBool("hasItems", true);
                }

                if (eyeChanger != null && !hasSetEyesToHappy)
                {
                    hasSetEyesToHappy = true;
                    eyeChanger.SetEyesToHappy();
                }
            }
        }

        if (hasDoneEvent)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("hasItems", false);
            }

            if (eyeChanger != null && !hasResetEyes)
            {
                hasResetEyes = true;
                eyeChanger.ResetEyes();
            }
        }

        if (hasSpoken && !SCR_DialogueManager.isConversationActive && !hasDoneEvent || 
            hasSpokenFridaAndFrog && !SCR_DialogueManager.isConversationActive && !hasDoneEvent)
        {
            SCR_ThoughtBubble thoughtBubble = GetComponent<SCR_ThoughtBubble>();
            if (thoughtBubble != null)
                thoughtBubble.Activate();
        }
        else
        {
            SCR_ThoughtBubble thoughtBubble = GetComponent<SCR_ThoughtBubble>();
            if (thoughtBubble != null)
                thoughtBubble.Deactivate();
        }

        if (hasSpokenToOlle && gameObject.name == "Olle" && !SCR_DialogueManager.isConversationActive)
        {
            exclamationMark.SetActive(true);          
            
        }
        if (turnOfExclamationMarkOlle && gameObject.name == "Olle" && !SCR_DialogueManager.isConversationActive)
        {
            exclamationMark.SetActive(false);
        }
    }

    public Sprite GetSpriteIcon()
    {
        return spriteBubble;
    }

    public int GetItemAmountNeeded()
    {
        return itemAmount;
    }

    public SCR_ItemData itemToGiveToNPC()
    {
        return itemDataToGive;
    }

    public int ReturnAmountInInventory()
    {
        return inventory.ReturnAmountInInventory(itemDataToGive);
    }

    private void Rotate()
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
    }
}
