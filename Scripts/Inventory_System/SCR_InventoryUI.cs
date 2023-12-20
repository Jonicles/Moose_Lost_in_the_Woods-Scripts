using UnityEngine;
using TMPro;

// Written by Isabelle H. Heiskanen
public class SCR_InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coneAmountText;

    [SerializeField] TextMeshProUGUI plankAmountText;

    [SerializeField] GameObject badge01;
    [SerializeField] GameObject badge02;
    [SerializeField] GameObject badge03;

    private void OnEnable()
    {
        SCR_ConeInteractable.OnConesCollected += UpdateConeUI;
        SCR_PlanksInteractable.OnPlanksCollected += UpdatePlankUI;
    }

    private void OnDisable()
    {
        SCR_ConeInteractable.OnConesCollected -= UpdateConeUI;
        SCR_PlanksInteractable.OnPlanksCollected += UpdatePlankUI;
    }
    private void Awake()
    {
        LoadGameData();
    }

    public void UpdateConeUI(SCR_ItemData itemData)
    {
        coneAmountText.text = SCR_Inventory.coneAmount.ToString();
    }

    public void UpdatePlankUI(SCR_ItemData itemData)
    {
        plankAmountText.text = SCR_Inventory.planksAmount.ToString();
    }

    public void LoadGameData()
    {
        plankAmountText.text = SCR_Inventory.planksAmount.ToString();
        coneAmountText.text = SCR_Inventory.coneAmount.ToString();

        if (SCR_Badges.hasBadge01)
        {
            badge01.SetActive(true);
        }

        if (SCR_Badges.hasBadge02)
        {
            badge02.SetActive(true);
        }

        if (SCR_Badges.hasBadge03)
        {
            badge03.SetActive(true);
        }
    }


}
