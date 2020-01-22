﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Aggro,
    Attack,
    Vulnerable,
    Hit,
    Dead,
    Flee,
    StandingUp
}

public class EnemyBehaviorScript : MonoBehaviour
{

    protected Damagable health;
    public EnemyState currentState = EnemyState.Idle;
    public float behaveRate = 1; // How often the enemy does things and looks for new things to do
    public float aggroDistance = 15;
    public float speed = 5;
    public float speedLimit = 10;
    public float attackDamage; //How much DAMAGE this dude does
    public float attackForce = 100;
    protected Animator anim;
    //public float attackDistance;
    protected  GameObject car;

    protected Joint joint;
    protected Renderer rend;
    protected Rigidbody rb;

    protected CapsuleCollider cCollider;

    protected List<Collider> bodyColliders;
    protected bool ragDoll = true;
    public Vector3 idleWalkTarget=Vector3.zero;

    public float behaveTimer = 0;
    public float stateTimer = 0; // How long it's been in the current state, set to 0 whenever state changes

    

    // Start is called before the first frame update
    protected void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponentInChildren<Renderer>();
        cCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Damagable>();
        joint = GetComponent<Joint>();

        bodyColliders = new List<Collider>(GetComponentsInChildren<Collider>());
        behaveTimer = Random.value * -1;
        
        car = GameObject.FindWithTag("Player");

        SetRagDoll(false);
    }

    // Update is called once per frame
    void Update()
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
                case EnemyState.StandingUp:
                    StandingUp();
                    break;
            }
        }

    }

    protected virtual void StandingUp()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("StandUp")&&!anim.GetNextAnimatorStateInfo(0).IsName("StandUp"))
        { 
            currentState = EnemyState.Aggro;
            joint.massScale = 1;
        }
    }

    protected void LateUpdate()
    {

        Vector2 moveSpeed = new Vector2(rb.velocity.x, rb.velocity.z);

        if (rb.velocity.magnitude > speedLimit && IsGrounded() && !ragDoll)
        {
            moveSpeed = moveSpeed.normalized*speedLimit;

            //rb.velocity = new Vector3(moveSpeed.x,rb.velocity.y,moveSpeed.y);
        }
    }

    protected virtual void Idle()
    {
        if(rb.velocity.magnitude<0.3&& !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            SetAnimation("Standing");

        if (Vector3.Distance(transform.position, idleWalkTarget) > 1)
        {
            MoveTo(idleWalkTarget, speed);
            SetAnimation("Walking");
        }

        if (Mathf.Floor(stateTimer) % 3 == 0&&IsGrounded()) //every 3 seconds this happens twice
        {
            idleWalkTarget.x = float.PositiveInfinity;
            float tries = 0;
            while (float.IsPositiveInfinity(idleWalkTarget.x)&&tries<9)
            {
                tries++;
                float walkRadius = 5;
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
                Vector3 finalPosition = hit.position;
                
                idleWalkTarget = finalPosition;
            }
            if (float.IsPositiveInfinity(idleWalkTarget.x))
                idleWalkTarget = transform.position;

            stateTimer = 1;
        }

        if (Vector3.Distance(car.transform.position, transform.position) < aggroDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, car.transform.position - transform.position, out hit) && hit.transform.tag != "Wall") {
                currentState = EnemyState.Aggro;
                stateTimer = 0;
                behaveTimer = 1; // setting this to 1 so it starts going NOW
            }
        }
    }

    protected virtual void Aggro()
    {
        
    }

    protected virtual void Attack()
    {
        
        
    }

    protected virtual void Vulnerable()
    {
        behaveTimer = behaveRate - 2.0f;
        currentState = EnemyState.Aggro;
    }

    protected virtual void Hit()
    {

        if (rb.velocity.magnitude <= 0.03)
        {
            if (stateTimer > 3)
            {
                currentState = EnemyState.StandingUp;
                SetRagDoll(false);
            }
        }
        else 
        {
            stateTimer = 0;
        }


    }

    protected virtual void Dead()
    {
        rb.constraints = (RigidbodyConstraints)0f;
        SetRagDoll(true);
    }

    public virtual void TakeDamage()
    {
        currentState = EnemyState.Hit;
        stateTimer = 0;
        SetRagDoll(true);
    }

    public virtual void Die()
    {
        currentState = EnemyState.Dead;
    }

    protected virtual void Flee()
    {
        
    }

    public void SetAnimation(string setting)
    {
        if (anim)
            if(anim.enabled)
            {
                anim.SetTrigger(setting);
            }
    }

    public void MoveTo(Vector3 target,float speed)
    {
        target.y = this.transform.position.y;
        Move((target-this.transform.position).normalized*speed);
    }

    public void Move(Vector3 targetVelocity)
    {
        if (IsGrounded())
        {
            var velocity = rb.velocity;

            var velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -speedLimit, speed);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -speedLimit, speed);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            velocity.y = 0;

            Quaternion rotato = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = rotato;
        }
    }

    public void SetRagDoll(bool rd)
    {
        if (anim && ragDoll!=rd)
        {
            Debug.Log("Ragdoll set to "+rd);

            if (rd)
            {
                foreach(Collider col in bodyColliders)
                {
                    col.enabled = true;
                    var body=col.gameObject.GetComponent<Rigidbody>();
                    if (body != null && body != rb)
                        body.mass *= 100;
                }
                
                joint.massScale = 1;
                cCollider.enabled = false;
                SetAnimation("StandingUp");
                anim.enabled = false;
                //rb.constraints = (RigidbodyConstraints)0f;
            }
            else
            {
                foreach (Collider col in bodyColliders)
                {
                    col.enabled = false;
                    var body = col.gameObject.GetComponent<Rigidbody>();
                    if (body != null && body != rb)
                        body.mass /= 100;
                }

                joint.massScale = 0.1f;

                var pos = gameObject.transform.localPosition;
                pos.y += cCollider.height / 2.0f * gameObject.transform.localScale.y;
                gameObject.transform.localPosition = pos;
                
                cCollider.enabled = true;
                
                anim.enabled = true;
                
                currentState = EnemyState.StandingUp;
            }

            ragDoll = rd;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, cCollider.bounds.extents.y + 0.1f);
    }
}