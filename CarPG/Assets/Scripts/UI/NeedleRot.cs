using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
    public class NeedleRot : MonoBehaviour
    {

        GameObject car;
        Rigidbody cController;
        RectTransform rTransform;

        float startRot;

        public float speed = 0;

        private void Start()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");

            for(int i = 0; i < players.Length; i++)
            {
                if (players[i].transform.parent?.tag != "Player")
                {
                    car = players[i];
                    break;
                }
            }

            cController = car.GetComponent<Rigidbody>();
            rTransform = GetComponent<RectTransform>();

            startRot = GetComponent<RectTransform>().rotation.eulerAngles.z;
        }

        // Update is called once per frame
        void Update()
        {
            rTransform.rotation = Quaternion.Euler(0, 0, startRot - (cController.velocity.magnitude* 1.868f * 3.0f));
        }
    }
}