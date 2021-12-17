using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform p2;
    public Segment segment { get; set; }

    void Start()
    {
        segment = new Segment(transform.position, p2.position);
    }

    void Update()
    {
        // On met ? jour le mod?le
        segment.p1 = transform.position;
        segment.p2 = p2.position;

        // On adapte la r?pr?sentation
        gameObject.transform.LookAt(segment.p2);
        gameObject.transform.localScale = new Vector3(1, 1, Vector3.Distance(segment.p1, segment.p2));

    }
}
