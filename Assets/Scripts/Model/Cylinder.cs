using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder
{
    public Vector3 pt1 { get; set; }
    public Vector3 pt2 { get; set; }
    public float radius { get; set; }

    public Cylinder(Vector3 pt1, Vector3 pt2, float radius)
    {
        this.pt1 = pt1;
        this.pt2 = pt2;
        this.radius = radius;
    }

    public static bool InterSegCylInf(Segment seg, Cylinder cyl, out Vector3 interPt1, out Vector3 interPt2, out Vector3 interNormal)
    {
        interPt1 = new Vector3();
        interPt2 = new Vector3();
        interNormal = new Vector3();

        Vector3 AB = seg.p2 - seg.p1;
        Vector3 PA = cyl.pt1 - seg.p2;
        Vector3 PQ = cyl.pt2 - cyl.pt1;

        float a = AB.magnitude * AB.magnitude - (Vector3.Dot(PQ, AB) * Vector3.Dot(PQ, AB)) / (PQ.magnitude * PQ.magnitude);
        float b = 2 * Vector3.Dot(PA, AB) - 2 * ((Vector3.Dot(AB, PQ) * Vector3.Dot(PA, PQ)) / (PQ.magnitude * PQ.magnitude));
        float c = Vector3.Dot(PA, PA) - ((Vector3.Dot(PA, PQ) * Vector3.Dot(PA, PQ)) / (PQ.magnitude * PQ.magnitude));

        float delta = (b * b) - 4 * a * c;

        float racine1;
        float racine2;

        if (delta < 0)
        {
            return false;
        }
        else
        {
            racine1 = (-b - Mathf.Sqrt(delta)) / 2 * a;
            racine2 = (-b + Mathf.Sqrt(delta)) / 2 * a;
        }

        float t1 = racine1;
        float t2 = racine2;
        interPt1 = (seg.p1 / 2) + t1 * AB + seg.p2;
        interPt2 = (seg.p2 / 2) + t2 * AB + seg.p1;

        return true;
    }
}