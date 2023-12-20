using System;
using System.Collections.Generic;

// Written by Isabelle H. Heiskanen

[Serializable]
public class SCR_InventoryItem 
{
    public SCR_ItemData itemData;
    public int stackSize;
    private List<SCR_InventoryItem> inventoryList = new List<SCR_InventoryItem>();

    public SCR_InventoryItem(SCR_ItemData item)
    {
        itemData= item;
        AddToStack(inventoryList);
    }
    public void AddToStack(List<SCR_InventoryItem> inventoryList)
    {
        stackSize++;
    }
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
    }
}
