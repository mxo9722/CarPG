using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{

    enum BossStates
    {
        BeamAttack,
        BeamCool,
        RapidFireAttack,
        RapidCool,
        CreateHealers,
        HealerCool
    }

    private BossStates curState;

    private int lives = 3;

    public GameObject eye;

    public GameObject car;

    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward=Vector3.Lerp(transform.forward, car.transform.position - transform.position,0.2f*Time.deltaTime);
    }

    void BeamAttack()
    {

    }

    void BeamCool()
    {

    }

    void RapidFireAttack()
    {

    }

    void RapidCool()
    {

    }

    void CreateHealers()
    {

    }

    void HealerCool()
    {

    }
}
