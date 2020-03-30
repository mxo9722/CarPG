using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeManager : MonoBehaviour
{
    Spike[] spikes;
    GameObject car;
    // Start is called before the first frame update
    void Start()
    {
        spikes = GetComponentsInChildren<Spike>(true);
        car = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, car.transform.position);
        if(dist < 10)
        {
            for(int i=0; i < spikes.Length; i++)
            {
                spikes[i].trigger();
                if (spikes[i].spikeCooldown > 0)
                {
                    float objY = spikes[i].transform.position.y;
                    spikes[i].transform.position = new Vector3(spikes[i].transform.position.x, objY -= 0.2f, spikes[i].transform.position.z);
                }
            }
        }
    }
}
