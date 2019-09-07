using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

public class CarJump : MonoBehaviour
{

    private bool jumpReady = false;
    private bool jumpPressed = false;

    private CarController controller;
    private Rigidbody rigidBody;

    private WheelCollider[] m_WheelColliders;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CarController>();
        rigidBody = gameObject.GetComponent<Rigidbody>();

        if (controller == null)
        {
            this.enabled = false;
        }

        m_WheelColliders = controller.WheelColliders;

    }

    private void FixedUpdate()
    {
        jumpPressed = CrossPlatformInputManager.GetButtonDown("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        if(jumpReady && jumpPressed)
        {
            jumpPressed = false;
            rigidBody.AddRelativeForce(0, 500000, 0);
            Debug.Log("Hi");
        }

        if (!jumpPressed)
        {
            jumpReady = true;
        }
    }
}
