using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON THE IMAGE BG ON CANVAS FADE SCREEN IN SCENE

public class SCR_FadeScreen : MonoBehaviour
{
    // Singelton reference
    private static SCR_FadeScreen instance;

    private Animator animator;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        animator = GetComponentInChildren<Animator>();
    }

    public static void Fade()
    {
        instance.animator.Play("AC_FadeScreen");
    }

    public static void FadeIn()
    {
        instance.animator.Play("AC_FadeInScreen");
    }
}
