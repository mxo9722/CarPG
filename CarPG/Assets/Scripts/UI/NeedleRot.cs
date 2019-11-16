using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
    public class NeedleRot : MonoBehaviour
    {

        GameObject car;
        CarController cController;
        RectTransform rTransform;

        float startRot = 127.35f;

        private void Start()
        {
            car = GameObject.FindWithTag("Player");
            cController = car.GetComponent<CarController>();
            rTransform = GetComponent<RectTransform>();

            //startRot = transform.rotation.z;
        }

        // Update is called once per frame
        void Update()
        {
            rTransform.rotation = Quaternion.Euler(0, 0, startRot - (cController.CurrentSpeed * 1.975f));
        }
    }
}