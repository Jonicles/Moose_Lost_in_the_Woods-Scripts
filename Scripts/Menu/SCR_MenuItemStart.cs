using UnityEngine;

// John and Isabelle H. Heiskanen
public class SCR_MenuItemStart : SCR_MenuItem
{
    SCR_SceneTransition sceneTransition;
    SCR_Inventory inventory;

    private void Awake()
    {
        sceneTransition = FindObjectOfType<SCR_SceneTransition>();
        inventory = FindObjectOfType<SCR_Inventory>();
    }
    public override void Select()
    {
        ResetSaveData();
        sceneTransition.TransitionToNewScene();
    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        SCR_Inventory.coneAmount = 0;
        SCR_Inventory.planksAmount= 0;

        SCR_Badges.hasBadge01 = false;
        SCR_Badges.hasBadge02 = false;
        SCR_Badges.hasBadge03 = false;
    }
}

