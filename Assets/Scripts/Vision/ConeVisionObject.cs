using UnityEngine;

public class ConeVisionObject : MonoBehaviour
{

    [SerializeField, Tooltip("AABB used to check if it intersects with a vision cone before raycasting")]
    private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

    [SerializeReference, Tooltip("Colliders used to check if the cone vision sees this object")]
    private AbstractVisionCollider[] visionColliders;

    [SerializeField, Tooltip("Use this collider bounds")]
    private Collider boundsFromCollider;


    public void Start()
    {
        this.visionColliders = GetComponentsInChildren<AbstractVisionCollider>(true);
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.Bounds.center, Bounds.extents * 2);
    }

    public AbstractVisionCollider[] Colliders
    {
        get => this.visionColliders;
    }

    public Bounds Bounds
    {
        get => boundsFromCollider != null ? this.boundsFromCollider.bounds : this.bounds;
    }
}
