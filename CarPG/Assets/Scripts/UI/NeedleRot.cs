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

        float startRot;

        public float speed = 0;

        private void Start()
        {
            car = GameObject.FindWithTag("Player");
            cController = car.GetComponent<CarController>();
            rTransform = GetComponent<RectTransform>();

            startRot = GetComponent<RectTransform>().rotation.eulerAngles.z;
        }

        // Update is called once per frame
        void Update()
        {
            rTransform.rotation = Quaternion.Euler(0, 0, startRot - (cController.CurrentSpeed* 1.868f));
        }
    }
}