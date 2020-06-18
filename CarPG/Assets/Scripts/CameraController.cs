using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Cinemachine.CinemachineFreeLook freeLook;

    private void Start()
    {
        freeLook = GetComponent<Cinemachine.CinemachineFreeLook>();
    }

    private void Update()
    {
        freeLook.m_XAxis.m_InputAxisValue = UniInputs.look.x;
        freeLook.m_YAxis.m_InputAxisValue = UniInputs.look.y;
    }
}
