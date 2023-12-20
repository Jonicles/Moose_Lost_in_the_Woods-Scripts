using UnityEngine;
using UnityEngine.UI;

// John
public class SCR_MenuItemSlider : SCR_MenuItem
{
    Slider slider;
    [SerializeField] string vcaName;
    [SerializeField] float increment = 1f;
    FMOD.Studio.VCA vca;


    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();

        vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        float tempVolume;
        vca.getVolume(out tempVolume);

        slider.value = tempVolume;
    }

    public void ChangeValue(float strength)
    {
        if (slider.value + (increment * strength) > slider.maxValue)
        {
            slider.value = slider.maxValue;
            vca.setVolume(slider.value);
            return;
        }

        if (slider.value + (increment * strength) < slider.minValue)
        {
            slider.value = slider.minValue;
            vca.setVolume(slider.value);
            return;
        }

        slider.value += (increment * strength);
        vca.setVolume(slider.value);
    }

    public void ResetToDefault()
    {
        slider.value = 1;
        vca.setVolume(slider.value);
    }

    public override void Hover(Color hoverColor)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Move");
        transform.Find("Handle Slide Area").transform.Find("Handle").GetComponent<Image>().color = hoverColor;
        transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = hoverColor;
    }

    public override void Leave(Color defaultColor)
    {
        transform.Find("Handle Slide Area").transform.Find("Handle").GetComponent<Image>().color = defaultColor;
        transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = defaultColor;
    }
}
