using System.Collections.Generic;
using UnityEngine;

// Written by Isabelle H. Heiskanen

// SITS ON THE INVENTORY PANEL ON THE CANVAS
public class SCR_InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] int slotAmount = 9;
    private List<SCR_InventorySlot> inventorySlots = new List<SCR_InventorySlot>(9);

    private void OnEnable()
    {
        SCR_Inventory.OnInventoryChange += DrawInventory;
    }
    private void OnDisable()
    {
        SCR_Inventory.OnInventoryChange -= DrawInventory;
    }


    private void ResetInventory(List<SCR_InventoryItem> inventory)
    {
        foreach (Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
        inventorySlots = new List<SCR_InventorySlot>(slotAmount);
    }
    public void DrawInventory(List<SCR_InventoryItem> inventory)
    {
        ResetInventory(inventory);

        for (int i = 0; i < slotAmount; i++)
        {
            CreateInventorySlots();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].DrawSlot(inventory[i]);
        }
    }

    private void CreateInventorySlots()
    {
        GameObject newSlot = Instantiate(slotPrefab);
        newSlot.transform.SetParent(transform, false);

        SCR_InventorySlot newSlotComponent = newSlot.GetComponent<SCR_InventorySlot>();
        newSlotComponent.ClearSlot();

        inventorySlots.Add(newSlotComponent);
    }

    public int GetSlotAmount()
    {
        return slotAmount;
    }
}
