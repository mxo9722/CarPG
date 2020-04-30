using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLights : MonoBehaviour
{

    bool activated = false;

    Light[] lights;
    Renderer[] renderers;

    void Start()
    {
        lights = GetComponentsInChildren<Light>();

        foreach(Light light in lights)
        {
            light.enabled = false;
        }

        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {

        if (Input.GetButtonDown("Headlights"))
        {
            if (activated)
            {
                for(int i = 0; i < 2; i++)
                {
                    lights[i].enabled = false;
                    renderers[i].enabled = false;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    lights[i].enabled = true;
                    renderers[i].enabled = true;
                }
            }
            activated = !activated;
        }
    }
}
