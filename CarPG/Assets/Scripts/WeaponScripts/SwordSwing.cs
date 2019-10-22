using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SwordSwing : MonoBehaviour
{
    private HingeJoint joint;
    private Rigidbody rigidBody;
    public BoxCollider hitBox;
    private bool hitboxEnabled = false;

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
            HoldToPosition(0.49f);
        }
        else
        {
            HoldToPosition(-0.49f);
        }

        float swingSpeed = 1f;

        if (Vector3.Scale(rigidBody.angularVelocity, transform.right).magnitude > swingSpeed && pressed && !hitboxEnabled)
        {
            hitboxEnabled = true;
            hitBox.enabled=(true);
            Debug.Log("enabled");
        }
        else if(hitboxEnabled&&(Vector3.Scale(rigidBody.angularVelocity, transform.right).magnitude < swingSpeed || !pressed) )
        {
            hitboxEnabled = false;
            hitBox.enabled=(false);
            Debug.Log("disabled");
        }

    }

    public void HoldToPosition(float rot)
    {
        if (transform.localRotation.y > rot+0.01)
        {
            rigidBody.AddTorque(-75 * rigidBody.mass * transform.up);
        }
        else if(transform.localRotation.y < rot - 0.01)
        {
            rigidBody.AddTorque(75 * rigidBody.mass * transform.up);
        }
    }
}
