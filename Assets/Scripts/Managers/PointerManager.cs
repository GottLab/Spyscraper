using UnityEngine;

public class PointerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status => ManagerStatus.Started;


    [SerializeField, Tooltip("Mask used for cursor looking")]
    private LayerMask lookLayerMask;

    [SerializeField, Tooltip("The cursor transform")]
    private Transform targetTransform;

    [SerializeField]
    private Camera currentCamera;

    public void Startup()
    {

    }

    void Update()
    {
        if (this.targetTransform == null || currentCamera == null)
        {
            return;
        }

        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, lookLayerMask)) // Check if we hit something
        {

            this.targetTransform.position = Vector3.Lerp(this.targetTransform.position, hit.point, Time.deltaTime * 10.0f);
        }

    }
    
    public Transform pointer {
        get => targetTransform;
    }
}