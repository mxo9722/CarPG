using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : EnemyBehaviorScript
{
    protected override void Attack()
    {
        SetAnimation("Attacking");

        
        currentState = EnemyState.Aggro;
        stateTimer = 0;
    }

    public void AttackLand()
    {
        MeeleAttack.MakeMeeleAttack(attackDamage, cCollider.radius, transform.position + transform.forward * cCollider.radius * 2 * transform.localScale.x, health, attackForce * rb.mass, "Enemy");
    }

    protected override void Aggro()
    {
        SetAnimation("Running");


        var lookPos = car.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;

        MoveTo(car.transform.position,speed);


        if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 4)
        {
            currentState = EnemyState.Idle;
            stateTimer = 0;
        }
        else if (MeeleAttack.ObjectWithTagWithinRange(cCollider.radius, transform.position + transform.forward * cCollider.radius * 2 * transform.localScale.x, gameObject, "Player")&&stateTimer>3f)
        {
            currentState = EnemyState.Attack;
        }
    }
}
