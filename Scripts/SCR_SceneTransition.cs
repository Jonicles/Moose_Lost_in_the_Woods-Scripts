using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Written by Isabelle H. Heiskanen
// SITS ON THE OBJECT THAT THE PLAYER GOES THROUGH TO CHANGE SCENE
public class SCR_SceneTransition : MonoBehaviour
{
    [SerializeField] int sceneIndexToTransitionTo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SCR_FadeScreen.Fade();
            StartCoroutine(WaitToTransition());
            SCR_PlayerInputManager.Instance.Player.Disable();
        }
    }

    public void TransitionToNewScene()
    {
        SCR_FadeScreen.Fade();
        StartCoroutine(WaitToTransitionOnTriggerScene());
    }

    private IEnumerator WaitToTransition()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneIndexToTransitionTo);
    }

    private IEnumerator WaitToTransitionOnTriggerScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneIndexToTransitionTo);
    }
}