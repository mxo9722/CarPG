﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{ 

    public enum ItemType
    {
        eBumper,
        eCarmor,
        eMisc,
        eWeapon,
        eOther,
        any
    }

    [SerializeField]
    public string name;
    [SerializeField]
    public Sprite sprite;
    [SerializeField]
    private int itemType;

    public ItemType IType
    {
        get
        {
            return (ItemType)itemType;
        }
        set
        {
            itemType = (int)value;
        }
    }

    public void DisplayItemInfo(Vector2 mouse)
    {
        GUI.Box(new Rect(mouse.x, mouse.y, 36, 24), name);
    }
}
