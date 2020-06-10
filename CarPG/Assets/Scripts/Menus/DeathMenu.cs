using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Application.Quit();
    }

    public void Replay()
    {
        //Replay code will go here
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    /*public void LoadOptions()
    {
        SceneManager.LoadSceneAsync(AssetDatabase.GetAssetPath(optionsMenu),LoadSceneMode.Additive);
    }*/
}
