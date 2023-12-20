using UnityEngine;
// Written by Isabelle H. Heiskanen
// THIS SCRIPT NEEDS TO SIT ON THE PLAYER
public class SCR_PlayerInteract : MonoBehaviour
{
    SCR_PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions= SCR_PlayerInputManager.Instance;
        playerInputActions.Player.Interact.Enable();
        playerInputActions.Player.Interact.performed += Interact;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Interact.Disable();
        playerInputActions.Player.Interact.performed -= Interact;
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // If the player is in the area and finds an interactable object it sends it to the correct interact method
        if (!SCR_DialogueManager.isConversationActive && !SCR_PlanksInteractable.isPickingUpPlanks && !SCR_GlassesInteractable.isPickingUpGlases)
        {
            SCR_IInteractable interactable = GetInteractableObject();
            if (interactable != null)
            {
                interactable.Interact();
            }                
        }
    }
    // Finds the interactable object in the interact area. Looking for the component of the interface IInteractable
    public SCR_IInteractable GetInteractableObject()
    {
        float interactRange = 3f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out SCR_IInteractable interactable))
            {
                return interactable;
            }
        }
        return null;
    }

    // Finds the interactable object in the interact area. Then gets the sprite for the interactable object
    public Sprite GetObjectImage()
    {
        float interactRange = 3f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out SCR_IInteractable interactable))
            {
                return interactable.GetSpriteIcon();
            }
        }
        return null;
    }


}
