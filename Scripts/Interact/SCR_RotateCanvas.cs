using UnityEngine;

// Written by Isabelle H. Heiskanen
// THIS SCRIPT NEEDS TO SIT ON THE CANVAS ON EVERY PLAYER TO MAKE IT ROTATE TO THE CAMERA
public class SCR_RotateCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        transform.forward = Camera.main.transform.forward;
    }
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
