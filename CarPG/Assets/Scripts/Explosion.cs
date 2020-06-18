using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float power;
    public float radius;
    public float damage;

    public GameObject explosionEffect;

    public bool explodeOnDeath = false;

    void Explode()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        List<Rigidbody> explosionList = new List<Rigidbody>();
        List<Damagable> damagableList = new List<Damagable>();

        foreach (Collider collider in colliders)
        {

            GameObject go = collider.gameObject;

            Rigidbody rb = null;

            while (go.transform.parent != null&&!go.GetComponent<Damagable>())
            {
                if (go.layer != go.transform.parent.gameObject.layer)
                    break;

                go = go.transform.parent.gameObject;
                rb = go.GetComponentInChildren<Rigidbody>();

            }

            Debug.Log(go.name);

            Damagable d = go.GetComponent<Damagable>();
            //if(d==null)
                //d = go.GetComponentInChildren<Damagable>();

            if (d != null && !damagableList.Contains(d))
            {
                float damageAmount = 1 - (Vector3.Distance(transform.position, collider.ClosestPointOnBounds(transform.position)) / radius);

                damageAmount *= damage;

                d.ApplyDamage(damageAmount);

                damagableList.Add(d);
            }
        }

        colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = null;

            rb = collider.gameObject.GetComponent<Rigidbody>();

            if (!explosionList.Contains(rb) && rb != null)
            {
                rb.AddExplosionForce(power*rb.mass, transform.position, radius, 1f);
                explosionList.Add(rb);
            }
        }

        Instantiate(explosionEffect,transform.position, Quaternion.identity);
    }

    void DieCollision()
    {
        Die();
    }

    void Die()
    {
        if (explodeOnDeath)
        {
            Explode();
        }
    }
}
