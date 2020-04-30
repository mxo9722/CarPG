using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUnlock : MonoBehaviour
{
    public LockedDoor door;

    private bool dead = false;

    public ParticleSystem ps1;
    public ParticleSystem ps2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Die()
    {
        DieCollision();
    }

    // Update is called once per frame
    void DieCollision()
    {
        if (!dead)
        { 
            dead = true;
            Instantiate(ps1.gameObject, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            Instantiate(ps2.gameObject, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            door.Unlock(this);
        }
    }
}
