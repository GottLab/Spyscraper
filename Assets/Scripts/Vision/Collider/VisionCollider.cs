
using System;
using UnityEngine;


// A vision collider defines a collider that can be seen by a cone-based vision system.
// It provides the points in world-space that the cone vision component will use for raycasting.
public abstract class AbstractVisionCollider : MonoBehaviour
{
    public abstract void ApplyPositions(ConeVision coneVision, Action<Vector3> positionHandler);

    public abstract Collider GetCollider();
}

// VisionCollider defines the type of collider it supports.
// By using generics, you can specify properties specific to that collider type.
public abstract class VisionCollider<T> : AbstractVisionCollider where T : Collider
{

    protected T targetCollider;

    public void Start()
    {
        this.targetCollider = GetComponent<T>();
        this.Init();
    }

    protected virtual void Init()
    {
    }

    public abstract void Handle(T collider, Action<Vector3> positionHandler);

    public override void ApplyPositions(ConeVision coneVision, Action<Vector3> positionHandler)
    {
        if (targetCollider is T tCollider)
        {
            Handle(this.targetCollider, localPosition => positionHandler.Invoke(this.transform.TransformPoint(localPosition)));
        }
    }

    public override Collider GetCollider()
    {
        return this.targetCollider;
    }

    void OnDrawGizmos()
    {

        this.Handle(GetComponent<T>(), (position) =>
        {
            Gizmos.DrawSphere(this.transform.TransformPoint(position), 0.05f);
        });
    }
}
