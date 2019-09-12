﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Aggro,
    Attack,
    Vulnerable,
    Dead
}

public class EnemyBehaviorScript : MonoBehaviour
{
    public int health = 5;
    public EnemyState currentState = EnemyState.Idle;
    public float behaveRate = 1; // How often the enemy does things and looks for new things to do
    public float aggroDistance = 15;
    public float speed = 250;
    public GameObject car;

    private Renderer rend;
    private Rigidbody rb;

    public float behaveTimer = 0;
    public float stateTimer = 0; // How long it's been in the current state, set to 0 whenever state changes

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
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
                    Debug.Log(Vector3.Distance(car.transform.position, transform.position));
                    if (Vector3.Distance(car.transform.position, transform.position) < aggroDistance)
                    {
                        currentState = EnemyState.Aggro;
                        stateTimer = 0;
                        behaveTimer = 1; // setting this to 1 so it starts going NOW

                        rend.material.shader = Shader.Find("_Color");
                        rend.material.SetColor("_Color", Color.red);

                        rend.material.shader = Shader.Find("Specular");
                        rend.material.SetColor("_SpecColor", Color.red);
                    }
                    break;
                case EnemyState.Aggro:

                    rb.AddForce(Vector3.Normalize(car.transform.position - transform.position) * speed);
                    break;
                case EnemyState.Attack:

                    break;
                case EnemyState.Vulnerable:

                    break;
                case EnemyState.Dead:

                    break;
            }
        }
    }
}