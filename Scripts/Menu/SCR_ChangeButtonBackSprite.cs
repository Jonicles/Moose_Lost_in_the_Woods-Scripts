using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Written by Isabelle H. Heiskanen
public class SCR_ChangeButtonBackSprite : MonoBehaviour, SCR_IButtonListener
{
    [SerializeField] private Image backImage01;
    [SerializeField] private Image backImage02;
    [SerializeField] private Image backImage03;

    private void Awake()
    {
        SCR_ControllerDetection controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);
    }

    public void ChangeSprites(List<Sprite> currentSprites)
    {
        if (backImage01 != null)
        {
            backImage01.sprite = currentSprites[9];
        }

        if (backImage02 != null)
        {
            backImage02.sprite = currentSprites[9];
        }

        if (backImage03 != null)
        {
            backImage03.sprite = currentSprites[9];
        }
    }
}
