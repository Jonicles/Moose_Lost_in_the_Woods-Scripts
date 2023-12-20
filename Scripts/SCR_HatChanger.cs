using System.Collections.Generic;
using UnityEngine;


// Written by Isabelle H. Heiskanen
// SITS ON THE PLAYER, ON THE TOP HAT OBJECT
public class SCR_HatChanger : MonoBehaviour
{
    public List<GameObject> hats = new List<GameObject>();

    private void Awake()
    {
        foreach (GameObject hat in hats)
        {
            hat.SetActive(false);
        }
    }
}
