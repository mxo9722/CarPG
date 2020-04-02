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

    public GameObject car;

    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Car");
    }

    // Update is called once per frame
    void Update()
    {
        
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
