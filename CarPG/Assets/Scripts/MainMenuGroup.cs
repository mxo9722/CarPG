using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGroup : MonoBehaviour
{
    public static MainMenuButton[] buttons;

    public float lastButtonChange;

    public static int curSelected = 0;

    public static int CurSelected
    {
        set
        {
            curSelected = value;
            curSelected = Mathf.Clamp(curSelected, 0, 2);

            for(int i = 0; i < buttons.Length; i++)
            {
                if (curSelected != i)
                    buttons[i].hovering = false;
                else
                    buttons[i].hovering = true;
            }
        }
        get
        {
            return curSelected;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttons = GetComponentsInChildren<MainMenuButton>();
        CurSelected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var selected=GetSelectedButton();

        if (-UniInputs.navigate.y > 0.5f && selected.rectTransform.localScale.x > selected.bigScale*0.9f)
            CurSelected++;
        else if (UniInputs.navigate.y > 0.5f && selected.rectTransform.localScale.x > selected.bigScale * 0.9f)
            CurSelected--;
    }

    private MainMenuButton GetSelectedButton()
    {
        return buttons[curSelected];
    }
}
