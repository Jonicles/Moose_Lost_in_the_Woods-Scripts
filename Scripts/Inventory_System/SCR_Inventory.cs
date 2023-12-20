using System;
using System.Collections.Generic;
using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON AN GAMEOBJECT IN THE SCENE
public class SCR_Inventory : MonoBehaviour
{
    public static event Action<List<SCR_InventoryItem>> OnInventoryChange;

    [HideInInspector]
    public List<SCR_InventoryItem> hatInventoryList = new List<SCR_InventoryItem>();
    [HideInInspector]
    public Dictionary<SCR_ItemData, SCR_InventoryItem> hatDictionary = new Dictionary<SCR_ItemData, SCR_InventoryItem>();

    [HideInInspector]
    public List<SCR_InventoryItem> conesInventoryList = new List<SCR_InventoryItem>();
    [HideInInspector]
    public Dictionary<SCR_ItemData, SCR_InventoryItem> conesDictionary = new Dictionary<SCR_ItemData, SCR_InventoryItem>();

    [HideInInspector]
    public List<SCR_InventoryItem> planksInventoryList = new List<SCR_InventoryItem>();
    [HideInInspector]
    public Dictionary<SCR_ItemData, SCR_InventoryItem> planksDictionary = new Dictionary<SCR_ItemData, SCR_InventoryItem>();

    private SCR_PlayerInputActions playerInputActions;

    public static int coneAmount;
    public static int planksAmount;

    // Set in the inspector
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private SCR_InventoryUI SCR_inventoryUI;

    private void OnEnable()
    {
        SCR_PlanksInteractable.OnPlanksCollected += Add;
        SCR_ConeInteractable.OnConesCollected += Add;
        SCR_GlassesInteractable.OnGlassesCollected += Add;
        SCR_HatInteractable.OnHatCollected += Add;
    }

    private void OnDisable()
    {
        SCR_PlanksInteractable.OnPlanksCollected -= Add;
        SCR_ConeInteractable.OnConesCollected -= Add;
        SCR_GlassesInteractable.OnGlassesCollected -= Add;
        SCR_HatInteractable.OnHatCollected -= Add;

        playerInputActions.Player.Inventory.Disable();
        playerInputActions.Player.Inventory.performed -= OpenAndCloseInventory;

        playerInputActions.InventoryUI.InventoryCloseUI.performed -= OpenAndCloseInventory;
    }

    private void Awake()
    {
        inventoryUI.SetActive(false);
        playerInputActions = SCR_PlayerInputManager.Instance;
        playerInputActions.Player.Inventory.Enable();
        playerInputActions.Player.Inventory.performed += OpenAndCloseInventory;
        

        playerInputActions.InventoryUI.InventoryCloseUI.performed += OpenAndCloseInventory;

    }

    private void OpenAndCloseInventory(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inventoryUI.activeInHierarchy)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Inventory/Inventory_Open");
            inventoryUI.SetActive(true);
            OnInventoryChange?.Invoke(hatInventoryList);

            playerInputActions.Player.Disable();
            playerInputActions.InventoryUI.Enable();
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Inventory/Inventory_Close");
            inventoryUI.SetActive(false);

