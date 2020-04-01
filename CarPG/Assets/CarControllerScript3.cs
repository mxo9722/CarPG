using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Axel
{
    Front,
    Rear
}

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider collider;
    public Axel axel;
}

public class CarControllerScript3 : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float turnSensitivity = 1.0f;
    [SerializeField]
    private float maxSteerAngle = 45.0f;
    [SerializeField]
    private Vector3 centerOfMass;
    [SerializeField]
    private List<Wheel> wheels;

    private float inputX, inputY;

    private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = centerOfMass;
    }

    // Update is called once per frame
    private void Update()
    {
        AnimateWheels();
        GetInputs();
    }

    private void LateUpdate()
    {
        Move();
        Turn();
    }

    private void GetInputs()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.collider.motorTorque = inputY * maxAcceleration * 500 * Time.deltaTime;
        }
    }

    private void Turn()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                float steerAngle = inputX * turnSensitivity * maxSteerAngle;
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steerAngle, 0.5f);
            }
        }
    }

    private void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;
            wheel.collider.GetWorldPose(out position, out rotation);
            wheel.model.transform.position = position;
            wheel.model.transform.rotation = rotation;
        }
    }
}
