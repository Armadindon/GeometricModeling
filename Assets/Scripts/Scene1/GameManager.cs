using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Segment")]
    [SerializeField] private SegmentBehaviour segment;

    [Header("Plan")]
    [SerializeField] private GameObject plan;
    private Plan math_plan;

    [Header("Sphere")]
    [SerializeField] private GameObject sphere;
    private Sphere math_sphere;

    [Header("Cylinder")]
    [SerializeField] private GameObject cylinder;
    [SerializeField] float height;
    [SerializeField] float radius;
    private Cylinder math_cylinder;


    [Header("Intersections")]
    [SerializeField] private GameObject intersectionPointPrefab1;
    [SerializeField] private GameObject intersectionPointPrefab2;
    private GameObject intersectionGameObject1;
    private GameObject intersectionGameObject2;
    [SerializeField] Color colorIntersectCylinder;

    private ActiveShape currentActiveShape;

    private enum ActiveShape
    {
        NONE,
        PLANE,
        SPHERE,
        CYLINDER
    }


    private void Start()
    {
        math_plan = new Plan(plan.transform.forward, 0);
        math_cylinder = new Cylinder(cylinder.transform.position - cylinder.transform.up * height / 2, cylinder.transform.position + cylinder.transform.up * height / 2, radius);
        math_sphere = new Sphere(sphere.transform.position, radius);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(intersectionGameObject1);
        Destroy(intersectionGameObject2);

        Vector3 interPt = Vector3.zero;
        Vector3 interPt1 = Vector3.zero, interPt2 = Vector3.zero;
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
        bool result;
        switch (currentActiveShape)
        {
            case ActiveShape.PLANE:
                result = Plan.intersectSegmentplane(segment.segment, math_plan, out interPt, out interNormal);
                if (result) intersectionGameObject1 = Instantiate(intersectionPointPrefab1, interPt, Quaternion.identity);
                break;
            case ActiveShape.SPHERE:
                result = Sphere.InterSegmentSphere(segment.segment, math_sphere, out interPt1, out interPt2, out interNormal);
                if (result) intersectionGameObject1 = Instantiate(intersectionPointPrefab1, interPt1, Quaternion.identity);
                if (result) intersectionGameObject2 = Instantiate(intersectionPointPrefab2, interPt2, Quaternion.identity);
                break;
            case ActiveShape.CYLINDER:
                result = Cylinder.InterSegCylInf(segment.segment, math_cylinder, out interPt, out interNormal);
                if(result) intersectionGameObject1 = Instantiate(intersectionPointPrefab1, interPt, Quaternion.identity);
                break;
        }

    }

    public void showPlane()
    {
        currentActiveShape = ActiveShape.PLANE;
        plan.SetActive(true);
        sphere.SetActive(false);
        cylinder.SetActive(false);
    }

    public void showSphere()
    {
        currentActiveShape = ActiveShape.SPHERE;
        plan.SetActive(false);
        sphere.SetActive(true);
        cylinder.SetActive(false);
    }

    public void showCylinder()
    {
        currentActiveShape = ActiveShape.CYLINDER;
        plan.SetActive(false);
        sphere.SetActive(false);
        cylinder.SetActive(true);
    }

    public void clear()
    {
        currentActiveShape = ActiveShape.NONE;
        plan.SetActive(false);
        sphere.SetActive(false);
        cylinder.SetActive(false);
    }

    public void changeScene()
    {
        SceneManager.LoadScene(1);
    }
}
