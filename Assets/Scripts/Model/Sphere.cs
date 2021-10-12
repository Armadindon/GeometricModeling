using UnityEngine;

public class Sphere
{
    public Vector3 center { get; set; }
    public float radius { get; set; }

    public Sphere(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public static bool InterSegmentSphere(Segment seg, Sphere sph, out Vector3 interPt, out Vector3 interNormal)
    {
        interPt = Vector3.one;
        interNormal = Vector3.one;
        Vector3 centerA = seg.p1 - sph.center;

        Vector3 ab = seg.p2 - seg.p1;
        float a = Vector3.Dot(ab, ab);
        float b = 2 * Vector3.Dot(ab, centerA);
        float c = Vector3.Dot(centerA, centerA) - (sph.radius * sph.radius);

        float delta = (b * b) - (4 * a * c);

        if(delta < 0)
        {
            return false;
        }


        float t = -(b + Mathf.Sqrt(delta)) / (4 * a);

        if (t < 0 || t > 1)
        {
            return false;
        }

        interPt = seg.p1 + t * ab;
        
        return true;
    }

}
