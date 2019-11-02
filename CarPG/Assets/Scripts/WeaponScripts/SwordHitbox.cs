using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public float explosionForce;
    public float damage;
    public GameObject explosionPos;

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.GetComponent<Rigidbody>())
            collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPos.transform.position, 1f);

        Damagable damagable;
        if (damagable = collision.gameObject.GetComponent<Damagable>())
        {
            damagable.ApplyDamage(damage);
        }
        Debug.Log("hit!");
        GetComponent<BoxCollider>().enabled = false;
    }
}
