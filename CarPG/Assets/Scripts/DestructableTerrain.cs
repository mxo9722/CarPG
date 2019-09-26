using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableTerrain : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    GameObject[] children;

    public float massOfBricks = 1;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        children = new GameObject[rigidbodies.Length];
        for(int i=0;i<rigidbodies.Length;i++)
        {
            rigidbodies[i].isKinematic = true;
            //rigidbodies[i].constraints = (RigidbodyConstraints.FreezeAll);
        }
    }

    public void Die()
    {
        for(int i= 0;i<rigidbodies.Length;i++)
        {
            rigidbodies[i].isKinematic=false;
            //rigidbodies[i].constraints = (RigidbodyConstraints.None);
            //rigidbodies[i] = massOfBricks;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
