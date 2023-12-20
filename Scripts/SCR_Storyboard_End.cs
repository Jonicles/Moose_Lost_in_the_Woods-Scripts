using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Written by Isabelle H. Heiskanen
// SITS ON THE OBJECT THAT THE PLAYER GOES THROUGH TO ACTIVATE THE STORY BOARD
public class SCR_Storyboard_End : MonoBehaviour, SCR_IButtonListener
{
    [Header("Fill in from Canvas")]
    [SerializeField] private Image interactButtonImage;
    [SerializeField] private Image convoImage;
    [SerializeField] private GameObject storyBoardCanvas;
    [SerializeField] private GameObject[] storyBoardImages;

    [Header("Sprites")]
    [SerializeField] private Sprite buttonToChangeToWhenPressed;
    [SerializeField] private Sprite deafultButtonSprite;
    [SerializeField] private Sprite activeConvoSprite;
    [SerializeField] private Sprite lastConvoSprite;

    private SCR_PlayerInputActions playerInputActions;
    private int currentImageIndex;

    Coroutine storyboardRoutine;

    private void Awake()
    {
        SCR_ControllerDetection controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);

        playerInputActions = SCR_PlayerInputManager.Instance;

        playerInputActions.StoryBoard.Continue.performed += Interact;
        playerInputActions.StoryBoard.Continue.canceled += StopedInteract;

        convoImage.sprite = activeConvoSprite;
        currentImageIndex = 1;
    }

    private void OnDisable()
    {
        playerInputActions.StoryBoard.Continue.performed -= Interact;
        playerInputActions.StoryBoard.Continue.canceled -= StopedInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (storyboardRoutine == null)
                storyboardRoutine = StartCoroutine(Storyboardla());
        }
    }

    IEnumerator Storyboardla()
    {
        playerInputActions.Player.Disable();
        SCR_FadeScreen.Fade();
        yield return new WaitForSeconds(2);
        storyBoardCanvas.SetActive(true);
        SCR_FadeScreen.FadeIn();
        yield return new WaitForSeconds(1.5f);
        playerInputActions.StoryBoard.Enable();
    }


    private void CloseCanvas()
    {
        playerInputActions.StoryBoard.Disable();
        StartCoroutine(MoveCanvas());
    }

    IEnumerator MoveCanvas()
    {
        yield return new WaitForSeconds(0.5f);

        SCR_FadeScreen.Fade();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(4);
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/NPC/Dialogue_Continue");

        interactButtonImage.sprite = buttonToChangeToWhenPressed;

        // Important to not get a null error when a conversation has ended
        if (currentImageIndex > storyBoardImages.Length - 1)
        {
            CloseCanvas();
            return;
        }

        // Last sprite is showing
        if (currentImageIndex == storyBoardImages.Length - 1)
        {
            convoImage.sprite = lastConvoSprite;
        }

        storyBoardImages[currentImageIndex].SetActive(true);

        currentImageIndex++;
    }

    private void StopedInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        interactButtonImage.sprite = deafultButtonSprite;
    }

    public void ChangeSprites(List<Sprite> currentSprites)
    {
        deafultButtonSprite = currentSprites[1];
        interactButtonImage.sprite = currentSprites[1];
        buttonToChangeToWhenPressed = currentSprites[2];

    }
}
