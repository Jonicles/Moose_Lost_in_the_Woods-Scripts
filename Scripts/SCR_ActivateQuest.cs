using UnityEngine;

public class SCR_ActivateQuest : MonoBehaviour
{
    [SerializeField] SCR_DansBanaVolumes dansBana;
    private void Awake()
    {
        dansBana.ActivateQuestCollider();
    }
}
