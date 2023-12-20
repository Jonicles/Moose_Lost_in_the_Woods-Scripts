using UnityEngine;

// Written by Isabelle H. Heiskanen
[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Dialogue/New Speaker")]
public class SCR_Speaker : ScriptableObject
{
    [SerializeField] private string speakerName;    

    public string GetName()
    {
        return speakerName;
    }
}
