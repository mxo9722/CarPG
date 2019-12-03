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

        List<Rigidbody> explosionList = new List<Rigidbody>();
        List<Damagable> damagableList = new List<Damagable>();

        foreach (Collider collider in colliders)
        {

            GameObject go = collider.gameObject;

            Rigidbody rb = null;

            while (go.transform.parent != null && rb == null)
            {
                if (go.tag != go.transform.parent.tag)
                    break;

                go = go.transform.parent.gameObject;
                rb = go.GetComponentInChildren<Rigidbody>();

            }

            if (!explosionList.Contains(rb)&&rb!=null)
            {
                rb.AddExplosionForce(power, transform.position, radius, 0.3f);
                explosionList.Add(rb);
            }

            Damagable d = go.GetComponentInChildren<Damagable>();

            if (d != null && !damagableList.Contains(d))
            {
                float damageAmount = 1 - (Vector3.Distance(transform.position, go.transform.position) / radius);

                damageAmount *= damage;

                damageAmount -= d.damageThreshhold;

                damageAmount = Mathf.Max(0, damageAmount);

                d.ApplyDamage(damageAmount);

                damagableList.Add(d);
            }
        }

        Instantiate(explosionEffect,transform.position, Quaternion.identity);
    }
}
