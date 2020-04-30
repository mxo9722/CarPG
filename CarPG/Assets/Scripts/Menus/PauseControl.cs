using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{

    //public SceneAsset pauseMenu;

    public static bool MenuOpen = false;

    public static CursorLockMode hideCursor;

    public bool prevMouseVisible;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pressed = Input.GetButtonDown("Pause");

        if (pressed&&!SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.LoadSceneAsync("PauseMenu",LoadSceneMode.Additive);
            hideCursor = Cursor.lockState;
            prevMouseVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            MenuOpen = true;
        }
        else if (pressed)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("PauseMenu").buildIndex);
            Cursor.visible = prevMouseVisible;
            Cursor.lockState = hideCursor;
            
            MenuOpen = false;
        }
    }
}
