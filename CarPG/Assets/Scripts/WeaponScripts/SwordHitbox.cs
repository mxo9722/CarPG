using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public Weapon weapon;
    public GameObject explosionPos;

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.GetComponent<Rigidbody>())
            collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(weapon.pushForce, explosionPos.transform.position, 1f);

        Damagable damagable;
        if (damagable = collision.gameObject.GetComponent<Damagable>())
        {
            damagable.ApplyDamage(weapon.damage);
        }
        Debug.Log("hit!");
        GetComponent<BoxCollider>().enabled = false;
    }
}
