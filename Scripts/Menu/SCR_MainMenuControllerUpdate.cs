using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//John
public class SCR_MainMenuControllerUpdate : MonoBehaviour, SCR_IButtonListener
{
    [SerializeField] private Image jumpImage;
    [SerializeField] private Image interactImage;
    [SerializeField] private Image moveForwardImage;
    [SerializeField] private Image moveLeftImage;
    [SerializeField] private Image moveRightImage;
    [SerializeField] private Image moveBackwardsImage;
    [SerializeField] private Image moveCameraImage;
    [SerializeField] private Image inventoryImage;
    [SerializeField] private Image resetCameraImage;
    [SerializeField] private Image sprintImage;

    private void Awake()
    {
        SCR_ControllerDetection controllerDetection = FindAnyObjectByType<SCR_ControllerDetection>();
        controllerDetection.AddListener(this);
    }

    public void ChangeSprites(List<Sprite> currentSprites)
    {
        jumpImage.sprite = currentSprites[0];
        interactImage.sprite = currentSprites[1];
        moveForwardImage.sprite = currentSprites[3];
        moveLeftImage.sprite = currentSprites[4];
        moveRightImage.sprite = currentSprites[5];
        moveBackwardsImage.sprite = currentSprites[6];
        moveCameraImage.sprite = currentSprites[7];
        inventoryImage.sprite = currentSprites[8];
        resetCameraImage.sprite = currentSprites[10];
        sprintImage.sprite = currentSprites[11];
    }
}
