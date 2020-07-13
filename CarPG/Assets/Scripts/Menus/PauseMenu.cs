using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //public SceneAsset mainMenu;

    private static Button[] pauseObjects;
    public Button[] pauseButtons;

    private static Button[] optionsObjects;
    public Button[] optionsButtons;

    static bool optionsOpen = false;

    private static int curSelected=0;

    private float timeChanged = 0;

    public int CurSelected
    {
        set
        {

            if (optionsOpen)
            {
                curSelected = Mathf.Clamp(value, 0, optionsObjects.Length);
            }
            else
            {
                curSelected = Mathf.Clamp(value, 0, pauseObjects.Length);
            }

            for (int i = 0; i < pauseObjects.Length; i++)
            {
                var selected = pauseObjects[i].GetComponentInChildren<TextMeshProUGUI>();
                if (!optionsOpen && i == curSelected)
                {
                    selected.color = Color.yellow;
                }
                else
                {
                    selected.color = Color.white;
                }
            }

            for (int i = 0; i < optionsObjects.Length; i++)
            {
                var selected = optionsObjects[i].GetComponentInChildren<TextMeshProUGUI>();
                if (optionsOpen && i == curSelected)
                {
                    selected.color = Color.yellow;
                }
                else
                {
                    selected.color = Color.white;
                }
            }
        }

        get
        {
            return curSelected;
        }
    }

    public void ReturnToMainMenu()
    {
        //if (EditorUtility.DisplayDialog("Return To Main Menu", "Are you sure you'd like to return to the main menu? Your progress will be lost.", "Return to Main Menu", "Continue Playing"))
        //{
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        //}
    }

    public void DisplayOptions(bool goingToOptions)
    {
        for (int i = 0; i < pauseObjects.Length; i++)
        {
            pauseObjects[i].gameObject.SetActive(!goingToOptions);
        }

        for (int i = 0; i < optionsObjects.Length; i++)
        {
            optionsObjects[i].gameObject.SetActive(goingToOptions);
        }

        optionsOpen = goingToOptions;
        CurSelected = 0;
    }

    public void ChangeCameraStyle()
    {
        if (optionsObjects[1].GetComponentInChildren<TextMeshProUGUI>().text == "CAMERA: MANUAL")
        {
            optionsObjects[1].GetComponentInChildren<TextMeshProUGUI>().text = "CAMERA: AUTO";
            PlayerPrefs.SetString("Control", "AUTO");
        }
        else
        {
            optionsObjects[1].GetComponentInChildren<TextMeshProUGUI>().text = "CAMERA: MANUAL";
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

    public void Accept()
    {
        if (optionsOpen)
        {
            optionsObjects[curSelected].onClick.Invoke();
        }
        else
        {
            pauseObjects[curSelected].onClick.Invoke();
        }
    }

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene);
        PauseControl.MenuOpen = false;
        Cursor.lockState = PauseControl.hideCursor;
    }

    public void Awake()
    {
        pauseObjects = pauseButtons;

        optionsObjects = optionsButtons;

        UniInputs.SubmitPressed.AddListener(Accept);

        CurSelected = 0;

        Time.timeScale = 0;
        optionsObjects[1].GetComponentInChildren<TextMeshProUGUI>().text = "CAMERA: " + PlayerPrefs.GetString("Control");
    }

    void Update()
    {
        if (UniInputs.navigate.y > 0.5f && Time.unscaledTime - timeChanged > 0.3f)
        {
            timeChanged = Time.unscaledTime;
            CurSelected = CurSelected - 1;
        }
        else if(UniInputs.navigate.y < -0.5f && Time.unscaledTime - timeChanged > 0.3f)
        {
            timeChanged = Time.unscaledTime;
            CurSelected = CurSelected + 1;
        }
    }

    public void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
