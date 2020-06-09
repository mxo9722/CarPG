using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicle;

public class BoostPad : MonoBehaviour
{
    VehicleController carController;
    Light flash;

    bool boostActive = false;

    float boostTimer;

    Rigidbody player;

    float yRotMod;

    public GameObject orientationArrows;

    // Start is called before the first frame update
    void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleController>();
        boostTimer = 3.0f;
        flash = GetComponentInChildren<Light>();
        flash.gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        yRotMod = transform.eulerAngles.y % 90;
        GameObject.Destroy(orientationArrows);
    }

    // Update is called once per frame
    void Update()
    {
        if (boostActive)
        {
            boostTimer -= Time.deltaTime;
            carController.maximumVelocity = 50;
            carController.accelerationFactor = 100;
<<<<<<< Updated upstream
            if(boostTimer <= 0)
=======

            if(boostTimer <= 0 || Input.GetAxis("Vertical") < 0)
>>>>>>> Stashed changes
            {
                boostActive = false;
                flash.gameObject.SetActive(boostActive);
                carController.maximumVelocity = 25;
                carController.accelerationFactor = 50;
                boostTimer = 3.0f;
            }
        }
        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player"&&!boostActive&&Input.GetAxis("Vertical")>0)
        {
            Debug.Log("boost trigger");
            boostActive = true;
            flash.gameObject.SetActive(boostActive);

            Debug.Log(player.transform.rotation.eulerAngles);

            float rotY = Mathf.Round((player.transform.rotation.eulerAngles.y-yRotMod) / 90.000f)*90+yRotMod;

            var nRot = player.transform.rotation.eulerAngles;

            nRot.y = rotY;

            player.transform.eulerAngles = Vector3.Lerp(nRot,player.transform.eulerAngles,0.5f);
        }
    }
}
