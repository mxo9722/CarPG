using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UniInputs : MonoBehaviour
{

    public static double gas;

    public static Vector2 move;

    public static Vector2 look;

    public static Cinemachine.CinemachineFreeLook camera;

    public void OnGas(InputValue context)
    {
        if (context.isPressed)
            gas = 1;
        else
            gas = 0;
    }

    public void OnMove(InputValue context)
    {
        move = context.Get<Vector2>();
    }

    public void OnLook(InputValue context)
    {
        
        look = context.Get<Vector2>();
        camera.m_XAxis.Value += look.x;
        camera.m_YAxis.Value += look.y;
    }
}
