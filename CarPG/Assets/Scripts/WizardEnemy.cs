using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemy : EnemyBehaviorScript
{
    public float attackRange = 10.0f;
    public Projectile fireballProjectile;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }  

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
                    if (Vector3.Distance(car.transform.position, transform.position) < attackRange)
                    {
                        currentState = EnemyState.Attack;
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
            }
        }
    }
    
    new void Attack()
    {
        Projectile newFireball = Instantiate(fireballProjectile, transform.position, new Quaternion());
        newFireball.CreateProjectile(car.transform.position, 5.0f);
        currentState = EnemyState.Vulnerable;
    }

    new void LateUpdate()
    {
        base.LateUpdate();
    }
    
}
