using System.Collections;
using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON THE PLANKS IN THE SCENE
public class SCR_PlanksInteractable : MonoBehaviour, SCR_IInteractable
{
    public static event HandlePlanksCollected OnPlanksCollected;
    public delegate void HandlePlanksCollected(SCR_ItemData itemdata);
    SCR_PlayerMovement playerMovement;
    [SerializeField] SCR_ItemData planksData;
    [SerializeField] ParticleSystem particleSystem;

    private Animator animator;
    public static bool isPickingUpPlanks;

    const string collectingString = "isCollecting";

    private void Awake()
    {
        playerMovement = FindObjectOfType<SCR_PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        StartCoroutine(DestroyTimer());
        OnPlanksCollected?.Invoke(planksData);
    }
    IEnumerator DestroyTimer()
    {
        particleSystem.Stop();
        SCR_RumbleManager.Instance.StartRumble(RumbleType.Pickup);
        gameObject.GetComponent<Collider>().enabled = false;
        playerMovement.StopAndInteract(false, transform);
        isPickingUpPlanks = true;
        animator.SetBool(collectingString, true);
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
        isPickingUpPlanks = false;
    }

    public Sprite GetSpriteIcon()
    {
        return planksData.icon;
    }

    public void PlayCollectSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Collectibles/Collectible_Plank_Collect", gameObject);
    }
}
