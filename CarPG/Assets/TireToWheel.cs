using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireToWheel : MonoBehaviour
{
    public WheelCollider wheelCollider;

    void Start()
    {
        //wheelCollider.GetComponent<ParticleSystem>().emissionRate = 500;
    }

    void FixedUpdate()
    {
        //	transform.position = wheelCollider.su

        UpdateWheelHeight(this.transform, wheelCollider);
    }


    void UpdateWheelHeight(Transform wheelTransform, WheelCollider collider)
    {

        Vector3 localPosition = wheelTransform.localPosition;

        WheelHit hit = new WheelHit();

        // see if we have contact with ground

        if (collider.GetGroundHit(out hit))
        {

            float hitY = collider.transform.InverseTransformPoint(hit.point).y;

            localPosition.y = hitY + collider.radius;

            //wheelCollider.GetComponent<ParticleSystem>().enableEmission = true;
            if (
                    Mathf.Abs(hit.forwardSlip) >= wheelCollider.forwardFriction.extremumSlip ||
                    Mathf.Abs(hit.sidewaysSlip) >= wheelCollider.sidewaysFriction.extremumSlip
                )
            {
                //wheelCollider.GetComponent<ParticleSystem>().enableEmission = true;
            }
            else
            {
                //wheelCollider.GetComponent<ParticleSystem>().enableEmission = false;
            }


        }
        else
        {

            // no contact with ground, just extend wheel position with suspension distance

            localPosition = Vector3.Lerp(localPosition, -Vector3.up * collider.suspensionDistance, .05f);
            //wheelCollider.GetComponent<ParticleSystem>().enableEmission = false;

        }

        // actually update the position

        wheelTransform.localPosition = localPosition;

        wheelTransform.localRotation = Quaternion.Euler(0, collider.steerAngle, 0);

    }
}
