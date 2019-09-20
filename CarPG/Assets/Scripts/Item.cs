using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{ 
    public string name;
    public Sprite sprite;

    public void DisplayItemInfo(Vector2 mouse)
    {
        GUI.Box(new Rect(mouse.x, mouse.y, 36, 24), name);
    }
}
