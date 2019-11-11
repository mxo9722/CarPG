﻿using System.Collections;
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
    protected  GameObject car;

    private Renderer rend;
    protected Rigidbody rb;

    private CapsuleCollider cCollider;
    private MeshCollider mCollider;

    public float behaveTimer = 0;
    public float stateTimer = 0; // How long it's been in the current state, set to 0 whenever state changes

    // Start is called before the first frame update
    protected void Start()
    {
        behaveTimer = Random.value * -1;
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
        cCollider = GetComponent<CapsuleCollider>();
        mCollider = GetComponent<MeshCollider>();
        car = GameObject.FindWithTag("Player");
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
        {
            currentState = EnemyState.Aggro;
            stateTimer = 0;
            behaveTimer = 1; // setting this to 1 so it starts going NOW

        }
    }

    protected void Aggro()
    {
        rb.rotation = Quaternion.identity;

        var lookPos = car.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;

        rb.velocity = Vector3.Normalize(car.transform.position - transform.position) * speed;

        rb.velocity += Vector3.up * 2;

        if (Vector3.Distance(car.transform.position, transform.position) > aggroDistance * 4)
        {
            currentState = EnemyState.Idle;
            stateTimer = 0;

        }
    }

    protected void Attack()
    {

    }

    protected void Vulnerable()
    {
        behaveTimer = behaveRate - 3.0f;
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