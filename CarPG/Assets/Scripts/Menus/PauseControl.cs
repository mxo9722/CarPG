using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PauseControl : MonoBehaviour
{
    public CinemachineFreeLook cfl;
    //public SceneAsset pauseMenu;

    public static bool MenuOpen = false;

    public static CursorLockMode hideCursor;

    public bool prevMouseVisible;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("Control") == "")
        {
            PlayerPrefs.SetString("Control", "MANUAL");
        }
        else if (PlayerPrefs.GetString("Control") == "MANUAL")
        {
            cfl.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }
        else 
        {
            cfl.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        }

        UniInputs.pauseOpen.AddListener(PauseOpen);
    }

    public void PauseOpen()
    {
        if (!SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.LoadSceneAsync("PauseMenu", LoadSceneMode.Additive);
            hideCursor = Cursor.lockState;
            prevMouseVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            MenuOpen = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("PauseMenu").buildIndex);
            Cursor.visible = prevMouseVisible;
            Cursor.lockState = hideCursor;

            if (PlayerPrefs.GetString("Control") == "MANUAL")
            {
                cfl.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            }
            else
            {
                cfl.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            }

            MenuOpen = false;
        }
    }
}
