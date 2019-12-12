using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinTest : MonoBehaviour
{

    bool testRun;
    int testEnum;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        testRun = false;
        testEnum = 0;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(testEnum > 250 && !testRun)
        {
            //testEnum = 0;
            testRun = !testRun;
            anim.SetTrigger("Running");
        }
        if (testEnum > 750)
        {
            testEnum = 0;
            //testRun = !testRun;
            anim.SetTrigger("Hit");
        }
        if (testEnum % 100 == 0)
        {
            //testEnum = 0;
            anim.SetTrigger("Attack");
        }
        testEnum++;
    }
}
