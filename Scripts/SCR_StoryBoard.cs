using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Written by Isabelle H. Heiskanen
// THIS SCRIPTS NEEDS TO SIT ON AN EMPTY GAMEOBJECT IN SCENE
public class SCR_StoryBoard : MonoBehaviour, SCR_IButtonListener
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

    private void Update()
    {
        if (storyBoardCanvas.activeInHierarchy)
        {
            playerInputActions.Player.Disable();
            //playerInputActions.InventoryUI.SelectUI.Enable();
        }
    }

    private void CloseCanvas()
    {
        storyBoardCanvas.GetComponent<Animator>().SetBool("isClosing", true);        
        StartCoroutine(MoveCanvas()); 
    }

    IEnumerator MoveCanvas()
    {
        playerInputActions.StoryBoard.Disable();
        yield return new WaitForSeconds(2f);
        storyBoardCanvas.SetActive(false);
        playerInputActions.Player.Enable();
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
