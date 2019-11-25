using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{

    public float maxHealth = 100;
    public float health;
    public float damageThreshhold;
    public float damageMultiplier;
    [HideInInspector]
    public Damagable healthPool=null;

    void Start()
    {
        health = maxHealth;
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                if (rb.gameObject != gameObject)
                {
                    Damagable damagable = rb.gameObject.AddComponent<Damagable>();
                    damagable.maxHealth = maxHealth;
                    damagable.health = health;
                    damagable.damageThreshhold = damageThreshhold;
                    damagable.damageMultiplier = damageMultiplier;
                    damagable.healthPool = this;
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CollisionDamage(collision);
    }

    void CollisionDamage(Collision collision)
    {
        //Debug.Log(collision.impulse.magnitude + "   " + collision.relativeVelocity.magnitude);
        float impulse = collision.impulse.magnitude;

        Damagable damagable;
        Rigidbody rb;
        rb = GetComponent<Rigidbody>();

        

        impulse /= rb.mass;
        //Debug.Log(impulse);

        if (collision.rigidbody == null)
        {
            //impulse /= 50;
        }
        else
        {
            if ((damagable = collision.gameObject.GetComponent<Damagable>()) != null && rb != null)
            {
                if (collision.rigidbody.velocity.magnitude < rb.velocity.magnitude)
                {
                    impulse /= 2;
                }
            }
        }

        var cushion=collision.gameObject.GetComponent<Cushioned>();

        if (cushion)
        {
            if (!cushion.takeDamage)
                impulse = 0;
            else
                impulse /= cushion.impulseDivider;
        }

        cushion = collision.contacts[0].thisCollider.gameObject.GetComponent<Cushioned>();
       if (cushion)
       {
            impulse /= cushion.impulseDivider;
        }

        ApplyDamage( impulse * damageMultiplier - damageThreshhold );
        
    }

    public void ApplyDamage(float damage)
    {

        damage -= damageThreshhold;

        if (damage <= 0)
            return;

        if (healthPool!=null)
        {
            healthPool.ApplyDamage(damage);
        }
        else if (health > 0)
        {
            health -= damage;
            gameObject.SendMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
            if (health <= 0)
            {
                gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void Update()
    {

    }

    
}