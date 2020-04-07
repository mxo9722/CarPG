using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float upY;
    public float retractY;
    float spikeCooldown;
    bool isUp;
    public bool trigger;
    float length;
    // Start is called before the first frame update
    void Start()
    {
        upY = -6.5f;
        retractY = -9.5f;
        spikeCooldown = 2.0f;
        isUp = false;
        trigger = false;
        length = Vector3.Distance(new Vector3(transform.position.x,upY), new Vector3(transform.position.x, retractY, transform.position.z));
    }

    void Update()
    {
        spikeCooldown -= Time.deltaTime;
        while(spikeCooldown < 0 && trigger == true)
        {
            Triggered(Time.deltaTime, isUp);
        }
    }

    void Triggered(float time, bool isUpCheck)
    {
        if (isUpCheck)
        {
            length = Vector3.Distance(new Vector3(transform.position.x, retractY), new Vector3(transform.position.x, upY, transform.position.z));

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = time / length;

            // Set our position as a fraction of the distance between the markers.
            gameObject.transform.position = Vector3.Lerp(new Vector3(transform.position.x, retractY, transform.position.z), new Vector3(transform.position.x, upY, transform.position.z), fractionOfJourney);
            if(gameObject.transform.position.y >= -9.5f)
            {
                isUp = false;
                spikeCooldown = 2.0f;
                trigger = false;
            }
        }
        else
        {
            length = Vector3.Distance(new Vector3(transform.position.x, upY), new Vector3(transform.position.x, retractY, transform.position.z));

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = time / length;

            // Set our position as a fraction of the distance between the markers.
            gameObject.transform.position = Vector3.Lerp(new Vector3(transform.position.x, upY, transform.position.z), new Vector3(transform.position.x, retractY, transform.position.z), fractionOfJourney);
            if (gameObject.transform.position.y <= -6.5f)
            {
                isUp = true;
                spikeCooldown = 2.0f;
                trigger = false;
            }
        }
        
    }
}
