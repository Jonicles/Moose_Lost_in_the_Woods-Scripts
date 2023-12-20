using System.Collections.Generic;
using UnityEngine;

// John and Isabelle H. Heiskanen
public class SCR_SaveAndLoad : MonoBehaviour
{
    SCR_Inventory inventory;
    SCR_InventoryInput inventoryInput;
    private void Awake()
    {
        inventory = FindAnyObjectByType<SCR_Inventory>();
        inventoryInput = FindAnyObjectByType<SCR_InventoryInput>();
    }

    private void Start()
    {
        SCR_FadeScreen.FadeIn();
        Load();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Save();
        }
    }


    public void Save()
    {
        PlayerPrefs.DeleteAll();
        List<SCR_InventoryItem> hats = inventory.hatInventoryList;

        foreach (SCR_InventoryItem hat in hats)
        {
            PlayerPrefs.SetString(hat.itemData.displayName, hat.itemData.displayName);
        }

        PlayerPrefs.SetInt("Cone", SCR_Inventory.coneAmount);
        PlayerPrefs.SetInt("Planks", SCR_Inventory.planksAmount);
        PlayerPrefs.SetString("CurrentHatPlayerPref", inventoryInput.GetCurrentHat());

        PlayerPrefs.Save();
    }

    public void Load()
    {
        //Make sure the lists are empty so that we do not get duplicate items
        inventory.hatInventoryList.Clear();
        inventory.hatDictionary.Clear();

        inventory.conesInventoryList.Clear();
        inventory.conesDictionary.Clear();

        inventory.planksInventoryList.Clear();
        inventory.planksDictionary.Clear();

        List<string> hatAssetNames = new List<string>();

        //Finding hat assets
        Object[] hatObjects = Resources.LoadAll("Hats");

        foreach (Object hatObject in hatObjects)
        {
            hatAssetNames.Add(hatObject.name);
        }

        FindPlayerPrefs(hatAssetNames);
    }

    private void FindPlayerPrefs(List<string> hatAssetNames)
    {
        foreach (string hatName in hatAssetNames)
        {
            if (PlayerPrefs.HasKey(hatName))
            {
                SCR_ItemData hatData;
                hatData = (SCR_ItemData)Resources.Load($"Hats/{hatName}");
                inventory.Add(hatData);
            }
        }

        if (PlayerPrefs.HasKey("Cone"))
        {
            for (int i = 0; i < PlayerPrefs.GetInt("Cone"); i++)
            {
                SCR_ItemData coneData;
                coneData = (SCR_ItemData)Resources.Load("Cone");
                inventory.Add(coneData);
            }
        }

        if (PlayerPrefs.HasKey("Planks"))
        {
            for (int i = 0; i < PlayerPrefs.GetInt("Planks"); i++)
            {
                SCR_ItemData plankData;
                plankData = (SCR_ItemData)Resources.Load("Planks");
                inventory.Add(plankData);
            }
        }
    }
}
