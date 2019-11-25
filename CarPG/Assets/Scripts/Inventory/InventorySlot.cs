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

    public void Hover(bool hover)
    {
        this.hover = hover;
    }

    private void OnGUI()
    {
        if (hover && _content)
        {
            var mouse = Event.current.mousePosition;
            mouse = GUIUtility.ScreenToGUIPoint(mouse);
            _content.DisplayItemInfo(mouse);
        }
    }

    void OnValidate()
    {
        UpdateGraphic();
    }

    public void UpdateGraphic()
    {
        if (_content)
        {
            itemDisplay.sprite = _content.sprite;
            itemDisplay.enabled = true;
        }
        else
        {
            itemDisplay.sprite = null;
            itemDisplay.enabled = false;
        }
    }
}
