using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : EnemyBehaviorScript
{
    protected override void Attack()
    {
        SetAnimation("Attacking");

        MeeleAttack.MakeMeeleAttack(attackDamage, cCollider.radius, transform.position + transform.forward * cCollider.radius * 2 * transform.localScale.x, health, attackForce*rb.mass, "Enemy");
        currentState = EnemyState.Vulnerable;
    }

    protected override void Aggro()
    {
        SetAnimation("Running");


        var lookPos = car.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;

        rb.AddForce(Vector3.Normalize(car.transform.position - transform.position) * speed * rb.mass/Time.deltaTime);


        if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 4)
        {
            currentState = EnemyState.Idle;
            stateTimer = 0;
        }
        else if (MeeleAttack.ObjectWithTagWithinRange(cCollider.radius, transform.position + transform.forward * cCollider.radius * 2 * transform.localScale.x, gameObject, "Player"))
        {
            currentState = EnemyState.Attack;
        }
    }
}
