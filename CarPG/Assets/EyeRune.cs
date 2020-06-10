using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRune : MonoBehaviour
{
    public LockedDoor door;
    public int lCount;

    private bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (door.locks < lCount&&!done)
        {
            done = true;

            var renderer = gameObject.GetComponent<Renderer>();


            //Call SetColor using the shader property name "_Color" and setting the color to red
            renderer.material.SetColor("_Color", Color.yellow);
        }
    }
}
