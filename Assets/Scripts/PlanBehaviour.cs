using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanBehaviour : MonoBehaviour
{
    public Plan plan { get; set; }


    void Start()
    {
        plan = new Plan(new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
    }

    void Update()
    {
        // On repositionne le plan en fonction du vecteur normal
        gameObject.transform.rotation = Quaternion.LookRotation(plan.normal);
    }
}
