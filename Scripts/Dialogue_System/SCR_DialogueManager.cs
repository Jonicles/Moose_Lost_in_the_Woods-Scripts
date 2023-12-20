using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Written by Isabelle H. Heiskanen
// THIS SCRIPTS NEEDS TO SIT ON AN EMPTY GAMEOBJECT IN SCENE
public class SCR_DialogueManager : MonoBehaviour
{
    // Fill in, in the inspector
    [Header("All Characters in scene that are interactable")]
    [SerializeField] GameObject[] characterArray;

    // Fill in, in the inspector
    [Header("Sprites")]
    [SerializeField] GameObject InteractObjectConvo;
    [SerializeField] Image InteractObjectConvoImage;
    [SerializeField] Sprite spriteActiveConversation;
    [SerializeField] Sprite spriteLastConversation;

    [Header("Rockland Vibe Check")]
    [SerializeField] bool isRockland;

    // Reference to get the names and the correct dialogue line
    private int currentConversationIndex;
    private string currentSpeaker;
    private SCR_Conversation currentConversation;
    private Sprite currentDialogueSprite;
    private SpriteRenderer spriteOnCharacter;
    private SCR_PlayerInputActions playerInputActions;

    public static bool isConversationActive;
    public static bool isConversationStarted = false;
    public static bool isConversationOnLastSprite;

    // Singelton reference
    private static SCR_DialogueManager instance;

    Coroutine coolDownRoutine;

