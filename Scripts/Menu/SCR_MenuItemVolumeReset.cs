// John
public class SCR_MenuItemVolumeReset : SCR_MenuItem
{
    SCR_MenuItemSlider[] sliderArray;

    private void Awake()
    {
        sliderArray = FindObjectsOfType<SCR_MenuItemSlider>();
    }

    public override void Select()
    {
        foreach (SCR_MenuItemSlider slider in sliderArray)
        {
            slider.ResetToDefault();
        }
    }
}
