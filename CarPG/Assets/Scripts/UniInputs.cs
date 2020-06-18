using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UniInputs : MonoBehaviour
{

    public static double gas = 0;
    public static double breaks = 0;

    public static Vector2 move = Vector2.zero;
    public static Vector2 look = Vector2.zero;

    public static UnityEvent inventoryOpen = new UnityEvent();
    public static UnityEvent pauseOpen = new UnityEvent();

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
        inventoryOpen.Invoke();
    }

    public void OnPause(InputValue context)
    {
        pauseOpen.Invoke();
    }
}
