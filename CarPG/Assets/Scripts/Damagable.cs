using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{

    public float maxHealth;
    private float health;
    public float damageThreshhold;
    public float damageMultiplier;

    void Start()
    {
        health = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.impulse.magnitude + "   " + collision.relativeVelocity.magnitude);

        float impulse = collision.impulse.magnitude;

        if (collision.rigidbody == null)
        {
            impulse /= 100;
        }
        else
        {
            //impulse /= 10000;
        }

        if (impulse > damageThreshhold)
        {
            ApplyDamage((impulse - damageThreshhold) * damageMultiplier);
        }
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
    }

    void Update()
    {

    }

    void OnGUI()
    {

        GUI.Box(new Rect(10, 10, health * 3, 20), health + "/" + maxHealth);

    }
}