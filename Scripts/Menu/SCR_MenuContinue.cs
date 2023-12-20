using UnityEngine.InputSystem;

// Written by Isabelle H. Heiskanen

public class SCR_MenuContinue : SCR_MenuItem
{
    SCR_PausMenu pausmenu;

    private SCR_MenuInput menuInput;
    SCR_PlayerInputActions playerInputActions;
    private void Awake()
    {
        pausmenu = FindAnyObjectByType<SCR_PausMenu>();
        menuInput = FindAnyObjectByType<SCR_MenuInput>();   
    }

    private void OnEnable()
    {
        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.MenuUI.GoBackMenu.started += GoBackMenu_started;
    }

    private void OnDisable()
    {
        playerInputActions.MenuUI.GoBackMenu.started -= GoBackMenu_started;
    }

    private void GoBackMenu_started(InputAction.CallbackContext context)
    {
        if (!FindAnyObjectByType<SCR_MenuInput>().IsInSubMenu)
        {
            Select();
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Select");
        }
    }

    public override void Select()
    {
        menuInput.ExitSubmenu();
        pausmenu.ClosePausMenu();
        SCR_DialogueManager.ResetConversation();
    }

    public override void Deselect()
    {

    }

}
