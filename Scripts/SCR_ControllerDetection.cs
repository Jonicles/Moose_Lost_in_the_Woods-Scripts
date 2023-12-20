using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Switch;

//John
public class SCR_ControllerDetection : MonoBehaviour
{
    [Header("XBOX Sprites")]
    [SerializeField] Sprite XBOXJump;
    [SerializeField] Sprite XBOXInteract;
    [SerializeField] Sprite XBOXInteractPressed;
    [SerializeField] Sprite XBOXMoveForward;
    [SerializeField] Sprite XBOXMoveLeft;
    [SerializeField] Sprite XBOXMoveRight;
    [SerializeField] Sprite XBOXMoveBackward;
    [SerializeField] Sprite XBOXMoveCamera;
    [SerializeField] Sprite XBOXInventory;
    [SerializeField] Sprite XBOXBack;
    [SerializeField] Sprite XBOXResetCamera;
    [SerializeField] Sprite XBOXSprint;

    [Header("Playstation Sprites")]
    [SerializeField] Sprite PlaystationJump;
    [SerializeField] Sprite PlaystationInteract;
    [SerializeField] Sprite PlaystationInteractPressed;
    [SerializeField] Sprite PlaystationMoveForward;
    [SerializeField] Sprite PlaystationMoveLeft;
    [SerializeField] Sprite PlaystationMoveRight;
    [SerializeField] Sprite PlaystationMoveBackward;
    [SerializeField] Sprite PlaystationMoveCamera;
    [SerializeField] Sprite PlaystationInventory;
    [SerializeField] Sprite PlaystationBack;
    [SerializeField] Sprite PlaystationResetCamera;
    [SerializeField] Sprite PlaystationSprint;

    [Header("Switch Sprites")]
    [SerializeField] Sprite SwitchJump;
    [SerializeField] Sprite SwitchInteract;
    [SerializeField] Sprite SwitchInteractPressed;
    [SerializeField] Sprite SwitchMoveForward;
    [SerializeField] Sprite SwitchMoveLeft;
    [SerializeField] Sprite SwitchMoveRight;
    [SerializeField] Sprite SwitchMoveBackward;
    [SerializeField] Sprite SwitchMoveCamera;
    [SerializeField] Sprite SwitchInventory;
    [SerializeField] Sprite SwitchBack;
    [SerializeField] Sprite SwitchResetCamera;
    [SerializeField] Sprite SwitchSprint;

    [Header("Keyboard Sprites")]
    [SerializeField] Sprite KeyboardJump;
    [SerializeField] Sprite KeyboardInteract;
    [SerializeField] Sprite KeyboardInteractPressed;
    [SerializeField] Sprite KeyboardMoveForward;
    [SerializeField] Sprite KeyboardMoveLeft;
    [SerializeField] Sprite KeyboardMoveRight;
    [SerializeField] Sprite KeyboardMoveBackward;
    [SerializeField] Sprite KeyboardMoveCamera;
    [SerializeField] Sprite KeyboardInventory;
    [SerializeField] Sprite KeyboardBack;
    [SerializeField] Sprite KeyboardResetCamera;
    [SerializeField] Sprite KeyboardSprint;

    Sprite CurrentJump;             // 0
    Sprite CurrentInteract;         // 1
    Sprite CurrentInteractPressed;  // 2
    Sprite CurrentMoveForward;      // 3
    Sprite CurrentMoveLeft;         // 4
    Sprite CurrentMoveRight;        // 5
    Sprite CurrentMoveBackward;     // 6
    Sprite CurrentMoveCamera;       // 7
    Sprite CurrentInventory;        // 8
    Sprite CurrentBack;             // 9
    Sprite CurrentResetCamera;      // 10
    Sprite CurrentSprint;           // 11

    List<SCR_IButtonListener> listeners = new List<SCR_IButtonListener>();
    List<Sprite> currentSprites = new List<Sprite>();

    public enum ControlType
    {
        XBOX,
        Playstation,
        Switch,
        Keyboard
    }
    ControlType currentControls;

    private void Awake()
    {
        currentSprites.Add(CurrentJump);
        currentSprites.Add(CurrentInteract);
        currentSprites.Add(CurrentInteractPressed);
        currentSprites.Add(CurrentMoveForward);
        currentSprites.Add(CurrentMoveLeft);
        currentSprites.Add(CurrentMoveRight);
        currentSprites.Add(CurrentMoveBackward);
        currentSprites.Add(CurrentMoveCamera);
        currentSprites.Add(CurrentInventory);
        currentSprites.Add(CurrentBack);
        currentSprites.Add(CurrentResetCamera);
        currentSprites.Add(CurrentSprint);
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Start()
    {
        //Do this in start to make sure all the listeners have subscribed in awake
        UpdateCurrentDevice();

        UpdateCurrentSprites(currentControls);

        if (listeners.Count != 0)
        {
            foreach (SCR_IButtonListener listener in listeners)
            {
                listener.ChangeSprites(currentSprites);
            }
        }
    }

    private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        if (arg2 == InputDeviceChange.Added || arg2 == InputDeviceChange.Removed)
        {
            ControlType previousControls = currentControls;
            ControlType newControls = UpdateCurrentDevice();

            if (previousControls != newControls)
            {
                UpdateCurrentSprites(currentControls);

                if (listeners.Count != 0)
                {
                    foreach (SCR_IButtonListener listener in listeners)
                    {
                        listener.ChangeSprites(currentSprites);
                    }
                }
                print("New controls: " + currentControls);
                SCR_RumbleManager.Instance.StartRumble(RumbleType.ControllerConnect);
            }
        }
    }

