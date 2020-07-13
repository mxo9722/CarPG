using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField]
    public Item _content = null;
    [SerializeField]
    private Item.ItemType type = Item.ItemType.any;

    public int gridX, gridY;

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
    public Image highlight;

    private Inventory inventory;

    public void SetClicked(bool c)
    {
        clicked = c;
    }

    public void Start()
    {
        UpdateGraphic();
        inventory = GetComponentInParent<Inventory>();
    }

    public void Hover(bool hover)
    {
        this.hover = hover;
        if (hover)
            highlight.color = Color.yellow;
        else
            highlight.color = Color.gray;
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
        if (hover)
            highlight.color = Color.yellow;
        else
            highlight.color = Color.gray;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.CurSelected = this;
    }
}
