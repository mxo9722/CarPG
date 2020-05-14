using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    //public SceneAsset mainMenu;

    public GameObject[] pauseObjects;

    public GameObject[] optionsObjects;

    public void ReturnToMainMenu()
    {
        Debug.Log("hi");
        //if (EditorUtility.DisplayDialog("Return To Main Menu", "Are you sure you'd like to return to the main menu? Your progress will be lost.", "Return to Main Menu", "Continue Playing"))
        //{
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        //}
    }

    public void DisplayOptions(bool goingToOptions)
    {
        for (int i = 0; i < pauseObjects.Length; i++)
        {
            pauseObjects[i].SetActive(!goingToOptions);
        }

        for (int i = 0; i < optionsObjects.Length; i++)
        {
            optionsObjects[i].SetActive(goingToOptions);
        }
    }

    public void ChangeCameraStyle()
    {
        if (optionsObjects[0].GetComponentInChildren<TextMeshProUGUI>().text == "CAMERA: MANUAL")
        {
            optionsObjects[0].GetComponentInChildren<TextMeshProUGUI>().text = "CAMERA: AUTO";
            PlayerPrefs.SetString("Control", "AUTO");
        }
        else
        {
            optionsObjects[0].GetComponentInChildren<TextMeshProUGUI>().text = "CAMERA: MANUAL";
            PlayerPrefs.SetString("Control", "MANUAL");
        }
    }

    public void MuteAudio()
    {
        AudioListener.volume = 1 - AudioListener.volume;
    }

    public void Exit()
    {
        //if (EditorUtility.DisplayDialog("Exit","Are you sure you'd like to quit? Your progress will be lost.","Exit","Continue Playing"))
        //{
            Application.Quit();
        //}
    }

    /*public void LoadOptions()
    {
        SceneManager.LoadSceneAsync(AssetDatabase.GetAssetPath(optionsMenu),LoadSceneMode.Additive);
    }*/

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene);
        PauseControl.MenuOpen = false;
        Cursor.lockState = PauseControl.hideCursor;
    }

    public void Awake()
    {
        Time.timeScale = 0;
        optionsObjects[0].GetComponentInChildren<TextMeshProUGUI>().text = "CAMERA: " + PlayerPrefs.GetString("Control");
    }

    public void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
