using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//John
public class SCR_MenuInput : MonoBehaviour
{
    int menuItemIndex = 0;
    int subItemIndex = 0;
    public bool IsInSubMenu { get; private set; }
    [SerializeField] float sliderStrength = 14f;
    [SerializeField] List<SCR_MenuItem> menuItemList = new List<SCR_MenuItem>();
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color hoverColor = Color.red;
    SCR_PlayerInputActions playerInputActions;

    private void Start()
    {
        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.MenuUI.MoveMenu.started += MoveCursor;
        playerInputActions.MenuUI.SelectMenu.started += SelectPanel;
        playerInputActions.MenuUI.GoBackMenu.performed += GoBack;
        menuItemList[menuItemIndex].Hover(hoverColor);
    }

    private void FixedUpdate()
    {
        Vector2 inputValue = playerInputActions.MenuUI.MoveMenu.ReadValue<Vector2>();
        float horizontalInput = inputValue.x;

        //We will only check this if horizontal value is bigger than vertical value
        if (Mathf.Abs(inputValue.y) > Mathf.Abs(horizontalInput))
            return;

        if (IsInSubMenu && Mathf.Abs(horizontalInput) > 0.125f)
        {
            List<SCR_MenuItem> tempSubMenuItems = menuItemList[menuItemIndex].GetSubItems();

            if (tempSubMenuItems.Count == 0)
                return;

            SCR_MenuItemSlider currentSlider;
            tempSubMenuItems[subItemIndex].gameObject.TryGetComponent<SCR_MenuItemSlider>(out currentSlider);

            if (currentSlider == null)
                return;

            currentSlider.ChangeValue(horizontalInput * Time.deltaTime * sliderStrength);
        }
    }
    public void ExitSubmenu()
    {
        List<SCR_MenuItem> tempSubMenuItems = menuItemList[menuItemIndex].GetSubItems();

        if (tempSubMenuItems.Count != 0)
        {
            tempSubMenuItems[subItemIndex].Leave(defaultColor);
        }

        menuItemList[menuItemIndex].Deselect();
        IsInSubMenu = false;
        IsInSubMenu = false;
        subItemIndex = 0;
    }

    private void GoBack(InputAction.CallbackContext context)
    {
        //If we are not in a submenu, we will just return since there is nothing to go back to.
        if (!IsInSubMenu)
            return;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Back");
        ExitSubmenu();
    }

    private void SelectPanel(InputAction.CallbackContext context)
    {
        //If you are not in submenu we will go into one
        if (!IsInSubMenu)
        {
            IsInSubMenu = true;
            menuItemList[menuItemIndex].Select();
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Select");

            List<SCR_MenuItem> tempSubMenuItems = menuItemList[menuItemIndex].GetSubItems();
            if (tempSubMenuItems.Count == 0)
                return;

            tempSubMenuItems[subItemIndex].Hover(hoverColor);
            return;
        }
        //We will select the subitme with the correct index if we indeed are in a submenu
        List<SCR_MenuItem> tempSubItems = menuItemList[menuItemIndex].GetSubItems();
        if (tempSubItems.Count == 0)
            return;

        tempSubItems[subItemIndex].Select();

        SCR_MenuItemSlider currentSlider;
        tempSubItems[subItemIndex].gameObject.TryGetComponent<SCR_MenuItemSlider>(out currentSlider);

        if (currentSlider == null)
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Select");
    }

    private void MoveCursor(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = playerInputActions.MenuUI.MoveMenu.ReadValue<Vector2>();

        if (Mathf.Abs(inputDirection.y) < 0.125f && Mathf.Abs(inputDirection.y) < Mathf.Abs(inputDirection.x))
            return;

        inputDirection.Normalize();
        if (inputDirection.y > 0)
        {
            //Up
            //If we are in a submenu we will look at the subitems indexes, if not we will go to through the main menu indexes
            if (IsInSubMenu)
            {
                if (subItemIndex == 0)
                    return;

                List<SCR_MenuItem> subMenuItems = menuItemList[menuItemIndex].GetSubItems();

                //If the submenu does not have any sub items, like for example the credits screen we will just return
                if (subMenuItems.Count == 0)
                    return;

                subMenuItems[subItemIndex].Leave(defaultColor);
                subItemIndex -= 1;
                subMenuItems[subItemIndex].Hover(hoverColor);

                return;
            }

            if (menuItemIndex == 0)
                return;

            menuItemList[menuItemIndex].Leave(defaultColor);
            menuItemIndex -= 1;
            menuItemList[menuItemIndex].Hover(hoverColor);
        }
        else
        {
            //Down
            //If we are in a submenu we will look at the subitems indexes, if not we will go to through the main menu indexes
            if (IsInSubMenu)
            {
                List<SCR_MenuItem> subMenuItems = menuItemList[menuItemIndex].GetSubItems();

                //If the submenu does not have any sub items, like for example the credits screen we will just return
                if (subMenuItems.Count == 0)
                    return;

                if (subItemIndex == subMenuItems.Count - 1)
                    return;

                subMenuItems[subItemIndex].Leave(defaultColor);
                subItemIndex += 1;
                subMenuItems[subItemIndex].Hover(hoverColor);

                return;
            }
            if (menuItemIndex == menuItemList.Count - 1)
                return;

            menuItemList[menuItemIndex].Leave(defaultColor);
            menuItemIndex += 1;
            menuItemList[menuItemIndex].Hover(hoverColor);
        }
    }

    private void OnDisable()
    {
        playerInputActions.MenuUI.MoveMenu.started -= MoveCursor;
        playerInputActions.MenuUI.SelectMenu.started -= SelectPanel;
        playerInputActions.MenuUI.GoBackMenu.performed -= GoBack;
        playerInputActions.MenuUI.Disable();
        playerInputActions.StoryBoard.Enable();
    }
}
