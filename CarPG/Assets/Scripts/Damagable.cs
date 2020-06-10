using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{

    public float maxHealth = 100;
    public float health;
    public float damageThreshhold;
    public float damageMultiplier=1;
    [HideInInspector]
    public Damagable healthPool=null;
    public bool includeChildren=false;
    private Rigidbody rb;

    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        if (rb == null||includeChildren)
        {
            Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                if (rb.gameObject != gameObject && !rb.gameObject.GetComponent<Damagable>())
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
        if(enabled)
        CollisionDamage(collision);
    }

    void CollisionDamage(Collision collision)
    {
        //Debug.Log(collision.impulse.magnitude + "   " + collision.relativeVelocity.magnitude);
        float impulse = collision.impulse.magnitude;

        Damagable damagable;

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
                if (collision.rigidbody.velocity.magnitude*collision.rigidbody.mass < rb.velocity.magnitude*rb.mass)
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

        ApplyDamage( impulse * damageMultiplier - damageThreshhold, collision );
        
    }

    public void ApplyDamage(float damage,Collision col=null)
    {
        damage = Mathf.Round(damage);

        if (healthPool!=null)
        {
            Rigidbody b;
            if (b=healthPool.gameObject.GetComponent<Rigidbody>())
            {
                healthPool.ApplyDamage(damage*rb.mass/b.mass);
            }
            else
            {
                healthPool.ApplyDamage(damage);
            }
        }
        else if (health > 0)
        {

            damage -= damageThreshhold;


            if (damage <= 0)
                return;

            health -= damage;
            
            if (gameObject.GetComponent<EnemyBehaviorScript>())
                DamageTextController.CreateDamageText(damage.ToString(), transform);

            gameObject.SendMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
            if (health <= 0)
            {
                if (col != null)
                {
                    gameObject.SendMessage("DieCollision", col, SendMessageOptions.DontRequireReceiver);
                } 
                
                SendMessage("Die",SendMessageOptions.DontRequireReceiver);
            }
        }
    }

   
}