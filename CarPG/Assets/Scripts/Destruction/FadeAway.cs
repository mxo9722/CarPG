using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAway : MonoBehaviour
{

    public bool WaitTillDeath;
    public float fadeStartTime=0;
    public float fadeTime=5;

    public Shader alphaShader;

    private float bornTime;
    private List<Renderer> renderer;
    // Start is called before the first frame update
    void Start()
    {
        bornTime = Time.time;
        fadeStartTime += bornTime;
        fadeTime += fadeStartTime;
        renderer = new List<Renderer>(GetComponentsInChildren<Renderer>());

        if(!WaitTillDeath)
        foreach (Renderer rend in renderer)
        {
            for (int i = 0; i < rend.materials.Length; i++)
            {
                rend.materials[i].shader = alphaShader;
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!WaitTillDeath)
        {
            if (Time.time > fadeStartTime)
            {
                foreach (Renderer rend in renderer)
                {
                    for (int i=0;i<rend.materials.Length;i++)
                    {
                        rend.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        rend.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        rend.materials[i].SetInt("_ZWrite", 0);
                        rend.materials[i].DisableKeyword("_ALPHATEST_ON");
                        rend.materials[i].EnableKeyword("_ALPHABLEND_ON");
                        rend.materials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        rend.materials[i].renderQueue = 3000;

                        float percent=(Time.time-fadeStartTime)/(fadeTime-fadeStartTime);
                        var c = rend.sharedMaterials[i].GetColor("_Color");
                        c.a = 1-percent;
                        rend.materials[i].SetColor("_Color",c);
                    }
                }
            }
            if (Time.time > fadeTime)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    void Die()
    {
        if (WaitTillDeath)
        {
            foreach (Renderer rend in renderer)
            {
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    rend.materials[i].shader = alphaShader;

                }
            }
            WaitTillDeath = !WaitTillDeath;
            fadeStartTime -= fadeTime;
            fadeTime -= bornTime;
            bornTime = Time.time;
            fadeTime += bornTime;
            fadeStartTime += fadeTime;
        }
    }
}
