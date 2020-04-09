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

    private bool animationEnded = false;

    public List<GameObject> chains;
    public GameObject car;

    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (animationEnded)
        {

        }
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
