using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float spikeCooldown;
    bool offCD;
    // Start is called before the first frame update
    void Start()
    {
        spikeCooldown = 2f;
        offCD = true;
    }

    void Update()
    {
        if(offCD == false)
        {
            spikeCooldown -= Time.deltaTime; 
            if (spikeCooldown < 0)
            {
                offCD = true;
            }
        }
        
    }

    public void trigger()
    {
        if(offCD == true)
        {
            float objY = gameObject.transform.position.y;
            if(objY < 0.5)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, objY += 0.1f, gameObject.transform.position.z);
            }
            else
            {
                spikeCooldown = 2f;
                offCD = false;
            }
            
        }
        
    }
}
