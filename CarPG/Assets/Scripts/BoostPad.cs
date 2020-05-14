using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicle;

public class BoostPad : MonoBehaviour
{
    VehicleController carController;

    bool boostActive = false;

    float boostTimer;
    // Start is called before the first frame update
    void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleController>();
        boostTimer = 3.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (boostActive)
        {
            boostTimer -= Time.deltaTime;
            carController.maximumVelocity = 50;
            carController.accelerationFactor = 100;

            if(boostTimer <= 0)
            {
                boostActive = false;
                carController.maximumVelocity = 25;
                carController.accelerationFactor = 50;
                boostTimer = 3.0f;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("boost trigger");
            boostActive = true;
        }
    }
}
