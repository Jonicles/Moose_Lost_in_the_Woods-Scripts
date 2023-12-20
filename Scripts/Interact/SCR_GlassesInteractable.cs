using System.Collections;
using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON THE GLASES IN THE SCENE
public class SCR_GlassesInteractable : MonoBehaviour, SCR_IInteractable
{
    public static event HandleGlasesCollected OnGlassesCollected;
    public delegate void HandleGlasesCollected(SCR_ItemData itemdata);
    SCR_PlayerMovement playerMovement;
    [SerializeField] SCR_ItemData glasesData;

    private Animator animator;
    public static bool isPickingUpGlases;

    const string collectingString = "isCollecting";

    private void Awake()
    {
        playerMovement = FindObjectOfType<SCR_PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        StartCoroutine(DestroyTimer());
        OnGlassesCollected?.Invoke(glasesData);
    }
    IEnumerator DestroyTimer()
    {
        SCR_RumbleManager.Instance.StartRumble(RumbleType.Pickup);
        gameObject.GetComponent<Collider>().enabled = false;
        playerMovement.StopAndInteract(false, transform);
        isPickingUpGlases = true;
        animator.SetBool(collectingString, true);
        yield return new WaitForSeconds(2);

        Destroy(gameObject);
        isPickingUpGlases = false;
    }

    public Sprite GetSpriteIcon()
    {
        return glasesData.icon;
    }
}
