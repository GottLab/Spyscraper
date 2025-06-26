using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ScreenCamera : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();

        canvas.worldCamera = Camera.main;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.planeDistance = 1.0f;
    }

}
