using System.Collections;
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

    private InventoryApplier applier;

    private GameObject car;
    [SerializeField]
    public InventorySlot weaponSlot;
    [SerializeField]
    public InventorySlot carmorSlot;
    [SerializeField]
    public InventorySlot bumperSlot;
    [SerializeField]
    public float pickupDistance = 3;
    [SerializeField]
    public static GameObject pickupItem;
    [SerializeField]
    
    private Cinemachine.CinemachineFreeLook cameraController;
    private float axisMSpeedX;
    private float axisMSpeedY;
    bool InventoryOpen;

    void Awake()
    {
        _canvas=gameObject.GetComponent<Canvas>();

        applier = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryApplier>();

        Button[] buttons = GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(Click);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //_canvas.enabled = false;

        cameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<Cinemachine.CinemachineFreeLook>();

        car = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnValidate()
    {
        slots = gameObject.GetComponentsInChildren<InventorySlot>();
    }

    public void Update()
    {
        bool press = CrossPlatformInputManager.GetButtonDown("Inventory");

        if (PauseControl.MenuOpen)
            press = false;

        if (press)
        {
            _canvas.enabled = !_canvas.enabled;
            DropItem(itemHeld);
            itemHeld = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (Cursor.lockState==CursorLockMode.Locked)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                axisMSpeedX = cameraController.m_XAxis.m_MaxSpeed;
                axisMSpeedY = cameraController.m_YAxis.m_MaxSpeed; 

                cameraController.m_XAxis.m_MaxSpeed = 0;
                cameraController.m_YAxis.m_MaxSpeed = 0;

                InventoryOpen = true;

            }
            else
            {
                Cursor.visible = false;

                cameraController.m_XAxis.m_MaxSpeed = axisMSpeedX;
                cameraController.m_YAxis.m_MaxSpeed = axisMSpeedY;

                Cursor.lockState = CursorLockMode.Locked;

                InventoryOpen = false;

                if(weaponSlot.Content!=null)
                    applier.SetWeapon(weaponSlot.Content?.prefab);
                applier.SetCarmor(carmorSlot.Content);
                if(bumperSlot.Content!=null)
                    applier.SetBumpers(bumperSlot.Content?.prefab);
                
            }
        }

        var items = GameObject.FindGameObjectsWithTag("ItemDrop");

        pickupItem = null;

        float closest = pickupDistance;

        foreach(GameObject go in items)
        {

            float distance = Vector3.Distance(go.transform.position, car.transform.position);

            if (distance < closest)
            {
                closest = distance;
                pickupItem = go;
            }
        }

        if (CrossPlatformInputManager.GetButtonDown("PickUp")&&pickupItem!=null)
        {
            if (PickUpItem(pickupItem.GetComponent<ItemHolder>().GetContent()))
            {
                GameObject.Destroy(pickupItem);
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

        Vector3 position = car.transform.position;

        position.y += 1;

        ItemHolder.CreateItemDrop(position,i);

    }

    public bool PickUpItem(Item item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Content == null)
            {
                slots[i].Content = item;
                return true;
            }
        }
        return false;
    }

    private void OnGUI()
    {
        if (pickupItem != null)
        {
            Rect rect = new Rect(Screen.width/2.0f-100,Screen.height/2.0f+60,200,20);
            string key = "Q"; //You cannot get the bound key as of now. There may be a way to get it using an eternal tool.
            GUI.Box(rect, "Press "+key+" to pick up "+pickupItem.name);
        }
    }
}
