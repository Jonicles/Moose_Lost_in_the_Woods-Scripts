using UnityEngine;

// Written by Isabelle H. Heiskanen
// SITS ON A CHILD OF THE NPC WITH A TRIGGER COLLIDER
public class SCR_ResetConversation : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SCR_DialogueManager.ResetConversation();
        }
    }
}
