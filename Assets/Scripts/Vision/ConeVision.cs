using UnityEngine;

[RequireComponent(typeof(Light))]
public class ConeVision : MonoBehaviour
{

    [SerializeField, Tooltip("LayerMask for raycast vision check")]
    private LayerMask layerMask = 0;

    private Light visionLight;

    public void Start()
    {
        visionLight = this.GetComponent<Light>();
    }


    private bool ConeIntersectBoxCollider(AbstractVisionCollider collideHandler)
    {

        Vector3 coneApex = this.transform.position;
        Vector3 coneDirection = this.transform.forward;

        Collider collider = collideHandler.GetCollider();

        bool hitVisionCollider = false;

        collideHandler.ApplyPositions(this, position =>
        {
            Vector3 direction = position - coneApex;

            if (!hitVisionCollider && ConeAABBIntersection.IsPointInCone(position, coneApex, coneDirection, this.ConeAngle, this.ConeRange))
            {

                if (Physics.Raycast(coneApex, direction, out RaycastHit hit, direction.magnitude, layerMask))
                {
                    bool isHit = hit.collider == collider;
                    if (isHit)
                    {
                        Debug.DrawRay(coneApex, direction, Color.green);
                        hitVisionCollider = true;
                    }
                    else
                    {
                        Debug.DrawRay(coneApex, hit.point - coneApex, Color.yellow);

                    }
                }
            }
        });

        return hitVisionCollider;

    }


    public bool CheckVision(ConeVisionObject coneVisionObject)
    {
        Vector3 coneApex = this.transform.position;
        Vector3 coneDirection = this.transform.forward;

        if (!ConeAABBIntersection.ConeIntersectsAABB(this.transform.position, this.transform.forward, this.ConeAngle, this.ConeRange, coneVisionObject.Bounds))
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


    public float ConeAngle
    {
        get => this.visionLight.spotAngle * 0.5f;
    }

    public float ConeRange
    {
        get => this.visionLight.range;
    }
    
    public Light VisionLight
    {
        get => this.visionLight;
    }

}