    private void UpdateCurrentSprites(ControlType newControls)
    {
        switch (newControls)
        {
            case ControlType.XBOX:
                CurrentJump = XBOXJump;
                CurrentInteract = XBOXInteract;
                CurrentInteractPressed = XBOXInteractPressed;
                CurrentMoveForward = XBOXMoveForward;
                CurrentMoveLeft = XBOXMoveLeft;
                CurrentMoveRight = XBOXMoveRight;
                CurrentMoveBackward = XBOXMoveBackward;
                CurrentMoveCamera = XBOXMoveCamera;
                CurrentInventory = XBOXInventory;
                CurrentBack = XBOXBack;
                CurrentResetCamera = XBOXResetCamera;
                CurrentSprint = XBOXSprint;
                break;
            case ControlType.Playstation:
                CurrentJump = PlaystationJump;
                CurrentInteract = PlaystationInteract;
                CurrentInteractPressed = PlaystationInteractPressed;
                CurrentMoveForward = PlaystationMoveForward;
                CurrentMoveLeft = PlaystationMoveLeft;
                CurrentMoveRight = PlaystationMoveRight;
                CurrentMoveBackward = PlaystationMoveBackward;
                CurrentMoveCamera = PlaystationMoveCamera;
                CurrentInventory = PlaystationInventory;
                CurrentBack = PlaystationBack;
                CurrentResetCamera = PlaystationResetCamera;
                CurrentSprint = PlaystationSprint;
                break;
            case ControlType.Switch:
                CurrentJump = SwitchJump;
                CurrentInteract = SwitchInteract;
                CurrentInteractPressed = SwitchInteractPressed;
                CurrentMoveForward = SwitchMoveForward;
                CurrentMoveLeft = SwitchMoveLeft;
                CurrentMoveRight = SwitchMoveRight;
                CurrentMoveBackward = SwitchMoveBackward;
                CurrentMoveCamera = SwitchMoveCamera;
                CurrentInventory = SwitchInventory;
                CurrentBack = SwitchBack;
                CurrentResetCamera = SwitchResetCamera;
                CurrentSprint = SwitchSprint;
                break;
            case ControlType.Keyboard:
                CurrentJump = KeyboardJump;
                CurrentInteract = KeyboardInteract;
                CurrentInteractPressed = KeyboardInteractPressed;
                CurrentMoveForward = KeyboardMoveForward;
                CurrentMoveLeft = KeyboardMoveLeft;
                CurrentMoveRight = KeyboardMoveRight;
                CurrentMoveBackward = KeyboardMoveBackward;
                CurrentMoveCamera = KeyboardMoveCamera;
                CurrentInventory = KeyboardInventory;
                CurrentBack = KeyboardBack;
                CurrentResetCamera = KeyboardResetCamera;
                CurrentSprint = KeyboardSprint;
                break;
            default:
                CurrentJump = KeyboardJump;
                CurrentInteract = KeyboardInteract;
                CurrentInteractPressed = KeyboardInteractPressed;
                CurrentMoveForward = KeyboardMoveForward;
                CurrentMoveLeft = KeyboardMoveLeft;
                CurrentMoveRight = KeyboardMoveRight;
                CurrentMoveBackward = KeyboardMoveBackward;
                CurrentMoveCamera = KeyboardMoveCamera;
                CurrentInventory = KeyboardInventory;
                CurrentBack = KeyboardBack;
                CurrentResetCamera = KeyboardResetCamera;
                CurrentSprint = KeyboardSprint;
                break;
        }

        currentSprites.Clear();
        currentSprites.Add(CurrentJump);
        currentSprites.Add(CurrentInteract);
        currentSprites.Add(CurrentInteractPressed);
        currentSprites.Add(CurrentMoveForward);
        currentSprites.Add(CurrentMoveLeft);
        currentSprites.Add(CurrentMoveRight);
        currentSprites.Add(CurrentMoveBackward);
        currentSprites.Add(CurrentMoveCamera);
        currentSprites.Add(CurrentInventory);
        currentSprites.Add(CurrentBack);
        currentSprites.Add(CurrentResetCamera);
        currentSprites.Add(CurrentSprint);
    }

    private ControlType UpdateCurrentDevice()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current is XInputController)
            {
                currentControls = ControlType.XBOX;
                return currentControls;
            }

            if (Gamepad.current is DualSenseGamepadHID || Gamepad.current is DualShock4GamepadHID || Gamepad.current is DualShock3GamepadHID)
            {
                currentControls = ControlType.Playstation;
                return currentControls;
            }

            if (Gamepad.current is SwitchProControllerHID)
            {
                currentControls = ControlType.Switch;
                return currentControls;
            }

            currentControls = ControlType.Keyboard;
            return currentControls;
        }
        else
        {
            currentControls = ControlType.Keyboard;
            return currentControls;
        }
    }

    public void AddListener(SCR_IButtonListener listenerToAdd)
    {
        listeners.Add(listenerToAdd);
    }

    public List<Sprite> GetCurrentSprites()
    {
        return currentSprites;
    }
}