    //Speaking stuff
    Coroutine speakingRoutine;
    [Header("Speaking Sound Setting")]
    [SerializeField] float speakingTimeLow;
    [SerializeField] float speakingTimeHigh;
    [SerializeField] float betweenTimeLow;
    [SerializeField] float betweenTimeHigh;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.Player.Interact.Enable();
        playerInputActions.Player.Interact.performed += InteractPressed;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Interact.Disable();
        playerInputActions.Player.Interact.performed -= InteractPressed;
    }

    public static void StartConversation(SCR_Conversation conversation)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/NPC/Dialogue_Bubble_Appear");

        instance.playerInputActions.Player.Move.Disable();
        instance.playerInputActions.Player.Jump.Disable();
        instance.playerInputActions.Player.Inventory.Disable();
        instance.playerInputActions.Player.Sprint.Disable();
        instance.playerInputActions.Player.Look.Disable();
        instance.playerInputActions.Player.ResetCamera.Disable();

        isConversationStarted = true;

        // When starting conversation, make sure everything is set to 0 and no text
        instance.currentConversationIndex = 0;
        instance.currentConversation = conversation;
        instance.currentDialogueSprite = null;
        isConversationActive = true;
        isConversationOnLastSprite = false;
        instance.InteractObjectConvoImage.sprite = instance.spriteActiveConversation;
        instance.InteractObjectConvo.SetActive(true);
        // Read the correct Line of Dialogue depending on the index
        instance.ReadNext();

    }

    // Resets the whole conversation
    public static void ResetConversation()
    {
        instance.playerInputActions.Player.Move.Enable();
        instance.playerInputActions.Player.Jump.Enable();
        instance.playerInputActions.Player.Inventory.Enable();
        instance.playerInputActions.Player.Sprint.Enable();
        instance.playerInputActions.Player.Look.Enable();
        instance.playerInputActions.Player.ResetCamera.Enable();

        isConversationStarted = false;

        instance.currentConversationIndex = 0;
        instance.currentConversation = null;
        instance.currentDialogueSprite = null;
        isConversationActive = false;
        isConversationOnLastSprite = false;
        instance.InteractObjectConvo.SetActive(false);
        SCR_CameraManager.Instance.ResetToWorldCamera();

        foreach (GameObject gameObject in instance.characterArray)
        {
            gameObject.GetComponent<SCR_SpeechBubble>().Deactivate();

        }
    }

    private void InteractPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (isConversationActive)
        {
            ReadNext();
        }
    }

    private void ReadNext()
    {
        if (isRockland)
        {
            isConversationStarted = false;
        }

        // Important to not get a null error when a conversation has ended
        if (currentConversationIndex > currentConversation.GetLength())
        {
            foreach (GameObject gameObject in characterArray)
            {
                gameObject.GetComponent<SCR_SpeechBubble>().Deactivate();
            }

            if (coolDownRoutine == null)
            {
                instance.playerInputActions.Player.Move.Enable();
                instance.playerInputActions.Player.Jump.Enable();
                instance.playerInputActions.Player.Inventory.Enable();
                instance.playerInputActions.Player.Sprint.Enable();
                instance.playerInputActions.Player.Look.Enable();
                instance.playerInputActions.Player.ResetCamera.Enable();


                FMODUnity.RuntimeManager.PlayOneShot("event:/NPC/Dialogue_Exit");

                if (speakingRoutine != null)
                {
                    StopCoroutine(speakingRoutine);
                }

                SCR_CameraManager.Instance.ResetToWorldCamera();
                coolDownRoutine = StartCoroutine(CoolDownForDialogue());
                isConversationOnLastSprite = false;
                instance.InteractObjectConvo.SetActive(false);
            }
            return;
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/NPC/Dialogue_Continue");

        if (currentConversationIndex == currentConversation.GetLength())
        {
            isConversationOnLastSprite = true;
            InteractObjectConvoImage.sprite = spriteLastConversation;
        }

        string lastSpeaker = currentSpeaker;
        // Gets the name of the current speaker
        currentSpeaker = currentConversation.GetDialogueLineByIndex(currentConversationIndex).speaker.GetName();

        if (lastSpeaker == currentSpeaker && currentConversationIndex != 0)
        {
            foreach (GameObject gameObject in instance.characterArray)
            {
                if (gameObject.name == currentSpeaker)
                {
                    gameObject.GetComponent<SCR_SpeechBubble>().Bob();
                }
            }
        }

        if (speakingRoutine != null)
            StopCoroutine(speakingRoutine);

        string speakerName = currentSpeaker;
        speakerName = ConvertSpeakerNameToEventName(speakerName);

        if (speakerName != "")
            speakingRoutine = StartCoroutine(Speak(speakerName));

        // Gets the current dialogue sprite
        currentDialogueSprite = currentConversation.GetDialogueLineByIndex(currentConversationIndex).dialogueSprite;

        GetSpeaker();

        if (!isConversationStarted)
            currentConversationIndex++;

        isConversationStarted = false;
    }

    private string ConvertSpeakerNameToEventName(string nameInScene)
    {
        if (nameInScene == "P_Player")
        {
            return "Player";
        }

        if (nameInScene == "Bear" || nameInScene == "BearArea01")
        {
            return "Bear";
        }

        if (nameInScene == "RockyArea01" || nameInScene == "RockyArea01_02" || nameInScene == "Sune" || nameInScene == "Frida" || nameInScene == "Frida02" || nameInScene == "Frida03")
        {
            return "Beaver";
        }

        if (nameInScene == "Rocky" || nameInScene == "Mole")
        {
            return "Mole";
        }

        if (nameInScene == "Olle" || nameInScene == "Olle02" || nameInScene == "Olle03" || nameInScene == "Woodpecker")
        {
            return "Bird";
        }

        if (nameInScene == "Frog02" || nameInScene == "Frog")
        {
            return "Frog";
        }

        return "";
    }

    IEnumerator Speak(string speakerName)
    {
        float elapsedTime = 0;

        float elapsedbetweenTime = 10f;
        float currentBetweenTime = Random.Range(betweenTimeLow, betweenTimeHigh);
        float currentSpeakingTime = Random.Range(speakingTimeLow, speakingTimeHigh);

        while (currentSpeakingTime > elapsedTime)
        {
            if (elapsedbetweenTime > currentBetweenTime)
            {
                FMODUnity.RuntimeManager.PlayOneShot($"event:/NPC/{speakerName}_Voice");
                currentBetweenTime = Random.Range(betweenTimeLow, betweenTimeHigh);
                elapsedbetweenTime = 0;
            }
            elapsedbetweenTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        speakingRoutine = null;
    }

    public void GetSpeaker()
    {
        foreach (GameObject gameObject in characterArray)
        {

            if (gameObject.name != currentSpeaker)
            {
                gameObject.GetComponent<SCR_SpeechBubble>().Deactivate();
            }


            spriteOnCharacter = gameObject.GetComponent<SCR_SpeechBubble>().GetSpriteRenderer();

            if (gameObject.name == currentSpeaker)
            {
                gameObject.GetComponent<SCR_SpeechBubble>().Activate();

                spriteOnCharacter.sprite = currentDialogueSprite;
            }
        }
    }

    private IEnumerator CoolDownForDialogue()
    {
        yield return new WaitForSeconds(1f);
        isConversationActive = false;
        coolDownRoutine = null;
    }
}

