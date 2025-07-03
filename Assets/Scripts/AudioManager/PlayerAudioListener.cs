using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class PlayerAudioListener : MonoBehaviour
{
    private Transform currentCamera;

    void Start()
    {
        this.currentCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = currentCamera.transform.eulerAngles.y;
        transform.eulerAngles = currentRotation;
    }
}