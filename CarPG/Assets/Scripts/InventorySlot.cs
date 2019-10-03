using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    public Item _content = null;
    [SerializeField]
    private Item.ItemType type = Item.ItemType.any;

    public Item.ItemType Type
    {
        get
        {
            return type;
        }
    }

    [SerializeField]
    public Item Content
    {
        get
        {
            return _content;
        }
        set
        {
            _content = value;
            UpdateGraphic();
        }
    }

    public bool hover = false;

    [HideInInspector]
    public bool clicked = false;

 
    public Image itemDisplay;

    public void SetClicked(bool c)
    {
        clicked = c;
    }

    public void Start()
    {
        UpdateGraphic();
    }

    public void OnMouseEnter()
    {
        hover = true;
    }

    private void OnMouseExit()
    {
        hover = false;
    }

    private void OnGUI()
    {
        if (hover && _content)
        {
            _content.DisplayItemInfo(Input.mousePosition);
        }
    }

    void OnValidate()
    {
        UpdateGraphic();
    }

    public void UpdateGraphic()
    {
        if (_content)
            itemDisplay.sprite = _content.sprite;
        else
            itemDisplay.sprite = null;
    }
}
