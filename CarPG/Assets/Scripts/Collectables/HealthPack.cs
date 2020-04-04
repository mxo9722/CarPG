using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Collectible
{
    private Damagable car;
    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player").GetComponent<Damagable>();
    }

    // Update is called once per frame
    void Update()
    {
        float degreesPerSecond = 50.0f;
        transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self);
    }

    void Collect()
    {
        if((car.health+50) >= car.maxHealth)
        {
            car.health = car.maxHealth;
        }
        else
        {
            car.health += 50;
        }
        
    }
}
