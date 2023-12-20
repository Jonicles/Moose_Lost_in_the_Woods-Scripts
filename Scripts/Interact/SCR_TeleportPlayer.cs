using System.Collections;
using UnityEngine;

// Written by Isabelle H. Heiskanen
// THIS SCRIPTS NEEDS TO SIT ON THE NPC THAT WILL TELEPORT THE PLAYER WHEN EVENT IS ACTIVE
public class SCR_TeleportPlayer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] float timerToMovePlayer = 1.5f;
    [SerializeField] float timeBeforeSound = 0.2f;
    [SerializeField] GameObject InteractObjectConvo;

    private void Awake()
    {
        player = FindAnyObjectByType<SCR_PlayerInteract>().gameObject;
    }

    public void TeleportPlayer(GameObject teleportToObject)
    {
        SCR_PlayerInputManager.Instance.Player.Disable();
        SCR_FadeScreen.Fade();
        StartCoroutine(TimerToMovePlayer(teleportToObject));
    }

    private IEnumerator TimerToMovePlayer(GameObject teleportToObject)
    {
        float timeAfterSound = timerToMovePlayer - timeBeforeSound;
        yield return new WaitForSeconds(timeBeforeSound);

        if (gameObject.name == "Sune" || gameObject.name == "Woodpecker")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Player_Build");
            SCR_RumbleManager.Instance.StartRumble(RumbleType.Build);
        }

        yield return new WaitForSeconds(timeAfterSound);

        InteractObjectConvo.SetActive(false);
        player.transform.position = teleportToObject.transform.position;
        SCR_PlayerInputManager.Instance.Player.Enable();
    }
}
