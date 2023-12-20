using UnityEngine;

// John
public class SCR_MenuItemQuitYes : SCR_MenuItem
{
    public override void Select()
    {
        Application.Quit();
    }
}
