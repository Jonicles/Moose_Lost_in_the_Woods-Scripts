using UnityEngine;

// Written by Isabelle H. Heiskanen
[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/New Conversation")]
public class SCR_Conversation : ScriptableObject
{
    [SerializeField] private SCR_DialogueLine[] allDialogueLines;    

    // Returns the correct dialogue Line
    public SCR_DialogueLine GetDialogueLineByIndex(int index)
    {
        return allDialogueLines[index];
    }

    // Returns the Lenght of the conversation to make sure no errors accure
    public int GetLength()
    {
        return allDialogueLines.Length - 1;
    }
}
