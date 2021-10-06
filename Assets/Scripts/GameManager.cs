using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject planPrefab;
    [SerializeField]
    private GameObject planSegment;
    [SerializeField]
    private GameObject planInterPoint;

    [SerializeField]
    private Transform p2;

    //Modèle
    private Plan p;
    private Segment s;

    private GameObject planGameObject;
    private GameObject segmentGameObject;

    private GameObject intersectionGameObject;

    // Start is called before the first frame update
    void Start()
    {
        p = new Plan(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        planGameObject =  Instantiate(planPrefab, p.normal * p.d, Quaternion.identity);

        s = new Segment(new Vector3(0, 0, 0), p2.position);
        segmentGameObject = Instantiate(planSegment, s.p1, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(intersectionGameObject);

        s = new Segment(new Vector3(-1, -1, -1), p2.position);
        segmentGameObject.transform.LookAt(s.p2);
        segmentGameObject.transform.localScale = new Vector3(1, 1, Vector3.Distance(s.p1, s.p2));

        Vector3 interPt;
        Vector3 interNormal;
        bool result = Plan.inntersectSegmentplane(s, p, out interPt, out interNormal);
        if (result) intersectionGameObject = Instantiate(planInterPoint, interPt, Quaternion.identity);
    }
}
