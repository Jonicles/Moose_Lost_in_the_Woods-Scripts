using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON THE WATERCOLLIDERS IN AREA ONE
public class SCR_TurnOffObject : MonoBehaviour
{

    void Update()
    {
        if (SCR_Badges.hasBadge01)
        {
            gameObject.SetActive(false);
        }
    }
}
