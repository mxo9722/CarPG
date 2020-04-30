using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Protector : EnemyBehaviorScript
{
    public float attackRange = 40.0f;
    public float fleeRange = 20.0f;

    public void CastSpell()
    {
        //Projectile newFireball = Instantiate(fireballProjectile, transform.position, new Quaternion());
        //newFireball.CreateProjectile(car.transform.position, projectileSpeed,""+this.gameObject.GetInstanceID());
        //currentState = EnemyState.Aggro;
        //SetAnimationTrigger("Standing");
    }

    protected override void Idle()
    {
        PathTo(new Vector3(-45, 1, 0), maxSpeed);

        if (Vector3.Distance(car.transform.position, transform.position) > attackRange)
        {
            currentState = EnemyState.Aggro;
        }
    }

    protected override void Aggro()
    {
        if (Vector3.Distance(car.transform.position, transform.position) < fleeRange)
        {
            currentState = EnemyState.Flee;
        }
        else if (Vector3.Distance(car.transform.position, transform.position) < attackRange)
        {
            currentState = EnemyState.Aggro;

        }
        if (Vector3.Distance(car.transform.position, transform.position) > attackRange*2)
        {
            currentState = EnemyState.Idle;
        }
    }

    protected override void Flee()
    {
        var lookPos = transform.position - car.transform.position;
        lookPos.y = 0;
        lookPos = lookPos.normalized * fleeRange;
        lookPos += car.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(lookPos, out hit, fleeRange, 0);

        Debug.DrawLine(transform.position, lookPos);

        if (NavMesh.SamplePosition(lookPos, out hit, fleeRange, NavMesh.AllAreas))
        {
            PathTo(hit.position,acceleration);
        }

        if (Vector3.Distance(car.transform.position, transform.position) > fleeRange)
        {
            currentState = EnemyState.Idle;
        }
    }
    
}
