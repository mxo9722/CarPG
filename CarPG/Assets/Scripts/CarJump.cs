using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

public class CarJump : MonoBehaviour
{

    public float jumpHeight = 500000;
    public float driftForce = 400;
    private float steering;

    private bool jumpReady = false;
    private bool jumpPressed = false;
    private bool drifting = false;

    private CarController controller;
    private Rigidbody rigidBody;

    private WheelCollider[] wheelColliders;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CarController>();
        rigidBody = gameObject.GetComponent<Rigidbody>();

        if (controller == null)
        {
            this.enabled = false;
        }

        wheelColliders = controller.WheelColliders;
    }

    private void FixedUpdate()
    {
        var pressed = CrossPlatformInputManager.GetButton("Jump");

        if (!jumpPressed && pressed)
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                WheelFrictionCurve friction = wheel.sidewaysFriction;
                friction.stiffness = 0.3f;
                wheel.sidewaysFriction = friction;
            }
        }
        else if (jumpPressed && !pressed)
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                WheelFrictionCurve friction = wheel.sidewaysFriction;
                friction.stiffness = 1.0f;
                wheel.sidewaysFriction = friction;
            }
        }

        jumpPressed = CrossPlatformInputManager.GetButton("Jump");
        steering = CrossPlatformInputManager.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        Drift();

        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            wheelColliders[i].GetGroundHit(out wheelhit);

            if (wheelhit.normal == Vector3.zero)
            {
                return;
            }
            else if ((rigidBody.velocity).y <= 0 && drifting)
            {
                drifting = false;
                //rigidBody.angularVelocity *= 2;
            }
        }

        if (jumpReady && jumpPressed)
        {
            jumpReady = false;
            rigidBody.AddRelativeForce(0, jumpHeight, 0);
            drifting = true;
            rigidBody.angularVelocity = rigidBody.angularVelocity/2;
        }

        if (!jumpPressed)
        {
            jumpReady = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null)
        {
            if (drifting)
            {
                drifting = false;
                //rigidBody.angularVelocity *= 2;
            }
        }
        else if (collision.rigidbody.isKinematic && drifting)
        {
            drifting = false;
            //rigidBody.angularVelocity *= 2;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.rigidbody)
        {
            if (drifting)
            {
                drifting = false;
                //rigidBody.angularVelocity *= 2;
            }
        }
        else if (collision.rigidbody.isKinematic && drifting)
        {
            drifting = false;
            //rigidBody.angularVelocity *= 2;
        }
    }

    void Drift()
    {
        if (drifting)
        {
            Debug.Log("drift");
            transform.Rotate(transform.up * steering * driftForce * Time.deltaTime);
        }
    }
}