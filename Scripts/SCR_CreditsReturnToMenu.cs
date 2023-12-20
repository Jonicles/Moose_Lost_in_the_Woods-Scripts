using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SCR_CreditsReturnToMenu : MonoBehaviour
{
    [SerializeField] AnimationClip transitionClip;

    private void Start()
    {
        StartCoroutine(WaitAndTransition());
    }

    IEnumerator WaitAndTransition()
    {
        SCR_FadeScreen.FadeIn();
        yield return new WaitForSeconds(transitionClip.length);
        SCR_FadeScreen.Fade();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }
}
