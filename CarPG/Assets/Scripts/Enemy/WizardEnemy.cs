using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemy : EnemyBehaviorScript
{
    public float attackRange = 40.0f;
    public float fleeRange = 20.0f;
    public Projectile fireballProjectile;
    public float projectileSpeed = 5.0f;

    // Update is called once per frame
    new void Update()
    {
        behaveTimer += Time.deltaTime;
        stateTimer += Time.deltaTime;

        if (behaveTimer > behaveRate)
        {
            behaveTimer = 0;

            switch (currentState)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Aggro:
                    Aggro();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Vulnerable:
                    Vulnerable();
                    break;
                case EnemyState.Hit:
                    Hit();
                    break;
                case EnemyState.Dead:
                    Dead();
                    break;
                case EnemyState.Flee:
                    Flee();
                    break;
            }
        }
    }
    
    new void Attack()
    {
        var lookPos = car.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
        Projectile newFireball = Instantiate(fireballProjectile, transform.position, new Quaternion());
        newFireball.CreateProjectile(car.transform.position, projectileSpeed,""+this.gameObject.GetInstanceID());
        currentState = EnemyState.Vulnerable;
    }

    new void Aggro()
    {
        if (Vector3.Distance(car.transform.position, transform.position) < fleeRange)
        {
            stateTimer = 0.0f;
            currentState = EnemyState.Flee;
        }
        else if (Vector3.Distance(car.transform.position, transform.position) < attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else
        {
            rb.rotation = Quaternion.identity;

            var lookPos = car.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;

            rb.velocity = Vector3.Normalize(car.transform.position - transform.position) * speed * 2;

            rb.velocity += Vector3.up * 5;

            if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 4)
            {
                currentState = EnemyState.Idle;
                stateTimer = 0;
            }
        }
    }

    new void LateUpdate()
    {
        base.LateUpdate();
    }

    new void Flee()
    {
        rb.rotation = Quaternion.identity;

        var lookPos = transform.position - car.transform.position ;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;

        rb.velocity = Vector3.Normalize(transform.position - car.transform.position) * speed;

        rb.velocity += Vector3.up * 5;

        if (stateTimer > 6.0f || Vector3.Distance(car.transform.position, transform.position) > fleeRange)
        {
            lookPos = car.transform.position - transform.position;
            lookPos.y = 0;
            rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
            currentState = EnemyState.Attack;
        }
    }
    
}
