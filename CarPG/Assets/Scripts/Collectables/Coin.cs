using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Inventory inv;
    // Start is called before the first frame update
    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        float degreesPerSecond = 50.0f;
        transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void Collect()
    {
        inv.money++;
    }
}
