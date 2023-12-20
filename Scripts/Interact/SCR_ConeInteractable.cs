using System.Collections;
using UnityEngine;

// Written by Isabelle H. Heiskanen

// SITS ON ALL CONES IN THE SCENE
public class SCR_ConeInteractable : MonoBehaviour, SCR_ICollectible
{
    public static event HandleConesCollected OnConesCollected;
    public delegate void HandleConesCollected(SCR_ItemData itemdata);

    private new Rigidbody rigidbody;
    private bool hasTarget;
    private Vector3 targetPosition;
    public bool HasReachedTarget { get; private set; }
    [SerializeField] private int moveSpeed = 5;

    [SerializeField] SCR_ItemData conesData;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (hasTarget && !HasReachedTarget)
        {
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            rigidbody.velocity = new Vector3(targetDirection.x, targetDirection.y, targetDirection.z) * moveSpeed;
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void WhenCollected()
    {
        if (HasReachedTarget)
            return;

        HasReachedTarget = true;
        rigidbody.velocity = Vector3.zero;
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Collectibles/Collectible_Kotte_Collect", gameObject);
        OnConesCollected?.Invoke(conesData);
        StartCoroutine(ConeDestroy());
    }

    IEnumerator ConeDestroy()
    {
        GetComponent<Animator>().Play("AC_Cone_Collect");
        yield return new WaitForSeconds(0.417f);
        Destroy(gameObject);
    }

}
