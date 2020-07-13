using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;

    public float bigScale = 1.25f;
    public float scaleRate = 1f; 
    public RectTransform rectTransform;

    public MainMenuButton[] mainMenuButtons = new MainMenuButton[3];
    public int buttonIndex;
    public string sceneName;
    public PauseMenu scene;

    public bool hovering = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UniInputs.SubmitPressed.AddListener(Submit);
    }

    // Update is called once per frame
    void Update()
    {
        if (hovering)
        {
            if (rectTransform.localScale.x < bigScale)
            {
                
                rectTransform.localScale = new Vector3(rectTransform.localScale.x + Time.deltaTime * scaleRate * (bigScale - rectTransform.localScale.x), rectTransform.localScale.y + Time.deltaTime * scaleRate * (bigScale - rectTransform.localScale.y), 1);
                //rectTransform.localPosition += new Vector3(rectTransform.)
                for (int i = 0; i < mainMenuButtons.Length; i++)
                {
                    if (i == buttonIndex)
                    {
                        continue;
                    }
                    if (i > buttonIndex)
                    {
                        mainMenuButtons[i].rectTransform.localPosition -= new Vector3(0, Time.deltaTime * 100 * scaleRate * (bigScale - rectTransform.localScale.x));
                    }
                    else
                    {
                        mainMenuButtons[i].rectTransform.localPosition += new Vector3(0, Time.deltaTime * 100 * scaleRate * (bigScale - rectTransform.localScale.x));
                    }
                }
            }
        }
        else
        {
            if (rectTransform.localScale.x > 1)
            {
                rectTransform.localScale = new Vector3(rectTransform.localScale.x - Time.deltaTime * scaleRate * (rectTransform.localScale.x - 1), rectTransform.localScale.y - Time.deltaTime * scaleRate * (rectTransform.localScale.x - 1), 1);

                for (int i = 0; i < mainMenuButtons.Length; i++)
                {
                    if (i == buttonIndex)
                    {
                        continue;
                    }
                    if (i > buttonIndex)
                    {
                        mainMenuButtons[i].rectTransform.localPosition += new Vector3(0, Time.deltaTime * 100 * scaleRate * (rectTransform.localScale.x - 1));
                    }
                    else
                    {
                        mainMenuButtons[i].rectTransform.localPosition -= new Vector3(0, Time.deltaTime * 100 * scaleRate * (rectTransform.localScale.x - 1));
                    }
                }
            }
        }
        Mathf.Clamp(rectTransform.localScale.x, 1, bigScale);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        MainMenuGroup.CurSelected=buttonIndex;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Submit();
    }

    public void Submit()
    {
        Debug.Log("We here");
        if (!hovering)
            return;

        if (sceneName == "quit")
            Application.Quit();
        else if (sceneName == "resume")
            scene.UnloadScene();
        else
            SceneManager.LoadScene(sceneName);
    }
}
