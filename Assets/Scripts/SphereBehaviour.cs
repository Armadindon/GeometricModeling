using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehaviour : MonoBehaviour
{

    public Sphere sphere { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        sphere = new Sphere(gameObject.transform.position, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        sphere.center = gameObject.transform.position;

        // Update representation graphique
        gameObject.transform.localScale = new Vector3(sphere.radius*2, sphere.radius*2, sphere.radius*2);
    }

}
