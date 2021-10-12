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


    private GameObject intersectionGameObject;

    // Update is called once per frame
    void Update()
    {
        Destroy(intersectionGameObject);  

        Vector3 interPt;
        Vector3 interNormal;
        bool result = Plan.inntersectSegmentplane(segment.segment, plan.plan, out interPt, out interNormal);
        if (result) intersectionGameObject = Instantiate(planInterPoint, interPt, Quaternion.identity);
    }
}
