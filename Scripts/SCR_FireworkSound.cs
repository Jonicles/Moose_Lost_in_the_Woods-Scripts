using UnityEngine;

public class SCR_FireworkSound : MonoBehaviour
{
    ParticleSystem particleSystem;
    float currentParticleCount = 0;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (particleSystem.isPlaying)
        {
            float particleAmount = Mathf.Abs(particleSystem.particleCount - currentParticleCount);

            if(particleSystem.particleCount > currentParticleCount)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Other/Firework_Bang");
            }

            currentParticleCount = particleSystem.particleCount;
        }
    }
}
