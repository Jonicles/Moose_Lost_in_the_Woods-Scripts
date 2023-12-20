using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Written by Isabelle H. Heiskanen
// THIS SCRIPT NEEDS TO SIT ON THE CANVAS IN THE SCENE WITH THE INTERACT BUTTON 

public class SCR_PlayerInteractUI : MonoBehaviour, SCR_IButtonListener
{
    // Set Field Values in the inspector
    [Header("Fill in from Canvas")]
    [SerializeField] private GameObject interactButtonGameObject;
    [SerializeField] private Image interactButtonImage;
    [SerializeField] private SCR_PlayerInteract playerInteract;
    [SerializeField] private Image interactObjectImage;
    [SerializeField] private GameObject interactObject;

    SCR_PlayerInputActions playerInputActions;

    [Header("Sprites")]
    [SerializeField] private Sprite buttonToChangeToWhenPressed;
    [SerializeField] private Sprite deafultButtonSprite;

    private void Awake()
    {
        SCR_ControllerDetection controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);

        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.Player.Interact.Enable();
        playerInputActions.Player.Interact.performed += Interact;
        playerInputActions.Player.Interact.canceled += StopedInteract;
        interactButtonImage.sprite = deafultButtonSprite;

        playerInputActions.InventoryUI.SelectUI.performed += Interact;
        playerInputActions.InventoryUI.SelectUI.canceled += StopedInteract;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Interact.Disable();
        playerInputActions.Player.Interact.performed -= Interact;
        playerInputActions.Player.Interact.canceled -= StopedInteract;

        playerInputActions.InventoryUI.SelectUI.performed -= Interact;
        playerInputActions.InventoryUI.SelectUI.canceled -= StopedInteract;
    }
    private void Update()
    {
        if (playerInteract.GetInteractableObject() != null)
            Show();
        else
            Hide();

        if (SCR_DialogueManager.isConversationActive)
        {
            interactObject.SetActive(false);
        }
    }

    private void Show()
    {
        interactButtonGameObject.SetActive(true);
        interactObjectImage.sprite = playerInteract.GetObjectImage();
        interactObject.SetActive(true);
    }
    private void Hide()
    {
        interactButtonGameObject.SetActive(false);        
        interactObject.SetActive(false);
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        interactButtonImage.sprite = buttonToChangeToWhenPressed;
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
