using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{

    public enum BossStates
    {
        BeamAttack,
        BeamCool,
        RapidFireAttack,
        RapidCool,
        CreateHealers,
        HealerCool,
        Hurt
    }

    public BossStates c_state=BossStates.BeamAttack;

    BossStates curState
    {
        get {
            return c_state;
        }
        set
        {
            c_state = value;
            rapidFireCount = 0;
            if (c_state == BossStates.RapidFireAttack)
            {
                float x = Random.value > 0.5 ? 1 : -1;
                float z = Random.value > 0.5 ? 1 : -1;
                lastPosition = new Vector3(x,0,z).normalized*circleDistance;
            }
            stateTimer = 0;
        }
     }

    private int lives = 3;
    //private
    private float stateTimer = 0;

    public GameObject eye;

    public GameObject car;

    public GameObject protectorPrefab;
    public GameObject projectile;
    public GameObject forceField;

    private LaserBeam laser;
    private LineRenderer lr;
    private Rigidbody rb;
    private Animator animator;

    public Vector3 startLoc=new Vector3(-45,7.5f,0);

    private Damagable protector1;
    private Damagable protector2;

    public float circleDistance=20f;

    public float rapidFireCount = 0;

    public float nextBlink = 3;

    public Vector3 lastPosition;

    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player");
        //startLoc = transform.position;
        laser = GetComponent<LaserBeam>();
        rb = GetComponent<Rigidbody>();
        laser.enabled = false;
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        animator = GetComponent<Animator>();
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

        nextBlink -= Time.deltaTime;

        if (nextBlink <= 0 && curState != BossStates.BeamAttack && curState != BossStates.CreateHealers)
        {

            animator.SetBool("Open", false);
            if (animator.GetCurrentAnimatorClipInfo(0).ToString() == "close hold")
            {
                nextBlink = Random.Range(3, 5);
            }
        }
        else
        {
            animator.SetBool("Open", true);
        }

        if (protector1?.health > 0|| protector2?.health > 0)
        {
            forceField.SetActive(true);
        }
        else
        {
            forceField.SetActive(false);
        }
    }

    void BeamAttack()
    {
        transform.position = startLoc;
        if (stateTimer < 8)
        {
            //Charge up;
            transform.forward = Vector3.Lerp(transform.forward,(car.transform.position - transform.position).normalized,Time.deltaTime*2.5f);
            animator.SetBool("WideEye", true);
        }
        else if(stateTimer < 12)
        {
            //fire
            laser.enabled = true;
            lr.enabled = true;
            transform.forward = Vector3.Lerp(transform.forward,(car.transform.position - transform.position).normalized,Time.deltaTime*2.5f);
        }
        else
        {
            laser.enabled = false;
            lr.enabled = false;
            curState = BossStates.BeamCool;
            animator.SetBool("WideEye", false);
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
            else if(lives == 2)
            {
                curState = BossStates.RapidFireAttack;
            }
            else
            {
                curState = BossStates.CreateHealers;
            }
        }
    }

    void RapidFireAttack()
    {
        if (stateTimer < 3)
        {
            //float angle=Mathf.Deg2Rad*(-Vector2.Angle(new Vector2(startLoc.x,startLoc.z),new Vector2(car.transform.position.x,car.transform.position.z)));
            Vector3 newPos = lastPosition;
            newPos = newPos.normalized*circleDistance; 
            transform.position = Vector3.Lerp(startLoc,newPos+startLoc,stateTimer/3.0f);
            transform.forward = Vector3.Lerp(transform.forward,(newPos).normalized,0.5f);
            //lastPosition = newPos;
        }
        else if(stateTimer <13)
        {
            //RapidFire
            Vector2 v=new Vector2(lastPosition.x,lastPosition.z);

            if (stateTimer>rapidFireCount)
            {
                rapidFireCount=Mathf.Ceil(stateTimer / 0.25f ) * 0.25f;
                Instantiate(projectile,transform.position+transform.forward*2,Quaternion.identity).GetComponent<Projectile>().CreateProjectile(car.transform.position,35,"Enemy");
            }

            var deltaX = v.x;
            var deltaY = v.y;
            var rad = Mathf.Atan2(deltaY, deltaX);

            rad += ((stateTimer - 3)/10.0f)*2*Mathf.PI;

            transform.forward = Vector3.Lerp(transform.forward, (car.transform.position - transform.position).normalized, Time.deltaTime * 2.5f);

            transform.position = new Vector3(Mathf.Cos(rad)*circleDistance,0,Mathf.Sin(rad)*circleDistance)+startLoc;
            //Debug.Log((transform.position-startLoc) + ","+lastPosition+","+rad*Mathf.Rad2Deg);
        }
        else
        {
            curState = BossStates.RapidCool;
        }
    }

    void RapidCool()
    {
        if (stateTimer < 3)
        {
            //MoveBack
            transform.position = Vector3.Lerp(lastPosition + startLoc, startLoc, (stateTimer) / 3.0f);
            transform.forward = Vector3.Lerp(transform.forward, (-lastPosition).normalized, (stateTimer) / 3.0f);
        }
        else 
        {
            curState = BossStates.BeamAttack;
        }
    }

    void CreateHealers()
    {

        if (protector1?.health <= 0)
        {
            protector1 = null;
        }

        if(protector2?.health <= 0)
        {
            protector2 = null;
        }

        if (protector1 != null && protector2 != null)
        {
            curState=BossStates.BeamAttack;
            BeamAttack();
            return;
        }

        if (stateTimer < 3)
        {
            transform.position = Vector3.Lerp(startLoc, new Vector3(0,3,0) + startLoc, stateTimer / 3.0f);
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(0,-1,0), 0.5f);
        }
        else
        {
            var protectPos = transform.position;
            protectPos.y -= 4.5f;
            protectPos.x -= 1;
            if (protector1==null)
                protector1=Instantiate(protectorPrefab, protectPos, Quaternion.identity).GetComponent<Damagable>();
            protectPos.x += 2;
            if (protector2 == null)
                protector2 = Instantiate(protectorPrefab, protectPos, Quaternion.identity).GetComponent<Damagable>();
            curState = BossStates.HealerCool;
        }
    }

    void HealerCool()
    {
        if (stateTimer > 3)
        {
            curState = BossStates.RapidFireAttack;
        }
    }

    void Hurt()
    {

        animator.SetBool("EyeWide",false);

        if (lives == 0)
        {
            GameObject.Destroy(gameObject);
        }

        if (stateTimer < 3)
        {
            laser.enabled = false;
            lr.enabled = false;
        }
        else if (stateTimer < 8)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            transform.position = Vector3.Lerp(transform.position, startLoc, (stateTimer - 3) / 5.0f);
            transform.forward = Vector3.Lerp(transform.forward, (car.transform.position - transform.position).normalized, (stateTimer - 3) / 5.0f);
        }
        else if(lives==2)
        {
            curState = BossStates.RapidFireAttack;
        }
        else
        {
            curState = BossStates.CreateHealers;
        }
    }

    private void OnCollisionEnter(Collision collision)
    { 

        if ((curState == BossStates.BeamAttack||curState==BossStates.BeamCool) && (collision.gameObject.layer==9) && !forceField.activeSelf)
        {
            //Gets hurt
            rb.isKinematic = false;
            rb.useGravity = true;

            lives--;
            curState = BossStates.Hurt;
        }
    }
}
