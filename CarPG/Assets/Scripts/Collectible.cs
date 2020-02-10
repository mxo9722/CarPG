using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameObject.SendMessage("Collect", SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
        
    }
}
