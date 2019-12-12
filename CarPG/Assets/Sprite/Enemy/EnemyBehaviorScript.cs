using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Aggro,
    Attack,
    Vulnerable,
    Hit,
    Dead,
    Flee
}

public class EnemyBehaviorScript : MonoBehaviour
{

    protected Damagable dmg;
    public EnemyState currentState = EnemyState.Idle;
    public float behaveRate = 1; // How often the enemy does things and looks for new things to do
    public float aggroDistance = 15;
    public float speed = 5;
    public float speedLimit = 10;
    public float attackStrength; //How much DAMAGE this dude does
    public float attackKnockback = 10000;
    public Animator anim;
    //public float attackDistance;
    protected  GameObject car;

    private Renderer rend;
    protected Rigidbody rb;

    private CapsuleCollider cCollider;
    private MeshCollider mCollider;
    private BoxCollider atkCollider;

    private List<Collider> collidersInRange; 

    public float behaveTimer = 0;
    public float stateTimer = 0; // How long it's been in the current state, set to 0 whenever state changes

    // Start is called before the first frame update
    protected void Start()
    {
        anim = GetComponent<Animator>();
        behaveTimer = Random.value * -1;
        rb = GetComponent<Rigidbody>();
        atkCollider = GetComponent<BoxCollider>();
        rend = GetComponentInChildren<Renderer>();
        cCollider = GetComponent<CapsuleCollider>();
        mCollider = GetComponent<MeshCollider>();
        car = GameObject.FindWithTag("Player");
        collidersInRange = new List<Collider>();
    }

    // Update is called once per frame
    protected void Update()
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

    protected void LateUpdate()
    {
        if (rb.velocity.magnitude > speedLimit)
        {
            rb.velocity = Vector3.Normalize(rb.velocity);
            rb.velocity *= speedLimit;
        }
    }

    protected void Idle()
    {
        if (anim)
        {
            anim.SetTrigger("Standing");
        }
        if (Mathf.Floor(stateTimer) % 3 == 0) //every 3 seconds this happens twice
        {
            Vector3 wander = new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1);
            if (anim)
                anim.SetTrigger("Walking");
            rb.velocity = Vector3.Normalize(wander) * speed;

            rb.rotation = Quaternion.identity;

            Quaternion rotato = Quaternion.LookRotation(wander);
            transform.rotation = rotato;
            
            behaveTimer = Random.value * -1;
        }
        if (Vector3.Distance(car.transform.position, transform.position) < aggroDistance)
        {
            currentState = EnemyState.Aggro;
            stateTimer = 0;
            behaveTimer = 1; // setting this to 1 so it starts going NOW

        }
    }

    protected void Aggro()
    {
        if (anim)
            anim.SetTrigger("Running");
        rb.rotation = Quaternion.identity;

        var lookPos = car.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;

        rb.velocity = Vector3.Normalize(car.transform.position - transform.position) * speed;


        if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 4)
        {
            currentState = EnemyState.Idle;
            stateTimer = 0;
        }
        else if (collidersInRange.Contains(car.GetComponentInChildren<MeshCollider>()))
        {
            currentState = EnemyState.Attack;
            //Debug.Log("Get em, boys!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        collidersInRange.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < collidersInRange.Count; i++)
        {
            if (collidersInRange[i].Equals(other))
            {
                collidersInRange.RemoveAt(i);
                break;
            }
        }
    }

    protected void Attack()
    {
        for (int i = 0; i < collidersInRange.Count; i++)
        {
            if (collidersInRange[i] == null)
            {
                collidersInRange.RemoveAt(i); //this might happen if it's destroyed by something else
            }
            else if (collidersInRange[i].gameObject.GetComponentInParent<Damagable>() != null)
            {
                if (anim)
                    anim.SetTrigger("Attacking");
                collidersInRange[i].gameObject.GetComponentInParent<Damagable>().ApplyDamage(attackStrength);
                collidersInRange[i].gameObject.GetComponentInParent<Rigidbody>().AddForce((collidersInRange[i].gameObject.transform.position - transform.position) * attackStrength * attackKnockback);
                //Debug.Log("This is happening");
            }
        }
        currentState = EnemyState.Vulnerable;
    }

    protected void Vulnerable()
    {
        behaveTimer = behaveRate - 2.0f;
        currentState = EnemyState.Aggro;
    }

    protected void Hit()
    {
        rb.constraints = (RigidbodyConstraints)0f;
        mCollider.enabled = true;
        cCollider.enabled = false;

        if (rb.velocity.magnitude <= 0.03)
        {
            rb.constraints = (RigidbodyConstraints)80 + 32;

            currentState = EnemyState.Aggro;

            mCollider.enabled = false;
            cCollider.enabled = true;
        }
    }

    protected void Dead()
    {
        rb.constraints = (RigidbodyConstraints)0f;
        mCollider.enabled = true;
        cCollider.enabled = false;
    }

    public void TakeDamage()
    {
        currentState = EnemyState.Hit;
    }

    public void Die()
    {
        currentState = EnemyState.Dead;
    }

    public void Flee()
    {
        
    }
}