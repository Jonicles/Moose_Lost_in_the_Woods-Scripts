using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//John
public class SCR_MenuItem : MonoBehaviour, SCR_IButtonListener
{
    [SerializeField] List<SCR_MenuItem> subItems;
    [SerializeField] protected Image buttonImage;
    SCR_ControllerDetection controllerDetection;
    bool hasFoundController;

    private void Awake()
    {
        if (buttonImage != null)
        {
            Color buttonColor = buttonImage.color;
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0);
        }

    }

    private void Start()
    {
        controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);
        if (controllerDetection != null)
        {
            hasFoundController = true;
            ChangeSprites(controllerDetection.GetCurrentSprites());
        }
    }

    private void OnEnable()
    {
        if (hasFoundController)
        {
            ChangeSprites(controllerDetection.GetCurrentSprites());
        }
    }

    virtual public void Select() { }

    virtual public void Deselect() { }

    virtual public void Hover(Color hoverColor)
    {
        if (buttonImage != null)
        {
            Color buttonColor = buttonImage.color;
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 1);
        }

        if (TryGetComponent(out TMP_Text text))
            text.color = hoverColor;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Move");
    }

    virtual public void Leave(Color defaultColor)
    {
        if (buttonImage != null)
        {
            Color buttonColor = buttonImage.color;
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0);
        }

        if (TryGetComponent(out TMP_Text text))
            text.color = defaultColor;
    }

    public List<SCR_MenuItem> GetSubItems()
    {
        return subItems;
    }

    public void ChangeSprites(List<Sprite> newSprites)
    {
        if (buttonImage != null)
            buttonImage.sprite = newSprites[0];
    }
}
