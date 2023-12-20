using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//John
//Sits on canvas inventory highlight panel
public class SCR_InventoryHighlights : MonoBehaviour, SCR_IButtonListener
{
    [SerializeField] GameObject highlightPrefab;
    [SerializeField] List<Image> buttonList = new List<Image>();
    List<GameObject> highlightList = new List<GameObject>();
    int currentHighlightIndex = 0;
    
    SCR_ControllerDetection controllerDetection;

    SCR_InventoryManager inventoryManager;
    private void Awake()
    {
        controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);

        inventoryManager = GameObject.Find("InventoryPanel").GetComponent<SCR_InventoryManager>();
        CreateHighlights();
    }

    private void OnEnable()
    {
        ChangeSprites(controllerDetection.GetCurrentSprites());
    }

    private void CreateHighlights()
    {
        for (int i = 0; i < inventoryManager.GetSlotAmount(); i++)
        {
            GameObject tempHighlight = Instantiate(highlightPrefab, transform);
            highlightList.Add(tempHighlight);
            Color highLightColor = tempHighlight.GetComponent<Image>().color;
            Color buttonColor = buttonList[i].color;

            if (i != 0)
            {
                tempHighlight.GetComponent<Image>().color = new Color(highLightColor.r, highLightColor.g, highLightColor.b, 0);
                buttonList[i].color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0);
            }

        }
    }
    //Separate in case we want to expand what will happen when something is deactivated
    private void DeactivateHighlight(int indexToDeactivate)
    {
        Color highLightColor = highlightList[indexToDeactivate].GetComponent<Image>().color;
        highlightList[indexToDeactivate].GetComponent<Image>().color = new Color(highLightColor.r, highLightColor.g, highLightColor.b, 0);

        Color buttonColor = buttonList[indexToDeactivate].color;
        buttonList[indexToDeactivate].color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0);
    }
    public void ActivateHighlight(int indexToActivate)
    {
        DeactivateHighlight(currentHighlightIndex);
        Color currentColor = highlightList[indexToActivate].GetComponent<Image>().color;
        highlightList[indexToActivate].GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);

        Color buttonColor = buttonList[indexToActivate].color;
        buttonList[indexToActivate].color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 1);
        currentHighlightIndex = indexToActivate;
    }

    public void ChangeSprites(List<Sprite> currentSprites)
    {
        foreach (Image button in buttonList)
        {
            button.sprite = currentSprites[0];
        }
    }
}