            playerInputActions.InventoryUI.Disable();
            playerInputActions.Player.Enable();
        }
    }

    public void Add(SCR_ItemData itemData)
    {
        // Check the normal item dictionary and make sure it is not a plank or cone
        if (hatDictionary.TryGetValue(itemData, out SCR_InventoryItem item))
        {
            if (itemData.displayName != "Cone" && itemData.displayName != "Planks")
            {
                item.AddToStack(hatInventoryList);
                OnInventoryChange?.Invoke(hatInventoryList);                
            }

        }
        // Check if item is in cone dictionary
        else if (conesDictionary.TryGetValue(itemData, out SCR_InventoryItem coneItem))
        {
            if (itemData.displayName == "Cone")
            {
                coneItem.AddToStack(conesInventoryList);
                coneAmount = coneItem.stackSize;
                SCR_inventoryUI.UpdateConeUI(itemData);                
            }


        }
        // check if item is in plankdictionary
        else if (planksDictionary.TryGetValue(itemData, out SCR_InventoryItem plankItem))
        {
            if (itemData.displayName == "Planks")
            {
                plankItem.AddToStack(planksInventoryList);
                planksAmount = plankItem.stackSize;
                SCR_inventoryUI.UpdatePlankUI(itemData);                
            }

        }
        else
        {
            SCR_InventoryItem newItem = new SCR_InventoryItem(itemData);


            if (itemData.displayName == "Cone")
            {
                conesInventoryList.Add(newItem);
                conesDictionary.Add(itemData, newItem);
                coneAmount = newItem.stackSize;
                SCR_inventoryUI.UpdateConeUI(itemData);                
            }
            else if (itemData.displayName == "Planks")
            {
                planksInventoryList.Add(newItem);
                planksDictionary.Add(itemData, newItem);
                planksAmount = newItem.stackSize;
                SCR_inventoryUI.UpdatePlankUI(itemData);                
            }
            else if (itemData.displayName != "Cone" && itemData.displayName != "Planks")
            {
                hatInventoryList.Add(newItem);
                hatDictionary.Add(itemData, newItem);
                OnInventoryChange?.Invoke(hatInventoryList);                
            }

        }

    }

    public void Remove(SCR_ItemData itemData, int amount)
    {

        // Check the normal itemList and make sure it is not a plank or cone
        if (hatDictionary.TryGetValue(itemData, out SCR_InventoryItem item))
        {
            if (itemData.displayName != "Cone" && itemData.displayName != "Planks")
            {
                item.RemoveFromStack(amount);

                if (item.stackSize == 0)
                {
                    hatInventoryList.Remove(item);
                    hatDictionary.Remove(itemData);
                }
                OnInventoryChange?.Invoke(hatInventoryList);
            }

        }
        // Check if item is in cone dictionary
        else if (conesDictionary.TryGetValue(itemData, out SCR_InventoryItem coneItem))
        {
            if (itemData.displayName == "Cone")
            {
                coneItem.RemoveFromStack(amount);

                if (coneItem.stackSize == 0)
                {
                    conesInventoryList.Remove(coneItem);
                    conesDictionary.Remove(itemData);
                }
                coneAmount = coneItem.stackSize;
                SCR_inventoryUI.UpdateConeUI(itemData);
            }


        }
        // check if item is in plankdictionary
        else if (planksDictionary.TryGetValue(itemData, out SCR_InventoryItem plankItem))
        {
            if (itemData.displayName == "Planks")
            {
                plankItem.RemoveFromStack(amount);

                if (plankItem.stackSize == 0)
                {
                    planksInventoryList.Remove(plankItem);
                    planksDictionary.Remove(itemData);
                }
                planksAmount = plankItem.stackSize;
                SCR_inventoryUI.UpdatePlankUI(itemData);
            }

        }

    }

    public bool HasAmountInInventory(SCR_ItemData itemData, int amount)
    {
        if (hatDictionary.TryGetValue(itemData, out SCR_InventoryItem item))
        {
            if (item.stackSize >= amount)
                return true;
            else
                return false;

        }
        if (conesDictionary.TryGetValue(itemData, out SCR_InventoryItem coneItem))
        {
            if (coneItem.stackSize >= amount)
                return true;
            else
                return false;

        }
        if (planksDictionary.TryGetValue(itemData, out SCR_InventoryItem plankItem))
        {
            if (plankItem.stackSize >= amount)
                return true;
            else
                return false;

        }
        else
            return false;
    }

    public List<SCR_InventoryItem> GetHatsInInventory()
    {
        return hatInventoryList;
    }

    public int ReturnAmountInInventory(SCR_ItemData itemData)
    {
        if (hatDictionary.TryGetValue(itemData, out SCR_InventoryItem item))
        {
            return item.stackSize;

        }
        if (conesDictionary.TryGetValue(itemData, out SCR_InventoryItem coneItem))
        {
            return coneItem.stackSize;

        }
        if (planksDictionary.TryGetValue(itemData, out SCR_InventoryItem plankItem))
        {
            return plankItem.stackSize;

        }
        else
            return 0;
    }

}
