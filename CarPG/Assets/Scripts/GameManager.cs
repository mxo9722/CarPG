using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DamageTextController.Initialize(); //This thing is basically all static methods so we just initialize it here because why not
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
