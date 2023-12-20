using System.Collections;
using UnityEngine;

//John
public class SCR_BouncySurface : MonoBehaviour
{
    const string PlayerTag = "Player";
    [SerializeField] float bounceForce;
    [SerializeField] ParticleSystem mushroomParticle;
    Animator bounceAnimator;

    private void Awake()
    {
        bounceAnimator = gameObject.GetComponentInParent<Animator>();
        StartCoroutine(OffsetIdleAnimation());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Other/Mushroom_Bounce", gameObject);
            bounceAnimator.Play("AC_Bounce");
            mushroomParticle.Play();
            collision.transform.root.GetComponent<Rigidbody>().AddForce(transform.up * bounceForce, ForceMode.Impulse);
            SCR_RumbleManager.Instance.StartRumble(RumbleType.Mushroom);
        }
    }

    //This method is only so the idle animation will be offset from eachother after the bounce animation has been played
    IEnumerator OffsetIdleAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));
        bounceAnimator.Play("AC_Bounce");
    }
}
