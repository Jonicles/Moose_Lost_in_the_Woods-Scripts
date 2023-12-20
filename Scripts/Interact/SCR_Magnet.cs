using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON A CHILD OBJECT OF THE PLAYER
public class SCR_Magnet : MonoBehaviour
{
    private void Start()
    {
        //Makes sure that the water physics is not disturbed by magnet collider
        Physics.IgnoreLayerCollision(3, 4);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<SCR_ConeInteractable>(out SCR_ConeInteractable cone) && !cone.HasReachedTarget)
        {
            cone.SetTarget(transform.parent.position);
        }        
    }
}
