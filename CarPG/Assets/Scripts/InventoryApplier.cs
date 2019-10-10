using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class InventoryApplier : MonoBehaviour
{

    private CarController controller;
    private GameObject weaponObject;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CarController>();
    }

    public void SetWeapon(GameObject obj)
    {
        if (weaponObject != null)
        {
            Destroy(obj);
        }

        if (obj != null)
        {
            weaponObject = Instantiate(obj,transform);
            weaponObject.transform.SetParent(gameObject.transform);
        }
        else
        {
            weaponObject = null;
        }
    }
}
