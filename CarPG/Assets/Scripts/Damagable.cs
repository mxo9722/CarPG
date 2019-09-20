using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{

    public float maxHealth = 100;
    private float health;
    public float damageThreshhold;
    public float damageMultiplier;

    void Start()
    {
        health = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        CollisionDamage(collision);
    }

    void CollisionDamage(Collision collision)
    {
        Debug.Log(collision.impulse.magnitude + "   " + collision.relativeVelocity.magnitude);

        float impulse = collision.impulse.magnitude;

        if (collision.rigidbody == null)
        {
            impulse /= 100;
        }

        if (impulse * damageMultiplier > damageThreshhold)
        {
            ApplyDamage( impulse * damageMultiplier - damageThreshhold );
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