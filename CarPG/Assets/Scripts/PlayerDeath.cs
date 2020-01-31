using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
    public class PlayerDeath : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
          
        }

        void Die()
        {
            if (GetComponent<CarUserControl>() != null)
            {
                GetComponent<CarUserControl>().enabled = false;
            }
            if (GetComponent<CarJump>() != null)
            {
                GetComponent<CarJump>().enabled = false;
            }
            gameObject.SendMessage("Explode", SendMessageOptions.DontRequireReceiver);
        }
    }
}
