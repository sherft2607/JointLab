using UnityEngine;
using TMPro;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Make the text face the camera while aligning its up direction with the world's up
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);
        }
    }
}