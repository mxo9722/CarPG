using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;

    public float bigScale = 1.25f;
    public float scaleRate = 1f; 
    private RectTransform rectTransform;

    public MainMenuButton[] mainMenuButtons = new MainMenuButton[3];
    public int thisButtonIndex;
    public string sceneName;
    public PauseMenu scene;

    public bool hovering = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
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
                    if (i == thisButtonIndex)
                    {
                        continue;
                    }
                    if (i > thisButtonIndex)
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
                    if (i == thisButtonIndex)
                    {
                        continue;
                    }
                    if (i > thisButtonIndex)
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
        hovering = true;
        
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovering = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (sceneName == "quit")
            Application.Quit();
        else if (sceneName == "resume")
            scene.UnloadScene();
        else
            SceneManager.LoadScene(sceneName);
    }
}
