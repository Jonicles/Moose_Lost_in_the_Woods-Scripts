using UnityEngine;

// John
public class SCR_MenuItemPanel : SCR_MenuItem
{
    [SerializeField] GameObject panel;
    public override void Select()
    {
        panel.SetActive(true);
    }

    public override void Deselect()
    {
        panel.SetActive(false);
    }
}
