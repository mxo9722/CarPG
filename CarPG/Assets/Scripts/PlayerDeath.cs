﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    public class PlayerDeath : MonoBehaviour
    {
        public static CursorLockMode hideCursor;

        public bool prevMouseVisible;
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
            SceneManager.LoadSceneAsync("DeathMenu", LoadSceneMode.Additive);
            hideCursor = Cursor.lockState;
            prevMouseVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameObject.SendMessage("Explode", SendMessageOptions.DontRequireReceiver);
            
        }
    }
}
