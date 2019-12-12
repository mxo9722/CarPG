using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PauseControl : MonoBehaviour
{

    //public SceneAsset pauseMenu;

    public static bool MenuOpen = false;

    public static CursorLockMode hideCursor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pressed = CrossPlatformInputManager.GetButtonDown("Pause");

        if (pressed&&!SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.LoadSceneAsync("PauseMenu",LoadSceneMode.Additive);
            hideCursor = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            MenuOpen = true;
        }
        else if (pressed)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("PauseMenu").buildIndex);
            Cursor.lockState = hideCursor;
            MenuOpen = false;
        }
    }
}
