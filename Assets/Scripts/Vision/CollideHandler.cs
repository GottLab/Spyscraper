
using System;
using UnityEngine;

public interface ICollideHandler<T> where T : Collider
{
    public void Handle(T collider, Action<Vector3> positionHandler);
}