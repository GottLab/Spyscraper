
using System;
using UnityEngine;

public class BoxColliderHandler : ICollideHandler<BoxCollider>
{
    public float density = 4.0f;

    public void Handle(BoxCollider collider, Action<Vector3> positionHandler)
    {
        float dif = collider.size.y / (density + 1);
        for (int i = 1; i <= density; i++)
        {

            Vector3 basePosition = collider.center + collider.size.y * 0.5f * Vector3.down + (i * dif * Vector3.up);
            positionHandler.Invoke(basePosition);
        }
        
    }
}