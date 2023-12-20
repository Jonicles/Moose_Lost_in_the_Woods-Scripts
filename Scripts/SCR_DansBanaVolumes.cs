using System.Collections;
using UnityEngine;
using FMOD.Studio;

public class SCR_DansBanaVolumes : MonoBehaviour
{
    [SerializeField] float volumeChangeTime;
    [SerializeField] AnimationCurve volumeChangeCurve;
    Coroutine volumeRoutine;
    SCR_PlayerMovement player;
    Bus musicBus;
    float musicBusStartVolume;
    bool isQuestCompleted;

    private void Awake()
    {
        player = FindAnyObjectByType<SCR_PlayerMovement>();
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        musicBus.getVolume(out musicBusStartVolume);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LowerVolume();

            if (isQuestCompleted)
                player.SwitchDanceMode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaiseVolume();

            if (isQuestCompleted)
                player.SwitchDanceMode();
        }
    }

    public void LowerVolume()
    {
        if (volumeRoutine != null)
            StopCoroutine(volumeRoutine);

        StartCoroutine(ChangeVolume(0));
    }

    public void RaiseVolume()
    {
        if (volumeRoutine != null)
            StopCoroutine(volumeRoutine);

        StartCoroutine(ChangeVolume(musicBusStartVolume));

    }

    IEnumerator ChangeVolume(float desiredVolume)
    {
        float elapsedTime = 0;
        float startVolume;
        musicBus.getVolume(out startVolume);

        float currentVolume = 0;

        while (volumeChangeTime > elapsedTime)
        {
            currentVolume = Mathf.Lerp(startVolume, desiredVolume, volumeChangeCurve.Evaluate(elapsedTime / volumeChangeTime));
            musicBus.setVolume(currentVolume);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        musicBus.setVolume(desiredVolume);

        volumeRoutine = null;
    }

    public void ActivateQuestCollider()
    {
        isQuestCompleted = true;
        player.SwitchDanceMode();
    }
}
