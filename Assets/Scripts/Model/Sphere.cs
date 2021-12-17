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

    public static bool InterSegmentSphere(Segment seg, Sphere sph, out Vector3 interPt1, out Vector3 interPt2, out Vector3 interNormal)
    {
        interPt1 = Vector3.one;
        interPt2 = Vector3.one;
        interNormal = Vector3.one;
        Vector3 centerA = seg.p1 - sph.center;

        Vector3 ab = seg.p2 - seg.p1;
        float a = Vector3.Dot(ab, ab);
        float b = 2 * Vector3.Dot(ab, centerA);
        float c = Vector3.Dot(centerA, centerA) - (sph.radius * sph.radius);
        float t1 = 0.0f;
        float t2 = 0.0f;

        float delta = (b * b) - (4 * a * c);

        if(delta < 0)
        {
            return false;
        }
        else
        {
           t1 = -(b - Mathf.Sqrt(delta)) / (2 * a);
           t2 = -(b + Mathf.Sqrt(delta)) / (2 * a);

        }

        if((t1 < 0 || t1 > 1) && (t2 < 0 || t2 > 1)) 
        {
            return false;
        }

        if(t1 <= 1 || t1 >= 0)
        {
            interPt1 = seg.p1 + t1 * ab;
        }

        if (t2 <= 1 || t2 >= 0)
        {
            interPt2 = seg.p1 + t2 * ab;
        }

        return true;
    }

}
