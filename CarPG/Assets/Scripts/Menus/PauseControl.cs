using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PauseControl : MonoBehaviour
{

    public SceneAsset pauseMenu;

    public static bool MenuOpen = false;

    CursorLockMode hideCursor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pressed = CrossPlatformInputManager.GetButtonDown("Pause");

        if (pressed&&!SceneManager.GetSceneByName(pauseMenu.name).isLoaded)
        {
            SceneManager.LoadSceneAsync(AssetDatabase.GetAssetPath(pauseMenu),LoadSceneMode.Additive);
            hideCursor = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            MenuOpen = true;
        }
        else if (pressed)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(pauseMenu.name).buildIndex);
            Cursor.lockState = hideCursor;
            MenuOpen = false;
        }
    }
}
