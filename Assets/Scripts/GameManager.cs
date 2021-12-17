using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Plan")]
    [SerializeField] private GameObject plan;
    private Plan math_plan;

    [Header("Segment")]
    [SerializeField] private SegmentBehaviour segment;

    [Header("Sphere")]
    [SerializeField] private GameObject sphere;
    private Sphere math_sphere;

    [Header("Cylinder")]
    [SerializeField] private GameObject cylinder;
    [SerializeField] float height;
    [SerializeField] float radius;
    private Cylinder math_cylinder;


    [Header("Intersections")]
    [SerializeField] private GameObject planInterPoint;
    private GameObject intersectionGameObject;
    [SerializeField] Color colorIntersectCylinder;


    private void Start()
    {

        math_plan = new Plan(plan.transform.forward, 0);
        math_cylinder = new Cylinder(cylinder.transform.position - cylinder.transform.up * height / 2, cylinder.transform.position + cylinder.transform.up * height / 2, radius);
        math_sphere = new Sphere(sphere.transform.position, radius);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(intersectionGameObject);

        Vector3 interPt;
        Vector3 interPt1, interPt2;
        Vector3 interNormal;

        /* PLAN */
        // Update mathematics attributes of Plan
        math_plan.normal = plan.transform.forward;
        math_plan.d = Vector3.Dot(plan.transform.position, math_plan.normal);

        /* SPHERE */
        // Update mathematics attributes of Sphere
        math_sphere.center = sphere.transform.position;

        /* CYLINDER */
        // Update mathematics attributes of Cylinder
        math_cylinder.pt1 = cylinder.transform.position - cylinder.transform.up * height / 2;
        math_cylinder.pt2 = cylinder.transform.position + cylinder.transform.up * height / 2;

        // Check if there is a collision
        //bool result = Plan.intersectSegmentplane(segment.segment, math_plan, out interPt, out interNormal);
        //bool result = Sphere.InterSegmentSphere(segment.segment, math_sphere, out interPt1, out interPt2, out interNormal);
        bool result = Cylinder.InterSegCylInf(segment.segment, math_cylinder, out interPt, out interNormal);

        // Instanciate a sphere object representing the collision
        //if (result) intersectionGameObject = Instantiate(planInterPoint, (interPt1 + interPt2)/2, Quaternion.identity);
        if (result) intersectionGameObject = Instantiate(planInterPoint, interPt, Quaternion.identity);

    }
}
