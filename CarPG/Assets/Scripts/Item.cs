using System.Collections;
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
    public ItemType itemType;
    [Header("Weapon Properties")]
    [SerializeField]
    public GameObject prefab;
    [Header("Carmor Properties")]
    [SerializeField]
    public Color carmorColor;
    [SerializeField]
    public float thresholdBonus=0;
    [SerializeField]
    public float mass = 0;

    public void DisplayItemInfo(Vector2 mouse)
    {
        GUI.Box(new Rect(mouse.x, mouse.y, 36, 24), name);
    }
}
