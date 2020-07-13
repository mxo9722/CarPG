using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UniInputs : MonoBehaviour
{

    public static PlayerInput cur_PlayerInput;

    public static double gas = 0;
    public static double breaks = 0;

    public static Vector2 move = Vector2.zero;
    public static Vector2 look = Vector2.zero;

    private static UnityEvent inventoryOpen = null;

    public static UnityEvent InventoryOpen
    {
        set
        {
            inventoryOpen = value;
        }
        get
        {
            if (inventoryOpen == null)
                inventoryOpen = new UnityEvent();

            return inventoryOpen;
        }
    }

    private static UnityEvent pauseOpen = null;

    public static UnityEvent PauseOpen
    {
        set
        {
            pauseOpen = value;
        }
        get
        {
            if (pauseOpen == null)
                pauseOpen = new UnityEvent();

            return pauseOpen;
        }
    }

    //UI Controls Properties
    public static Vector2 navigate = Vector2.zero;
    public static Vector2 point = Vector2.zero;
    public static Vector2 scrollWheel = Vector2.zero;

    public static Vector3 trackedDevicePosition = Vector3.zero;

    public static Quaternion trackedDeviceOrientation = Quaternion.identity;

    private static UnityEvent submitPressed = null;

    public static UnityEvent SubmitPressed
    {
        set
        {
            submitPressed = value;
        }
        get
        {
            if(submitPressed==null)
                submitPressed = new UnityEvent();

            return submitPressed;
        }
    }

    private static UnityEvent cancelPressed = null;

    public static UnityEvent CancelPressed
    {
        set
        {
            cancelPressed = value;
        }
        get
        {
            if (cancelPressed == null)
                cancelPressed = new UnityEvent();

            return cancelPressed;
        }
    }

    private static UnityEvent clickPressed = null;

    public static UnityEvent ClickPressed
    {
        set
        {
            clickPressed = value;
        }
        get
        {
            if (clickPressed == null)
                clickPressed = new UnityEvent();

            return clickPressed;
        }
    }

    private static UnityEvent middleClickPressed = null;

    public static UnityEvent MiddleClickPressed
    {
        set
        {
            middleClickPressed = value;
        }
        get
        {
            if (middleClickPressed == null)
                middleClickPressed = new UnityEvent();

            return middleClickPressed;
        }
    }

    private static UnityEvent rightClickPressed = null;

    public static UnityEvent RightClickPressed
    {
        set
        {
            rightClickPressed = value;
        }
        get
        {
            if (rightClickPressed == null)
                rightClickPressed = new UnityEvent();

            return rightClickPressed;
        }
    }

    private void Start()
    {
        cur_PlayerInput = GetComponent<PlayerInput>();
    }

    public void OnGas(InputValue context)
    {
        if (context.isPressed)
            gas = context.Get<float>();
        else
            gas = 0;
    }

    public void OnBreaks(InputValue context)
    {
        if (context.isPressed)
            breaks = context.Get<float>();
        else
            breaks = 0;
    }

    public void OnMove(InputValue context)
    {
        move = context.Get<Vector2>();
    }

    public void OnLook(InputValue context)
    {
        look = context.Get<Vector2>();
    }

    public void OnFire(InputValue context)
    {

    }

    public void OnInventory(InputValue context)
    {
        InventoryOpen.Invoke();
    }

    public void OnPause(InputValue context)
    {
        PauseOpen.Invoke();
    }

    //UI functions

    public void OnNavigate(InputValue context)
    {
        navigate = context.Get<Vector2>();
    }

    public void OnSubmit(InputValue context)
    {
        Debug.Log("Submit!");
        submitPressed.Invoke();
    }

    public void OnCancel(InputValue context)
    {
        CancelPressed.Invoke();
    }

    public void OnPoint(InputValue context)
    {
        point = context.Get<Vector2>();
    }

    public void OnClick(InputValue context)
    {
        ClickPressed.Invoke();
    }

    public void OnScrollWheel(InputValue context)
    {
        scrollWheel = context.Get<Vector2>();
    }

    public void OnMiddleClick(InputValue context)
    {
        MiddleClickPressed.Invoke();
    }

    public void OnRightClick(InputValue context)
    {
        RightClickPressed.Invoke();
    }

    public void OnTrackedDevicePosition(InputValue context)
    {
        trackedDevicePosition = context.Get<Vector3>();
    }

    public void OnTrackedDeviceOrientation(InputValue context)
    {
        trackedDeviceOrientation = context.Get<Quaternion>();
    }
}
