
using System;
using UnityEngine;


//Implementation of Vision Collider for Boxes
[RequireComponent(typeof(BoxCollider))]
public class BoxVisionCollider : VisionCollider<BoxCollider>
{

    public enum Type
    {
        Spread, //points in the 8 corners of the cube and center
        Vertical // positioned Vertially at the center of the box
    }

    [SerializeField, Tooltip("Describes the number of points when using the Vertical Type mode")]
    private float density = 4.0f;

    [SerializeField, Tooltip("Define how the raycast points are placed in the box")]
    private Type type = Type.Vertical;


    public override void Handle(BoxCollider collider, Action<Vector3> positionHandler)
    {

        switch (this.type)
        {
            case Type.Vertical:
                float dif = collider.size.y / (density + 1);
                for (int i = 1; i <= density; i++)
                {
                    Vector3 basePosition = collider.center + collider.size.y * 0.5f * Vector3.down + (i * dif * Vector3.up);
                    positionHandler.Invoke(basePosition);
                }
                break;
            case Type.Spread:
                Vector3 center = collider.center;
                Vector3 size = collider.size * 0.45f;
                positionHandler.Invoke(center + new Vector3(-size.x, -size.y, -size.z)); // Bottom-back-left
                positionHandler.Invoke(center + new Vector3(size.x, -size.y, -size.z));  // Bottom-back-right
                positionHandler.Invoke(center + new Vector3(size.x, -size.y, size.z));   // Bottom-front-right
                positionHandler.Invoke(center + new Vector3(-size.x, -size.y, size.z));  // BopositionHandler.Invoke(
                positionHandler.Invoke(center + new Vector3(-size.x, size.y, -size.z));  // Top-back-left
                positionHandler.Invoke(center + new Vector3(size.x, size.y, -size.z));   // Top-back-right
                positionHandler.Invoke(center + new Vector3(size.x, size.y, size.z));    // Top-front-right
                positionHandler.Invoke(center + new Vector3(-size.x, size.y, size.z));    // Top-front-left
                positionHandler.Invoke(center); // Center

                break;
        }


    }


}