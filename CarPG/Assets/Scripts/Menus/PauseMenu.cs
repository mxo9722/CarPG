using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public SceneAsset optionsMenu;
    public SceneAsset mainMenu;

    

    public void ReturnToMainMenu()
    {
        if (EditorUtility.DisplayDialog("Return To Main Menu", "Are you sure you'd like to return to the main menu? Your progress will be lost.", "Return to Main Menu", "Continue Playing"))
        {
            SceneManager.LoadScene(AssetDatabase.GetAssetPath(mainMenu), LoadSceneMode.Single);
        }
    }

    public void Exit()
    {
        if (EditorUtility.DisplayDialog("Exit","Are you sure you'd like to quit? Your progress will be lost.","Exit","Continue Playing"))
        {
            Application.Quit();
        }
    }

    public void LoadOptions()
    {
        SceneManager.LoadSceneAsync(AssetDatabase.GetAssetPath(optionsMenu),LoadSceneMode.Additive);
    }

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene);
        PauseControl.MenuOpen = false;
        Cursor.lockState = PauseControl.hideCursor;
    }

    public void Awake()
    {
        Time.timeScale = 0;
    }

    public void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
