using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : EnemyBehaviorScript
{
    protected override void Attack()
    {
        SetAnimation("Attacking");

        MeeleAttack.MakeMeeleAttack(attackDamage, cCollider.radius, transform.position + transform.forward * cCollider.radius * 2, health, attackForce*rb.mass);
        //collidersInRange[i].gameObject.GetComponentInParent<Damagable>().ApplyDamage(attackStrength);
        //collidersInRange[i].gameObject.GetComponentInParent<Rigidbody>().AddForce((collidersInRange[i].gameObject.transform.position - transform.position) * attackStrength * attackKnockback);
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
        else if (Vector3.Distance(car.transform.position, transform.position)<cCollider.radius*3)
        //TODO: add condition that defines the car is close enough
        {
            currentState = EnemyState.Attack;
            //Debug.Log("Get em, boys!");
        }
    }
}
