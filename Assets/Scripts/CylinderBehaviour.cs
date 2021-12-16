using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderBehaviour : MonoBehaviour
{
    [SerializeField]
    float height;
    [SerializeField]
    float radius;

    public Cylinder cylinder { get; set; }

    //Start is called before the first frame update
    void Start()
    {
        cylinder = new Cylinder(transform.position - transform.up * height / 2, transform.position + transform.up * height / 2, radius);
    }

    //représentation graphique se base sur le model

    //Update is called once per frame
    void Update()
    {
        // Update representation graphique
        //transform.localScale = new Vector3(cylinder.radius * 2, Vector3.Distance(cylinder.pt1, cylinder.pt2) / 2, cylinder.radius * 2);
        cylinder = new Cylinder(transform.position - transform.up * height / 2, transform.position + transform.up * height / 2, radius);

        cylinder.pt1 = transform.position - transform.up * height / 2;
        cylinder.pt1 = transform.position + transform.up * height / 2;
        transform.Rotate(0, 20f * Time.deltaTime, 0, Space.Self);

    }
}