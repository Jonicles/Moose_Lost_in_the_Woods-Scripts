using UnityEngine;

//John
public class SCR_PlayerInputManager : MonoBehaviour
{
    public static SCR_PlayerInputActions Instance;
    public enum StartScene
    {
        MainMenu,
        RockLand,
        Marsh,
        Deepwood
    }

    [SerializeField] StartScene CurrentStart;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = new SCR_PlayerInputActions();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Instance.MenuUI.Enable();
        Instance.StoryBoard.Enable();
        Instance.Player.Enable();
        Instance.InventoryUI.Enable();

        switch (CurrentStart)
        {
            case StartScene.MainMenu:
                Instance.MenuUI.Enable();
                Instance.StoryBoard.Disable();
                Instance.Player.Disable();
                Instance.InventoryUI.Disable();
                break;
            case StartScene.RockLand:
                Instance.StoryBoard.Enable();
                Instance.MenuUI.Disable();
                Instance.Player.Disable();
                Instance.InventoryUI.Disable();
                break;
            case StartScene.Marsh:
                Instance.Player.Enable();
                Instance.StoryBoard.Disable();
                Instance.MenuUI.Disable();
                Instance.InventoryUI.Disable();
                break;
            case StartScene.Deepwood:
                Instance.Player.Enable();
                Instance.StoryBoard.Disable();
                Instance.MenuUI.Disable();
                Instance.InventoryUI.Disable();
                break;
            default:
                break;
        }
    }
}
