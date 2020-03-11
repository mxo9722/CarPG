using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HeadLights : MonoBehaviour
{

    bool activated = false;

    Light[] lights;
    Renderer[] renderers;

    void Start()
    {
        lights = GetComponentsInChildren<Light>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {

        if (CrossPlatformInputManager.GetButtonDown("Headlights"))
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
