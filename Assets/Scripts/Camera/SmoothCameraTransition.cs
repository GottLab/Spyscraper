using UnityEngine;

public class SmoothCameraTransition : MonoBehaviour
{
    public Vector3 startPoint = new Vector3(0f, 0f, 0f);
    public Vector3 endPoint = new Vector3(0f, 0f, 0f);
    public float duration = 3f; // Duration of the transition in seconds

    private float elapsedTime = 0f;
    private bool isMoving = true;

    void Start()
    {
        transform.position = startPoint;
    }

    void Update()
    {
        if (!isMoving)
            return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // Apply smoothstep for ease-in ease-out effect
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        transform.position = Vector3.Lerp(startPoint, endPoint, smoothT);

        if (t >= 1f)
        {
            isMoving = false;
        }
    }
}
