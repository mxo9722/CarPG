using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity;
    public string[] ignoreCollision;

    public float radius;
    public bool explode = false;
    public float damage = 0;

    public void CreateProjectile(Vector3 targetPos, float speed)
    {
        CreateProjectile(targetPos, speed, new string[] { "" });
    }

    public void CreateProjectile(Vector3 targetPos, float speed,string ignore)
    {
        CreateProjectile(targetPos, speed, new string[] { ignore });
    }

    public void CreateProjectile(Vector3 targetPos, float speed, string[] ignore)
    {
        velocity = Vector3.Normalize(targetPos - transform.position);
        velocity *= speed;

        List<string> ignoreList = new List<string>();

        ignoreList.AddRange(ignore);
        ignoreList.AddRange(ignoreCollision);

        ignoreCollision = ignoreList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        Quaternion lookQuat = Quaternion.LookRotation(velocity.normalized);
        gameObject.transform.rotation = lookQuat;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            if(collider.gameObject!=gameObject)
                Hit(collider);
        }
    }

    void Hit(Collider collision)
    {
        if (!ignoreCollision.Contains(collision.gameObject.GetInstanceID().ToString()) && !ignoreCollision.Contains(collision.gameObject.tag) && !ignoreCollision.Contains("" + collision.gameObject.layer)&&!collision.isTrigger)
        {
            if (explode)
            {
                SendMessage("Explode");
            }

            var dam = collision.GetComponent<Damagable>();
            float damage = GetDamageDealt();

            if (damage > 0 && dam != null)
            {
                dam.ApplyDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    protected virtual float GetDamageDealt()
    {
        return damage;
    } 
}
