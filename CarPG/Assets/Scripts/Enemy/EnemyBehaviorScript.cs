using System;
using System.Collections;
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
    public float acceleration = 5;
    public float maxSpeed = 10;
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
    public NavMeshAgent agent;

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
        behaveTimer = UnityEngine.Random.value * -1;
        
        car = GameObject.FindWithTag("Player");
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.speed = maxSpeed*2;
        agent.acceleration = acceleration*4;
        agent.baseOffset = cCollider.height / 2.0f;

        SetRagDoll(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Vector3.Distance(transform.position, car.transform.position) > 250)
            return;

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

        

        float horVel = new Vector2(rb.velocity.x,rb.velocity.z).magnitude;

        SetAnimationSpeeds(horVel);

        agent.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void LateUpdate()
    {
        
    }

    protected virtual void StandingUp()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("StandUp")&&!anim.GetNextAnimatorStateInfo(0).IsName("StandUp"))
        { 
            currentState = EnemyState.Aggro;
            joint.massScale = 1;
        }
    }


    protected virtual void Idle()
    {
        if (!agent.isStopped)
        {
            PathTo(idleWalkTarget, acceleration/2.0f,maxSpeed/2.0f);
        }

        if (Mathf.Floor(stateTimer) % 6 == 0&&IsGrounded()) //every 3 seconds this happens twice
        {
            idleWalkTarget.x = float.PositiveInfinity;
            float tries = 0;
            while (float.IsPositiveInfinity(idleWalkTarget.x)&&tries<9)
            {
                tries++;
                float walkRadius = 5;
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
                Vector3 finalPosition = hit.position;
                
                idleWalkTarget = finalPosition;
            }
            if (float.IsPositiveInfinity(idleWalkTarget.x))
            {
                idleWalkTarget = transform.position;
            }
            else
            {
                PathTo(idleWalkTarget, acceleration);
            }

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

    public void SetAnimationSpeeds(float horizontalSpeed)
    {
        if (anim)
            if (anim.enabled)
            {
                anim.SetFloat("HorizontalSpeed",horizontalSpeed);
            }
    }

    public void SetAnimationTrigger(string setting)
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

        target = target - transform.position;


        target.Normalize();
        target *= speed;
        target += rb.velocity;

        Move(target);
    }

    public virtual bool PathTo(Vector3 target,float speed,float maxSpeed=-1)
    {
        if (!agent.enabled)
            return false;

        if (agent.destination != target)
            agent.destination = target;

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return false;

        Vector3 movement = agent.transform.position - transform.position;
        //Debug.Log(movement);

        //movement.y = 0;

        if (movement.magnitude !=0)
        {
            movement/=agent.speed*Time.fixedDeltaTime;
            movement *= speed;
            movement += rb.velocity;           
        }

        Move(movement, maxSpeed);

        return true;
    }

    public virtual void Move(Vector3 targetVelocity, float maxSpeed=-1)
    {
        if (IsGrounded())
        {
            if (maxSpeed == -1)
            {
                maxSpeed = this.maxSpeed;
            }

            if (targetVelocity.magnitude > maxSpeed)
            {
                targetVelocity = targetVelocity.normalized;
                targetVelocity *= maxSpeed;
            }

            //var velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

            var velocityChange = targetVelocity - rb.velocity;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            if (targetVelocity.magnitude > 0.05f)
            {
                targetVelocity.y = 0;
                Quaternion rotato = Quaternion.LookRotation(targetVelocity.normalized);
                transform.rotation = rotato;
            }
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
                    {
                        body.mass *= 100;
                        body.constraints = RigidbodyConstraints.None;
                    }
                }
                
                joint.massScale = 1;
                cCollider.enabled = false;
                anim.enabled = false;
                agent.enabled = false;

                currentState = EnemyState.Hit;
                //rb.constraints = (RigidbodyConstraints)0f;
            }
            else
            {
                foreach (Collider col in bodyColliders)
                {
                    col.enabled = false;
                    var body = col.gameObject.GetComponent<Rigidbody>();
                    if (body != null && body != rb)
                    {
                        body.mass /= 100;
                        body.constraints = RigidbodyConstraints.FreezeAll;
                    }
                }

                joint.massScale = 0.1f;

                var pos = gameObject.transform.localPosition;
                pos.y += cCollider.height / 2.0f * gameObject.transform.lossyScale.y;
                gameObject.transform.localPosition = pos;
                
                cCollider.enabled = true;
                
                anim.enabled = true;
                agent.enabled = true;
                
                currentState = EnemyState.StandingUp;
            }

            ragDoll = rd;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, cCollider.bounds.extents.y + cCollider.radius / 2.0f);
    }
}