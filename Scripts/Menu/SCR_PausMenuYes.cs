using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Written by Isabelle H. Heiskanen
public class SCR_PausMenuYes : SCR_MenuItem
{
    public override void Select()
    {
        SCR_FadeScreen.Fade();
        StartCoroutine(WaitToTransition());
    }

    private IEnumerator WaitToTransition()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }
}
