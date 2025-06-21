using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[
    RequireComponent(typeof(CinemachineCamera)),
    RequireComponent(typeof(CinemachinePositionComposer))
]
public class IsometricCameraZoom : MonoBehaviour
{

    private CinemachineCamera _CinemachineCamera;
    private CinemachinePositionComposer _CinemachinePositionComposer;

   
    public float minSize = 1;
    public float maxSize = 20;


    private float targetSize = 10;
    public float zoomSensitivity = 1F;


    public Vector2 maxPanning = new Vector2(3.0f, 3.0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this._CinemachineCamera = GetComponent<CinemachineCamera>();
        this._CinemachinePositionComposer = GetComponent<CinemachinePositionComposer>();
    }
    void OnEnable()
    {
        PlayerManager.OnStatusChange += OnStateChange;
    }

    void OnDisable()
    {
        PlayerManager.OnStatusChange -= OnStateChange;
    }

    private void OnStateChange(PlayerManager.PlayerState playerState)
    {
        _CinemachinePositionComposer.Composition.ScreenPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL))
        {
            return;
        }

        this.targetSize += -Input.mouseScrollDelta.y * zoomSensitivity * Time.deltaTime;
        this.targetSize = Mathf.Clamp(this.targetSize, this.minSize, this.maxSize);
        this._CinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(this._CinemachineCamera.Lens.OrthographicSize, this.targetSize, Time.deltaTime * 10.0F);


        Vector2 cameraTarget = new Vector2(0, 0);
        if (Input.GetMouseButton(1))
        {

            Vector2 mousePosition = Input.mousePosition;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 mousePercentage = (mousePosition / screenSize * 2.0f) - 1.0f * Vector2.one;
            mousePercentage.x = Mathf.Clamp(-1, mousePercentage.x, 1);
            mousePercentage.y = Mathf.Clamp(-1, mousePercentage.y, 1);
            cameraTarget.x = -mousePercentage.x * this.maxPanning.x;
            cameraTarget.y = mousePercentage.y * this.maxPanning.y;
        }

        _CinemachinePositionComposer.Composition.ScreenPosition = cameraTarget;
    }
}
