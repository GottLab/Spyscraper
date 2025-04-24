using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class IsometricCameraZoom : MonoBehaviour
{

    private Camera cam;

   
    public float minSize = 1;
    public float maxSize = 20;


    private float targetSize = 10;
    public float zoomSensitivity = 1F;


    public Vector2 maxPanning = new Vector2(3.0f, 3.0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        this.targetSize += -Input.mouseScrollDelta.y * zoomSensitivity * Time.deltaTime;
        this.targetSize = Mathf.Clamp(this.targetSize, this.minSize, this.maxSize);
        this.cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, this.targetSize, Time.deltaTime * 10.0F);


        Vector2 cameraTarget = new Vector2(0,0);
        if(Input.GetMouseButton(1))
        {
            
            Vector2 mousePosition = Input.mousePosition;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 mousePercentage = ((mousePosition / screenSize) * 2.0f) - 1.0f * Vector2.one;
            mousePercentage.x = Mathf.Clamp(-1, mousePercentage.x, 1);
            mousePercentage.y = Mathf.Clamp(-1, mousePercentage.y, 1);
            cameraTarget.x = mousePercentage.x * this.maxPanning.x;
            cameraTarget.y = mousePercentage.y * this.maxPanning.y;
        }



        float panResetSpeed = 10.0F;
        
        Vector3 newPosition = transform.localPosition;
        newPosition.x = Mathf.Lerp(newPosition.x, cameraTarget.x, Time.deltaTime * panResetSpeed);
        newPosition.y = Mathf.Lerp(newPosition.y, cameraTarget.y, Time.deltaTime * panResetSpeed);      
        this.transform.localPosition = newPosition;
    }
}
