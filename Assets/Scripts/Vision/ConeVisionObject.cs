using UnityEngine;

public class ConeVisionObject : MonoBehaviour
{

    [SerializeField]
    private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

    [SerializeField]
    private Collider[] colliders;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnDrawGizmosSelected()
    {
        Vector3 worldPosition = this.transform.TransformPoint(this.bounds.center);
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(worldPosition, bounds.extents * 2);
    }
}
