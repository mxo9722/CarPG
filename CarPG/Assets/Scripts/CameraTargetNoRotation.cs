using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetNoRotation : MonoBehaviour
{
    public float yOffset;

    [SerializeField]
    private GameObject car;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 carPosition = car.transform.position;

        transform.position = carPosition + Vector3.up * yOffset + transform.forward;
    }
}
