using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardEnemy : EnemyBehaviorScript
{
    public float attackRange = 40.0f;
    public float fleeRange = 20.0f;
    public Projectile fireballProjectile;
    public float projectileSpeed = 5.0f;

    protected override void Attack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            SetAnimationTrigger("Attacking");
        var lookPos = car.transform.position - transform.position;
        lookPos.y = 0;

        Move(Vector3.zero);
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
    }

    public void CastSpell()
    {
        Projectile newFireball = Instantiate(fireballProjectile, transform.position, new Quaternion());
        newFireball.CreateProjectile(car.transform.position, projectileSpeed,""+this.gameObject.GetInstanceID());
        currentState = EnemyState.Aggro;
        SetAnimationTrigger("Standing");
    }

    protected override void Aggro()
    {
        if (Vector3.Distance(car.transform.position, transform.position) < fleeRange)
        {
            currentState = EnemyState.Flee;
        }
        else if (Vector3.Distance(car.transform.position, transform.position) < attackRange)
        {
            currentState = EnemyState.Attack;

        }
        else
        {
            PathTo(car.transform.position,acceleration);

            if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance*4)
            {
                currentState = EnemyState.Idle;
                stateTimer = 0;
            }
        }
    }

    protected override void Flee()
    {
        //rb.rotation = Quaternion.identity;

        //SetAnimationTrigger("Walking");

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
            currentState = EnemyState.Attack;
        }
    }
    
}
