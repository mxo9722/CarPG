using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
    public class PlayerDeath : MonoBehaviour
    {
        public bool dead;
        // Start is called before the first frame update
        void Start()
        {
            dead = false;

        }

        // Update is called once per frame
        void Update()
        {
            if (dead == false)
            {
                if (GetComponent<Damagable>() != null)
                {
                    float h = GetComponent<Damagable>().health;
                    if (h <= 0)
                    {
                        dead = true;
                        Death();
                    }
                }
            }
        }

        void Death()
        {
            if (GetComponent<CarUserControl>() != null)
            {
                GetComponent<CarUserControl>().enabled = false;
            }
            if (GetComponent<CarJump>() != null)
            {
                GetComponent<CarJump>().enabled = false;
            }
            Vector3 pos = GetComponent<Transform>().position;
            Instantiate(GetComponent<Damagable>().fireball, pos, Quaternion.identity);
        }
    }
}
