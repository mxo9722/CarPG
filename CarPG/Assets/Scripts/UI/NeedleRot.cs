using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleRot : MonoBehaviour
{

    GameObject car;
    Rigidbody rb;
    RectTransform rTransform;

    float startRot = 127.35f;

    private void Start()
    {
        car = GameObject.FindWithTag("Player");
        rb = car.GetComponent<Rigidbody>();
        rTransform = GetComponent<RectTransform>();

        //startRot = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        rTransform.rotation = Quaternion.Euler(0,0, startRot - (rb.velocity.magnitude * 1.975f));
    }
}
