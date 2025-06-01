using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class LightCone : MonoBehaviour
{
    public float coneAngle = 45f;
    public float rayDistance = 10f;
    public int raysPerAxis = 10;

    private Vector3[] localDirections;
    private NativeArray<RaycastCommand> rayCommands;
    private NativeArray<RaycastHit> rayResults;

    private Bounds foundBounds = new Bounds();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateLocalConeDirections();
    }

    // Update is called once per frame
    void Update()
    {
        ShootConeRaycastCommands();
    }

    void GenerateLocalConeDirections()
    {
        List<Vector3> directions = new List<Vector3>();

        Vector3 origin = Vector3.zero;

        float coneRadius = Mathf.Tan(this.coneAngle * 0.5f * Mathf.Deg2Rad) * this.rayDistance;

        // Grid on a plane in front of the origin (e.g., X-Z plane at distance `d`)
        for (int y = 0; y < raysPerAxis; y++)
        {
            for (int x = 0; x < raysPerAxis; x++)
            {
                float u = (x + 0.5f) / raysPerAxis - 0.5f;
                float v = (y + 0.5f) / raysPerAxis - 0.5f;

                Vector3 point = new(u * coneRadius * 2, v * coneRadius * 2, rayDistance);

                // Limit to circular aperture
                if (point.x * point.x + point.y * point.y > coneRadius * coneRadius)
                    continue;

                Vector3 dir = (point - origin);
                directions.Add(dir);
            }
        }
        localDirections = directions.ToArray();

        Debug.Log($"Total number of rays {localDirections.Count()}", this);
    }
    
    void ShootConeRaycastCommands()
    {
        int count = localDirections.Length;

        rayCommands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
        rayResults = new NativeArray<RaycastHit>(count, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            Vector3 worldDir = this.transform.rotation * localDirections[i];
            rayCommands[i] = new RaycastCommand(this.transform.position, worldDir, QueryParameters.Default);
        }

        // Schedule batched raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(rayCommands, rayResults, 1, default);
        handle.Complete();



        bool foundPoint = false;

        // Process results
        for (int i = 0; i < count; i++)
        {
            if (rayResults[i].collider != null)
            {
                Debug.DrawLine(this.transform.position, rayResults[i].point, Color.green);

                if (!foundPoint)
                {
                    this.foundBounds = new Bounds(rayResults[i].point, Vector3.zero);
                    foundPoint = true;
                }

                this.foundBounds.Encapsulate(rayResults[i].point);
            }
            else
            {
                Vector3 worldDir = this.transform.TransformDirection(localDirections[i]);
                Debug.DrawRay(this.transform.position, worldDir, Color.red);
            }
        }

        rayCommands.Dispose();
        rayResults.Dispose();
    }
}
