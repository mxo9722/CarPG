using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UniInputs.camera = gameObject.GetComponent<Cinemachine.CinemachineFreeLook>();   
    }
}
