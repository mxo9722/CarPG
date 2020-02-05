using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class DeathMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        //if (EditorUtility.DisplayDialog("Return To Main Menu", "Are you sure you'd like to return to the main menu? Your progress will be lost.", "Return to Main Menu", "Continue Playing"))
        //{
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
        //}
    }

    public void Exit()
    {
        //if (EditorUtility.DisplayDialog("Exit","Are you sure you'd like to quit? Your progress will be lost.","Exit","Continue Playing"))
        //{
        Application.Quit();
        //}
    }

    public void Replay()
    {
        //Replay code will go here
    }

    /*public void LoadOptions()
    {
        SceneManager.LoadSceneAsync(AssetDatabase.GetAssetPath(optionsMenu),LoadSceneMode.Additive);
    }*/
}
