using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : EnemyBehaviorScript
{
    protected override void Attack()
    {
        SetAnimationTrigger("Attacking");

        
        currentState = EnemyState.Aggro;
        stateTimer = 0;
    }

    public void AttackLand()
    {
        MeeleAttack.MakeMeeleAttack(attackDamage, cCollider.radius * transform.lossyScale.x, transform.position + transform.forward * cCollider.radius * 2 * transform.lossyScale.x, health, attackForce * rb.mass, "Enemy");
    }

    protected override void Aggro()
    {
        if (rb.velocity.magnitude > 1)
        { 
            SetAnimationTrigger("Running");
        }
        else
        {
            SetAnimationTrigger("Idle");
        }


        if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 4)
        {
            currentState = EnemyState.Idle;
            stateTimer = 0;
        }
        else if (MeeleAttack.ObjectWithTagWithinRange(cCollider.radius * transform.lossyScale.x, transform.position + (car.transform.position - transform.position).normalized * cCollider.radius * 2 * transform.lossyScale.x, gameObject, "Player"))
        {
            if (stateTimer > 3f)
            {
                currentState = EnemyState.Attack;
                var targ = car.transform.position;
                targ.y = transform.position.y;
                Quaternion rotato = Quaternion.LookRotation(targ - transform.position);
                transform.rotation = rotato;
            }
            Move(new Vector3());
        }
        else
        {
            PathTo(car.transform.position, acceleration);
        }
    }
}
