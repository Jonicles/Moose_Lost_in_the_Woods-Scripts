using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Written by Isabelle H. Heiskanen
// SITS ON THE INVENTORY SLOT PREFAB
// IMPORTANT THAT PREFAB HAS ASSIGNED ALL THE PARAMETERS IN THE INSPECTOR
public class SCR_InventorySlot : MonoBehaviour
{
    public Image icon;    
    public TextMeshProUGUI stackSizeText;

    public void ClearSlot()
    {        
        stackSizeText.enabled = false;        
    }
    public void FillSlot()
    {        
        stackSizeText.enabled = true;        
    }
    public void DrawSlot(SCR_InventoryItem item)
    {
        if(item == null)
        {
            ClearSlot();
            return;
        }

        FillSlot();

        icon.sprite = item.itemData.icon;        
        stackSizeText.text = item.stackSize.ToString();
    }
}
