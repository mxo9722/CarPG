using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float power;
    public float radius;
    public float damage;

    public GameObject explosionEffect;

    public void Explode()
    {
        Debug.Log("boom!");

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {

            GameObject go = collider.gameObject;

            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
            }

            Rigidbody rb = go.GetComponentInChildren<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(power, transform.position, radius, power/10);
            }

            Damagable d = go.GetComponentInChildren<Damagable>();

            if (d != null)
            {
                float damageAmount = 1 - (Vector3.Distance(transform.position, go.transform.position) / radius);

                damageAmount *= damage;

                damageAmount -= d.damageThreshhold;

                damageAmount = Mathf.Max(0, damageAmount);

                d.ApplyDamage(damageAmount);
            }
        }

        Instantiate(explosionEffect,transform.position, Quaternion.identity);
    }
}
