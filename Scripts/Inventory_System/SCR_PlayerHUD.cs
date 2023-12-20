using UnityEngine;
using TMPro;

// Written by Isabelle H. Heiskanen
// SITS ON THE PLAYER HUD ON THE CANVASHUD
public class SCR_PlayerHUD : MonoBehaviour
{
    // Set in the inspector    
    [SerializeField] TextMeshProUGUI coneAmountText;


    private void Update()
    {
        coneAmountText.text = SCR_Inventory.coneAmount.ToString();
    }
}
