using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//John
//Sits on inventory prefab
public class SCR_InventoryInput : MonoBehaviour
{
    [SerializeField] SCR_InventoryHighlights inventoryHighlights;
    [SerializeField] int inventoryRowSize = 4;
    [SerializeField] int inventoryRowCount = 3;
    [SerializeField] int inventorySpaces = 10;

    int currentSpaceX = 0;
    int currentSpaceY = 0;
    SCR_PlayerInputActions playerInputActions;
    SCR_Inventory inventory;
    SCR_HatChanger hatChanger;
    GameObject currentHat;

    List<SCR_InventoryItem> currentInventory = new List<SCR_InventoryItem>();

    private void Awake()
    {
        //inventoryHighlights = GameObject.Find("Inventory Highlights").GetComponent<SCR_InventoryHighlights>();
        inventory = GameObject.Find("P_Inventory").GetComponent<SCR_Inventory>();
        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.InventoryUI.MoveUI.started += MoveCursor;
        playerInputActions.InventoryUI.SelectUI.performed += SelectHat;

        hatChanger = FindAnyObjectByType<SCR_HatChanger>();
        currentInventory = inventory.GetHatsInInventory();

        if (PlayerPrefs.HasKey("CurrentHatPlayerPref"))
        {
            SetCurrentHat(PlayerPrefs.GetString("CurrentHatPlayerPref"));
        }
    }

    private void OnDisable()
    {
        playerInputActions.InventoryUI.MoveUI.started -= MoveCursor;
        playerInputActions.InventoryUI.SelectUI.performed -= SelectHat;
    }

    private void SelectHat(InputAction.CallbackContext context)
    {
        //List<SCR_InventoryItem> currentInventory = inventory.GetHatsInInventory();

        //This means that the spot is empty in which we will not continue the method
        if (currentInventory.Count - 1 < (currentSpaceX + (currentSpaceY * inventoryRowSize)))
            return;

        //Found the right hat
        //print(currentInventory[currentSpaceX + (currentSpaceY * inventoryRowSize)].itemData.displayName);

        foreach (GameObject hat in hatChanger.hats)
        {
            if (currentInventory[currentSpaceX + (currentSpaceY * inventoryRowSize)].itemData.displayName == hat.name)
            {
                if (hat.activeInHierarchy)
                {
                    hat.SetActive(false);
                    currentHat = null;
                }
                else
                {
                    hat.SetActive(true);
                    currentHat = hat;
                }
            }
            if (currentInventory[currentSpaceX + (currentSpaceY * inventoryRowSize)].itemData.displayName != hat.name)
            {
                hat.SetActive(false);
            }
        }
    }

    private void MoveCursor(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = playerInputActions.InventoryUI.MoveUI.ReadValue<Vector2>();
        inputDirection.Normalize();

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            if (inputDirection.x > 0)
            {
                //Right
                if (currentSpaceX == inventoryRowSize - 1 || inventorySpaces - 1 < currentSpaceX + 1 + (currentSpaceY * inventoryRowSize))
                    return;
                currentSpaceX++;
            }
            else
            {
                //Left
                if (currentSpaceX == 0)
                    return;
                currentSpaceX--;
            }
        }
        else
        {
            if (inputDirection.y > 0)
            {
                //Up
                if (currentSpaceY == 0)
                    return;
                currentSpaceY--;
            }
            else
            {
                //Down
                if (currentSpaceY == inventoryRowCount - 1 || inventorySpaces - 1 < currentSpaceX + ((currentSpaceY + 1) * inventoryRowSize))
                    return;
                currentSpaceY++;
            }
        }

        inventoryHighlights.ActivateHighlight(currentSpaceX + (currentSpaceY * inventoryRowSize));
    }

    public string GetCurrentHat()
    {
        if (currentHat == null)
            return "";

        return currentHat.name;
    }

    public void SetCurrentHat(string newHat)
    {
        foreach (GameObject hats in hatChanger.hats)
        {
            if (newHat == hats.name)
            {
                if (hats.name == "Glases")
                    return;

                hats.SetActive(true);
                currentHat = hats;
            }
            else
            {
                hats.SetActive(false);
            }
        }
    }
}
