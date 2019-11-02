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
                        Aggro();
                    }
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
        Projectile newFireball = Instantiate(fireballProjectile, transform.position, new Quaternion());
        newFireball.CreateProjectile(car.transform.position, projectileSpeed,""+this.gameObject.GetInstanceID());
        currentState = EnemyState.Vulnerable;
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

        rb.velocity += Vector3.up * 2;

        if (Vector3.Distance(car.transform.position, transform.position) > fleeRange)
        {
            currentState = EnemyState.Aggro;
        }
    }
    
}
