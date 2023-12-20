using System.Collections;
using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON THE HATS IN THE SCENE
public class SCR_HatInteractable : MonoBehaviour, SCR_IInteractable
{
    public static event HandleHatCollected OnHatCollected;
    public delegate void HandleHatCollected(SCR_ItemData itemdata);
    SCR_PlayerMovement playerMovement;
    [SerializeField] SCR_ItemData hatData;
    [SerializeField] ParticleSystem particleSystem;

    private Animator animator;
    public static bool isPickingUpHat;

    const string collectingString = "isCollecting";

    private void Awake()
    {
        playerMovement = FindObjectOfType<SCR_PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        StartCoroutine(DestroyTimer());
        OnHatCollected?.Invoke(hatData);
    }

    IEnumerator DestroyTimer()
    {
        particleSystem.Stop();
        SCR_RumbleManager.Instance.StartRumble(RumbleType.Pickup);
        gameObject.GetComponent<Collider>().enabled = false;
        playerMovement.StopAndInteract(false, transform);
        isPickingUpHat = true;
        animator.SetBool(collectingString, true);
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
        isPickingUpHat = false;
    }

    public Sprite GetSpriteIcon()
    {
        return hatData.icon;
    }

    // CHANGE TO CORRECT SOUND??
    public void PlayCollectSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Collectibles/Collectible_Plank_Collect", gameObject);
    }

}
