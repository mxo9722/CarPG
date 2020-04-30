using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUnlock : MonoBehaviour
{
    public GameObject door;

    public ParticleSystem ps1;
    public ParticleSystem ps2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Die()
    {
        Instantiate(ps1, gameObject.transform.position, Quaternion.identity);
        Instantiate(ps2, gameObject.transform.position, Quaternion.identity);
        door.GetComponent<LockedDoor>().Unlock(gameObject);
    }
}
