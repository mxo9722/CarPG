using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    private static DamageText popupText;
    private static GameObject canvas;


    public static void Initialize()
    {
        canvas = GameObject.Find("UI");
        if (!popupText)
            popupText = Resources.Load<DamageText>("PopupTextParent");

        Debug.Log((bool)popupText);
    }

    public static void CreateDamageText(string text, Transform location)
    {
        DamageText instance = Instantiate(popupText);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPosition;
        instance.SetText(text);
    }
}
