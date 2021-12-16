using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject planInterPoint;

    [SerializeField]
    private SegmentBehaviour segment;
    [SerializeField]
    private PlanBehaviour plan;
    [SerializeField]
    private GameObject real_plan_gameobject;
    [SerializeField]
    private SphereBehaviour sphere;
    [SerializeField]
    private CylinderBehaviour cylinder;

    private Plan math_plan;

    private GameObject intersectionGameObject;

    private void Start()
    {

        math_plan = new Plan(real_plan_gameobject.transform.forward, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(intersectionGameObject);  

        Vector3 interPt;
        Vector3 interPt1, interPt2;
        Vector3 interNormal;



       // real_plan_gameobject.transform.Rotate(20f * Time.deltaTime, 0, 0, Space.Self);
        math_plan.normal = real_plan_gameobject.transform.forward;
        math_plan.d = Vector3.Dot(real_plan_gameobject.transform.position, math_plan.normal);


        bool result = Plan.inntersectSegmentplane(segment.segment, math_plan, out interPt, out interNormal);
        //bool result = Sphere.InterSegmentSphere(segment.segment, sphere.sphere, out interPt, out interNormal);
        //bool result = Cylinder.InterSegCylInf(segment.segment, cylinder.cylinder, out interPt1, out interPt2, out interNormal);

        if (result) intersectionGameObject = Instantiate(planInterPoint, interPt, Quaternion.identity);

        //if (result) intersectionGameObject = Instantiate(planInterPoint, interPt2, Quaternion.identity);
     }
}
