using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HammerSwing : MonoBehaviour
{
    private HingeJoint joint;
    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<HingeJoint>();
        rigidBody = GetComponent<Rigidbody>();
        joint.connectedBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool pressed = CrossPlatformInputManager.GetButton("Fire3");

        if (pressed)
        {
            HoldToPosition(110);
        }
        else
        {
            HoldToPosition(0);
        }
    }

    public void HoldToPosition(float rot)
    {
        if (transform.localRotation.x > rot+0.01)
        {
            rigidBody.AddTorque(-50 * rigidBody.mass * transform.right);
        }
        else if(transform.localRotation.x < rot - 0.01)
        {
            rigidBody.AddTorque(50 * rigidBody.mass * transform.right);
        }
    }
}
