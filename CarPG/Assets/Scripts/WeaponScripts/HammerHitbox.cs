using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHitbox : MonoBehaviour
{

    public float explosionForce;
    public float damage;
    public GameObject explosionPos;

    private void OnTriggerEnter(Collider collision)
    {

        if(collision.gameObject.GetComponent<Rigidbody>())
           collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPos.transform.position, 1f);

        Damagable damagable;
        if (damagable=collision.gameObject.GetComponent<Damagable>())
        {
            damagable.ApplyDamage(damage);
            if (damagable.health <= 0 && collision.gameObject.tag == "Enemy")
            {
                Vector3 scale = collision.gameObject.transform.localScale;
                scale.y /= 10.0f;
                collision.gameObject.transform.localScale=scale;
            }
           
        }
        Debug.Log("hit!");
        GetComponent<Collider>().enabled = false;
    }
}
