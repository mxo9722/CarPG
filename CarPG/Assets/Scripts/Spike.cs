using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float upY;
    public float retractY;
    float spikeCooldown;
    bool onCD;
    public bool trigger;
    // Start is called before the first frame update
    void Start()
    {
        upY = -6.5f;
        retractY = -9.5f;
        spikeCooldown = 2f;
        onCD = false;
        trigger = false;
    }

    void Update()
    {
        spikeCooldown -= Time.deltaTime;
        if (onCD == false)
        {
            float objY = gameObject.transform.position.y;
            if (objY <= upY && trigger == true)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, objY += 0.05f, gameObject.transform.position.z);
            }
            if (spikeCooldown < 0)
            {
                onCD = true;
            }
        }
        else
        {
            float objY = gameObject.transform.position.y;
            if (objY >= retractY)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, objY -= 0.05f, gameObject.transform.position.z);
            }
            else
            {
                spikeCooldown = 2f;
                onCD = false;
            }
        }
        
    }
}
