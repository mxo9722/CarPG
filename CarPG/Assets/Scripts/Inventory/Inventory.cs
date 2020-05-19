using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using Cinemachine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    [SerializeField]
    public InventorySlot[] slots;
    [SerializeField]
    private static Item[] savedItems=null;
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
    public static Item pickedUpItem;

    public float endMessage = 0;

    private Cinemachine.CinemachineFreeLook cameraController;
    private float axisMSpeedX;
    private float axisMSpeedY;
    bool InventoryOpen;
    public int money;

    void Awake()
    {
        _canvas=gameObject.GetComponent<Canvas>();

        var players = GameObject.FindGameObjectsWithTag("Player");

        SceneManager.sceneUnloaded += SaveSlots;
        
        for (int i=0;i<players.Length;i++)
        {
            applier = players[i].GetComponent<InventoryApplier>();
            if (applier != null)
            {
                break;
            }
        }

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

        if (savedItems == null)
        {
            savedItems = new Item[29];
        }
        else
        {
            slots = gameObject.GetComponentsInChildren<InventorySlot>();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Content = savedItems[i];
            }

            if (weaponSlot.Content != null)
                applier.SetWeapon(weaponSlot.Content?.prefab);
            applier.SetCarmor(carmorSlot.Content);
            if (bumperSlot.Content != null)
                applier.SetBumpers(bumperSlot.Content?.prefab);
        }

        money = 0;
    }

    private void OnValidate()
    {
        slots = gameObject.GetComponentsInChildren<InventorySlot>();
    }

    public void Update()
    {
        bool press = Input.GetButtonDown("Inventory");

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

                    applier.SetWeapon(weaponSlot.Content?.prefab);
                applier.SetCarmor(carmorSlot.Content);
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

        if (pickupItem!=null)
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
                pickedUpItem = item;
                endMessage = Time.time+5;
                return true;
            }
        }
        return false;
    }

    private void OnGUI()
    {
        if (pickedUpItem != null && endMessage>Time.time)
        {
            Rect rect = new Rect(Screen.width/2.0f-100,Screen.height/2.0f+60,240,40);
            GUI.Box(rect, "Collected the " +pickedUpItem.name+"!\nPress E to access your inventory.");
        }
    }

    void SaveSlots<Scene>(Scene scene)
    {
        print("The scene was unloaded!");
        if (PlayerPrefs.GetString("Control") == "MANUAL")
        {
            cameraController.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }
        else if (PlayerPrefs.GetString("Control") == "AUTO")
        {
            cameraController.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        }

        for (int i=0;i<slots.Length;i++)
        {
            savedItems[i] = slots[i].Content;
        }
    }
}
