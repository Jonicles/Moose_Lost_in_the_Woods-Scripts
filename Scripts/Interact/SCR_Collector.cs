using UnityEngine;

// Written by Isabelle H. Heiskanen

// SITS ON THE PLAYER
// IMPORTANT TO HAVE A TRIGGER COLLIDER ON THE SAME OBJECT AND A RIGIDBODY !!!
public class SCR_Collector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        SCR_ICollectible collectible = other.GetComponent<SCR_ICollectible>();

        if (collectible != null)
        {
            collectible.WhenCollected();
        }
    }
}
