using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleAttack
{
    public static void MakeMeeleAttack(float damage,float radius,Vector3 center,Damagable self,float force=0,string ignoreTag="")
    {
        var colliders = Physics.OverlapSphere(center, radius);
        List<Damagable> targetDam=new List<Damagable>();

        foreach (Collider collider in colliders)
        {
            Damagable dam = null;
            if ((dam = collider.gameObject.GetComponentInParent<Damagable>()) && dam != self && !targetDam.Contains(dam)&& collider.gameObject.tag!=ignoreTag) 
            {
                dam.ApplyDamage(damage);
                targetDam.Add(dam);
            }
        }

        colliders = Physics.OverlapSphere(center, radius);
        List<Rigidbody> targetRB = new List<Rigidbody>();
        Rigidbody selfBody = self.gameObject.GetComponent<Rigidbody>();

        Vector3 forceVec = (center - self.gameObject.transform.position).normalized * force;

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = null;
            if ((rb = collider.gameObject.GetComponentInChildren<Rigidbody>()) && rb != selfBody && !targetRB.Contains(rb) && collider.gameObject.tag != ignoreTag)
            {
                rb.AddForce(forceVec);
                targetRB.Add(rb);
            }
        }
    }

    public static bool ObjectWithTagWithinRange(float radius, Vector3 center, GameObject self,string tag)
    {
        var colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == tag && collider.gameObject!=self && !collider.gameObject.transform.IsChildOf(self.transform))
                return true;
        }

        return false;
    }
}
