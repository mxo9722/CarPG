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
        HealerCool,
        Hurt
    }

    private BossStates c_state=BossStates.RapidFireAttack;

    BossStates curState
    {
        get {
            return c_state;
        }
        set
        {
            c_state = value;
            stateTimer = 0;
        }
     }

    private int lives = 2;
    //private
    private float stateTimer = 0;

    public GameObject eye;

    public GameObject car;

    private LaserBeam laser;
    private LineRenderer lr;

    public Vector3 startLoc;

    public float circleDistance=20f;

    public Vector3 lastPosition;

    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player");
        startLoc = transform.position;
        laser = GetComponent<LaserBeam>();
        laser.enabled = false;
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        stateTimer += Time.deltaTime;
        switch (curState)
        {
            case BossStates.BeamAttack:
                {
                    BeamAttack();
                    break;
                }
            case BossStates.BeamCool:
                {
                    BeamCool();
                    break;
                }
            case BossStates.CreateHealers:
                {
                    CreateHealers();
                    break;
                }
            case BossStates.HealerCool:
                {
                    HealerCool();
                    break;
                }
            case BossStates.Hurt:
                {
                    Hurt();
                    break;
                }
            case BossStates.RapidCool:
                {
                    RapidCool();
                    break;
                }
            case BossStates.RapidFireAttack:
                {
                    RapidFireAttack();
                    break;
                }
        }
    }

    void BeamAttack()
    {
        if (stateTimer < 4)
        {
            //Charge up;
            transform.forward = Vector3.Lerp((car.transform.position - transform.position).normalized,transform.forward,0.95f);
        }
        else if(stateTimer < 10)
        {
            //fire
            laser.enabled = true;
            lr.enabled = true;
            transform.forward = Vector3.Lerp((car.transform.position - transform.position).normalized, transform.forward, 0.95f);
        }
        else
        {
            laser.enabled = false;
            lr.enabled = false;
            curState = BossStates.BeamCool;
        }
    }

    void BeamCool()
    {
        if (stateTimer > 3)
        {
            if (lives == 3)
            {
                curState = BossStates.BeamAttack;
            }
            else
            {
                curState = BossStates.RapidFireAttack;
            }
        }
    }

    void RapidFireAttack()
    {
        if (stateTimer < 3) {
            //float angle=Mathf.Deg2Rad*(-Vector2.Angle(new Vector2(startLoc.x,startLoc.z),new Vector2(car.transform.position.x,car.transform.position.z)));
            Vector3 newPos = (car.transform.position- startLoc);
            newPos.y = 0;
            newPos = newPos.normalized*circleDistance; 
            transform.position = Vector3.Lerp(startLoc,newPos+startLoc,stateTimer/3.0f);
            
            lastPosition = newPos;
         }
        else if(stateTimer <13)
        {
            //RapidFire
            Vector2 v=new Vector2(lastPosition.x,lastPosition.z);

            var deltaX = v.x;
            var deltaY = v.y;
            var rad = Mathf.Atan2(deltaY, deltaX);

            rad += ((stateTimer - 3)/10.0f)*2*Mathf.PI;

            transform.position = new Vector3(Mathf.Cos(rad)*circleDistance,0,Mathf.Sin(rad)*circleDistance)+startLoc;
            Debug.Log((transform.position-startLoc) + ","+lastPosition+","+rad*Mathf.Rad2Deg);
        }
        else if(stateTimer < 16)
        {
            //MoveBack
            transform.position = Vector3.Lerp(lastPosition+startLoc, startLoc, (stateTimer-13) / 3.0f);
        }
        else
        {
            curState = BossStates.RapidCool;
        }
    }

    void RapidCool()
    {
        if (stateTimer > 3)
        {
            if (lives == 2)
            {
                curState = BossStates.BeamAttack;
            }
            else
            {
                curState = BossStates.CreateHealers;
            }
        }
    }

    void CreateHealers()
    {

    }

    void HealerCool()
    {

    }

    void Hurt()
    {
        laser.enabled = false;
        lr.enabled = false;
        if (stateTimer > 3)
        {
            curState = BossStates.RapidFireAttack;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((curState == BossStates.BeamAttack||curState==BossStates.BeamCool) && collision.transform.tag=="Player")
        {
            //Gets hurt
            lives--;
            curState = BossStates.Hurt;
        }
    }
}
