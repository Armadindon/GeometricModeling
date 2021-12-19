using UnityEngine;

public class Plan
{
    public Vector3 normal { get; set; }
    public float d { get; set; }

    public Plan(Vector3 normal, float d)
    {
        this.normal = normal;
        this.d = d;
    }

    public Plan(Vector3 normal, Vector3 P) : this(normal, Vector3.Dot(P, normal))
    { }

    public Plan(Vector3 P1, Vector3 P2, Vector3 P3) : this(Vector3.Cross(P2 - P1, P3 - P1).normalized, Vector3.Dot(P1, Vector3.Cross(P2 - P1, P3 - P1).normalized))
    { }

    public static bool intersectSegmentplane(Segment seg, Plan plan, out Vector3 interPt, out Vector3 interNormal)
    {           
        // Pour erreur compil
        interPt = Vector3.zero;
        interNormal = Vector3.zero;

        Vector3 ab = seg.p2 - seg.p1;
        float dotAbn = Vector3.Dot(ab, plan.normal);
        if (Mathf.Approximately(dotAbn, 0))
        {
            return false;
        }

        float t = (plan.d - Vector3.Dot(seg.p1, plan.normal)) / dotAbn;
        if(t < 0 || t > 1)
        {
            return false;
        }
        interPt = seg.p1 + t * ab;
        if (dotAbn < 0) interNormal = plan.normal;
        else interNormal = -plan.normal;
        return true;
    }

}

