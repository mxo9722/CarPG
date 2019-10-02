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
    Dead
}

public class EnemyBehaviorScript : MonoBehaviour
{

    public Damagable dmg;
    public int health = 5;
    public EnemyState currentState = EnemyState.Idle;
    public float behaveRate = 1; // How often the enemy does things and looks for new things to do
    public float aggroRange = 15;
    public float attackRange = 10;
    public float speed = 250;
    public float speedLimit = 10;
    public GameObject car;

    private Renderer rend;
    private Rigidbody rb;

    private CapsuleCollider cCollider;
    private MeshCollider mCollider;

    public float behaveTimer = 0;
    public float stateTimer = 0; // How long it's been in the current state, set to 0 whenever state changes

    // Start is called before the first frame update
    void Start()
    {
        behaveTimer = Random.value * -1;
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
        cCollider = GetComponent<CapsuleCollider>();
        mCollider = GetComponent<MeshCollider>();
        car = GameObject.FindWithTag("Player");
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
<<<<<<< HEAD
                    Debug.Log(Vector3.Distance(car.transform.position, transform.position));
                    if (Vector3.Distance(car.transform.position, transform.position) < aggroRange)
=======
                    if (Mathf.Floor(stateTimer) % 3 == 0) //every 3 seconds this happens twice
                    {
                        Vector3 wander = new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1);
                        rb.velocity = Vector3.Normalize(wander) * speed;

                        rb.rotation = Quaternion.identity;

                        Quaternion rotato = Quaternion.LookRotation(wander);
                        transform.rotation = rotato;

                        rb.velocity += Vector3.up * 2;
                        behaveTimer = Random.value * -1;
                    }
                    if (Vector3.Distance(car.transform.position, transform.position) < aggroDistance)
>>>>>>> Level_Testing
                    {
                        currentState = EnemyState.Aggro;
                        stateTimer = 0;
                        behaveTimer = 1; // setting this to 1 so it starts going NOW
<<<<<<< HEAD

                        rend.material.shader = Shader.Find("_Color");
                        rend.material.SetColor("_Color", Color.yellow);

                        rend.material.shader = Shader.Find("Specular");
                        rend.material.SetColor("_SpecColor", Color.yellow);
                    }
                    break;
                case EnemyState.Aggro:
                    rb.AddForce(Vector3.Normalize(car.transform.position - transform.position) * speed);

                    if (Vector3.Distance(car.transform.position, transform.position) < attackRange)
                    {
                        currentState = EnemyState.Attack;
                        stateTimer = 0;

                        rend.material.shader = Shader.Find("_Color");
                        rend.material.SetColor("_Color", Color.red);

                        rend.material.shader = Shader.Find("Specular");
                        rend.material.SetColor("_SpecColor", Color.red);

                        rb.AddForce(Vector3.Normalize(car.transform.position - transform.position) * speed * 4.0f);
                        rb.AddForce(Vector3.up);
                    }
=======
                        
                    }
                    break;
                case EnemyState.Aggro:

                    rb.rotation = Quaternion.identity;

                    var lookPos = car.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = rotation;

                    rb.velocity = Vector3.Normalize(car.transform.position - transform.position) * speed;

                    rb.velocity += Vector3.up * 2;

                    if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 2)
                    {
                        currentState = EnemyState.Idle;
                        stateTimer = 0;
                        
                    }

>>>>>>> Level_Testing
                    break;
                case EnemyState.Attack:
                    
                    currentState = EnemyState.Idle;
                    break;
                case EnemyState.Vulnerable:

                    break;
                case EnemyState.Hit:

                    rb.constraints = (RigidbodyConstraints)0f;
                    mCollider.enabled = true;
                    cCollider.enabled = false;

                    if (rb.velocity.magnitude <= 0.03)
                    {
                        rb.constraints = (RigidbodyConstraints)80+32;

                        currentState = EnemyState.Aggro;

                        mCollider.enabled = false;
                        cCollider.enabled = true;
                    }
                    break;
                case EnemyState.Dead:
                    {
                        rb.constraints = (RigidbodyConstraints)0f;
                        mCollider.enabled = true;
                        cCollider.enabled = false;
                    }
                    break;
            }
        }
        
    }

    private void LateUpdate()
    {
        if (rb.velocity.magnitude > speedLimit)
        {
            rb.velocity = Vector3.Normalize(rb.velocity);
            rb.velocity *= speedLimit;
        }
    }

    public void TakeDamage()
    {
        currentState = EnemyState.Hit;
    }

    public void Die()
    {
        currentState = EnemyState.Dead;
    }
}
