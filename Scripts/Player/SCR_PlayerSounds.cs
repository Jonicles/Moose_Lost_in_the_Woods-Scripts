using UnityEngine;
using FMODUnity;

//John
public class SCR_PlayerSounds : MonoBehaviour
{
    [SerializeField] ParticleSystem dustParticle;
    SCR_PlayerMovement player;
   
    private void Awake()
    {
        player = FindObjectOfType<SCR_PlayerMovement>();
    }
    public void PlayStepSound()
    {
        if (player.IsSprinting)
        {
            dustParticle.Play();
            RuntimeManager.PlayOneShotAttached("event:/Player/Player_Step_Sprint", gameObject);
        }
        else
            RuntimeManager.PlayOneShotAttached("event:/Player/Player_Step", gameObject);
    }

    public void PlaySwimStrokeSound()
    {
        RuntimeManager.PlayOneShotAttached("event:/Player/Player_Swim", gameObject);
    }
}
