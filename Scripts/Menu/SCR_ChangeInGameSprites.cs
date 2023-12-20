using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Written by Isabelle H. Heiskanen
public class SCR_ChangeInGameSprites : MonoBehaviour, SCR_IButtonListener
{
    [SerializeField] private Image backpackButton;

    private void Awake()
    {
        SCR_ControllerDetection controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);
    }

    public void ChangeSprites(List<Sprite> currentSprites)
    {
        backpackButton.sprite = currentSprites[8];
    }

}
