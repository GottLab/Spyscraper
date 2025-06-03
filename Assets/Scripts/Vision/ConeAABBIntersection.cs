using UnityEngine;

public static class ConeAABBIntersection
{
    /// <summary>
    /// Checks if a cone intersects with an AABB (Bounds).
    /// It may miss some edge cases where the cone intersects the box but no corner is inside and the axis ray misses.
    /// </summary>
    public static bool ConeIntersectsAABB(Vector3 coneApex, Vector3 coneDir, float angleDeg, float height, Bounds aabb)
    {
        coneDir.Normalize();
        Vector3[] corners = GetAABBCorners(aabb);

        // Check if any corner is inside the cone
        foreach (var corner in corners)
        {
            if (IsPointInCone(corner, coneApex, coneDir, angleDeg, height))
                return true;
        }
        // It's possible for the cone to be partially inside the AABB without intersecting any corners.
        // Therefore, we also check if the cone's direction ray collides with the AABB.
        if (LineIntersectsAABB(coneApex, coneDir, aabb, angleDeg, height))
            return true;

        return false;
    }

    public static bool IsPointInCone(Vector3 point, Vector3 apex, Vector3 dir, float angleDeg, float height)
    {
        Vector3 toPoint = point - apex;
        float distAlongAxis = Vector3.Dot(toPoint, dir.normalized);

        if (distAlongAxis < 0 || distAlongAxis > height)
            return false; // Outside cone height

        float cosAngle = Mathf.Cos(Mathf.Deg2Rad * angleDeg);
        float cosToPoint = Vector3.Dot(toPoint.normalized, dir.normalized);
        
        return cosToPoint >= cosAngle;
    }


    private static Vector3[] GetAABBCorners(Bounds bounds)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        corners[0] = new Vector3(min.x, min.y, min.z);
        corners[1] = new Vector3(max.x, min.y, min.z);
        corners[2] = new Vector3(min.x, max.y, min.z);
        corners[3] = new Vector3(max.x, max.y, min.z);
        corners[4] = new Vector3(min.x, min.y, max.z);
        corners[5] = new Vector3(max.x, min.y, max.z);
        corners[6] = new Vector3(min.x, max.y, max.z);
        corners[7] = new Vector3(max.x, max.y, max.z);

        return corners;
    }

    private static bool LineIntersectsAABB(Vector3 start, Vector3 direction, Bounds bounds, float angleDeg, float height)
    {
        Ray ray = new(start, direction);
        return bounds.IntersectRay(ray, out float hitDist) && IsPointInCone(start + direction * hitDist, start, direction, angleDeg, height);
    }


}
