using UnityEngine;

public struct ColliderInfo
{
    public Vector3 center;
    public float height;

    public ColliderInfo(Vector3 center, float height) : this()
    {
        this.center = center;
        this.height = height;
    }
}

[RequireComponent(typeof(Light))]
public class ConeVision : MonoBehaviour
{

    [SerializeField, Tooltip("Enable debug gizmo")]
    private bool debug = false;


    [SerializeField]
    private ConeVisionObject coneVisionObject;

    public float coneAngle = 45;

    public float coneHeight = 10;

    public int density = 4;

    public LayerMask layerMask = 0;


    private bool TryGetColliderInfo(Collider collider, out ColliderInfo info)
    {
        if (collider is BoxCollider boxCollider)
        {
            info = new ColliderInfo(boxCollider.center, boxCollider.size.y);
            return true;
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            info = new ColliderInfo(capsuleCollider.center, capsuleCollider.height);
            return true;
        }

        info = default;
        return false;
    }


    public bool ConeIntersectBoxCollider(Collider collider)
    {

        if (TryGetColliderInfo(collider, out ColliderInfo colliderInfo))
        {
            Vector3 coneApex = this.transform.position;
            Vector3 coneDirection = this.transform.forward;

            float dif = colliderInfo.height / (density + 1);


            for (int i = 1; i <= density; i++)
            {
                Vector3 basePosition = colliderInfo.center + colliderInfo.height * 0.5f * Vector3.down + (i * dif * Vector3.up);

                Vector3 position = collider.transform.TransformPoint(basePosition);

                Vector3 direction = position - coneApex;


                if (ConeAABBIntersection.IsPointInCone(position, coneApex, coneDirection, this.coneAngle, this.coneHeight))
                {

                    if (Physics.Raycast(coneApex, direction, out RaycastHit hit, direction.magnitude, layerMask))
                    {
                        Debug.DrawRay(coneApex, hit.point - coneApex, Color.yellow);
                        bool isHit = hit.collider == collider;
                        if (isHit)
                        {
                            Debug.DrawRay(coneApex, direction, Color.green);
                            return true;
                        }
                    }
                }
            }
        }
        return false;

    }

    public bool CheckVision(ConeVisionObject coneVisionObject)
    {
        Vector3 coneApex = this.transform.position;
        Vector3 coneDirection = this.transform.forward;
        if (!ConeAABBIntersection.ConeIntersectsAABB(this.transform.position, this.transform.forward, this.coneAngle, this.coneHeight, coneVisionObject.Bounds))
        {
            Debug.DrawRay(coneApex, coneDirection, Color.red);
            return false;
        }

        foreach (var collider in coneVisionObject.Colliders)
        {
            if (ConeIntersectBoxCollider(collider))
            {
                return true;
            }
        }
        return false;
    }

    public void Start()
    {
        Light light = this.GetComponent<Light>();
        this.coneAngle = light.spotAngle * 0.5f;
        this.coneHeight = light.range;
    }

    public void Update()
    {
        if (this.coneVisionObject == null)
            return;

        CheckVision(this.coneVisionObject);

    }
}