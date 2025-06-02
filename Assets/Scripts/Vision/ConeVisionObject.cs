using UnityEngine;

public class ConeVisionObject : MonoBehaviour
{

    [SerializeField, Tooltip("AABB used to check if it intersects with a vision cone before raycasting")]
    private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

    [SerializeField, Tooltip("Colliders used to check if the cone vision sees this object")]
    private Collider[] colliders;

    [SerializeField, Tooltip("Use this collider bounds")]
    private Collider boundsFromCollider;

    [SerializeField, Tooltip("Get colliders by iterating this object child")]
    private bool GetCollidersInChild = true;

    void OnValidate()
    {
        if (GetCollidersInChild)
        {
            this.colliders = GetComponentsInChildren<Collider>(true);
        }
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.Bounds.center, Bounds.extents * 2);
    }

    public Collider[] Colliders
    {
        get => this.colliders;
    }

    public Bounds Bounds
    {
        get => boundsFromCollider != null ? this.boundsFromCollider.bounds : this.bounds;
    }
}
