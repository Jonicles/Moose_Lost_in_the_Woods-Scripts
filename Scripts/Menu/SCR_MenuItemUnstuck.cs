using System.Collections;
using UnityEngine;

public class SCR_MenuItemUnstuck : SCR_MenuItem
{
    GameObject playerObject;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Quaternion startRotation;
    SCR_PausMenu pausmenu;
    SCR_MenuInput menuInput;

    private void Awake()
    {
        if (buttonImage != null)
        {
            Color buttonColor = buttonImage.color;
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0);
        }

        pausmenu = FindAnyObjectByType<SCR_PausMenu>();
        menuInput = FindAnyObjectByType<SCR_MenuInput>();
        playerObject = GameObject.Find("P_Player");
    }

    public override void Select()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        SCR_FadeScreen.Fade();
        yield return new WaitForSeconds(1.5f);
        SCR_DialogueManager.ResetConversation();
        SCR_CameraManager.Instance.ResetOnSceneLoad();
        Teleport();
        SCR_FadeScreen.FadeIn();
        menuInput.ExitSubmenu();
        pausmenu.ClosePausMenu();
    }

    private void Teleport()
    {
        playerObject.transform.position = startPosition;
        playerObject.transform.rotation = startRotation;
    }
}
