﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    [SerializeField]
    public InventorySlot[] slots;
    [SerializeField]
    public Item itemHeld;
    [SerializeField]
    private Canvas _canvas;

    private InventoryApplier car;
    [SerializeField]
    public InventorySlot weaponSlot;
    [SerializeField]
    public InventorySlot carmorSlot;
    [SerializeField]
    public InventorySlot bumperSlot;

    void Start()
    {
        _canvas=gameObject.GetComponent<Canvas>();

        car = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryApplier>();

        Button[] buttons = GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(Click);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //_canvas.enabled = false;
    }

    private void OnValidate()
    {
        slots = gameObject.GetComponentsInChildren<InventorySlot>();
    }

    public void Update()
    {
        bool press = CrossPlatformInputManager.GetButtonDown("Inventory");

        if (press)
        {
            _canvas.enabled = !_canvas.enabled;
            DropItem(itemHeld);
            itemHeld = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (_canvas.enabled)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                car.SetWeapon(weaponSlot.Content?.prefab);
                car.SetCarmor(carmorSlot.Content);
                car.SetBumpers(bumperSlot.Content?.prefab);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Click()
    {
        foreach(InventorySlot slot in slots)
        {
            if (slot.clicked)
            {
                if (itemHeld)
                {
                    if (slot.Type == itemHeld.itemType || slot.Type == Item.ItemType.any)
                    {
                        slot.clicked = false;
                        Click(slot);
                    }
                }
                else
                {
                    slot.clicked = false;
                    Click(slot);
                }
                return;
            }
        }
    }

    public void Click(InventorySlot other)
    {
        Item temp = other.Content;

        other.Content = itemHeld;

        itemHeld = temp;

        if (itemHeld)
        {

            var sprite = itemHeld.sprite;

            var croppedTexture = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            Cursor.SetCursor(croppedTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void DropItem(Item i)
    {
        //TODO: add code to drop whatever item we discard
    }
}
