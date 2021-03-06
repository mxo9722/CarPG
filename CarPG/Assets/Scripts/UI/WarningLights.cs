﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningLights : MonoBehaviour
{

    public Image[] warningLights;

    public Damagable car; 

    // Start is called before the first frame update
    void Awake()
    {
        
        var c = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject ob in c)
        {
            if (ob.GetComponent<Damagable>() != null)
            {
                car = ob.GetComponent<Damagable>();
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        int curHealth = (int)Mathf.Floor((1 - car.health / car.maxHealth) * (warningLights.Length+1));
        bool blink = (int)(Time.fixedTime) % 2 == 0;

        for (int i = 0; i < warningLights.Length; i++)
        {
            if (i + 1 > curHealth)
            {
                warningLights[i].color = new Color(1, 1, 1, 0.3f);
            }
            else
            {
                
                if (curHealth > 3 && blink)
                {
                    warningLights[i].color = new Color(1, 1, 1, 0.3f);
                }
                else
                {
                    warningLights[i].color = new Color(1, 1, 1, 1f);
                }
            }
        }
    }
}
