
// John
public class SCR_MenuItemQuitNo : SCR_MenuItem
{
    SCR_MenuInput menuInput;

    private void Awake()
    {
        menuInput = FindObjectOfType<SCR_MenuInput>();
    }

    public override void Select()
    {
        menuInput.ExitSubmenu();
    }
}
