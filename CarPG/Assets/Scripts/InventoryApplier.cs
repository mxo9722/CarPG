using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class InventoryApplier : MonoBehaviour
{

    private CarController controller;
    private Damagable damagable;
    private GameObject weaponObject;
    private Rigidbody rigidbody;
    private Item carmor;

    public Material carColor;

    private Color originalColor;

    public Transform bumperFrontPos;
    public Transform bumperBackPos;

    private GameObject[] bumpers;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CarController>();
        damagable = GetComponent<Damagable>();
        rigidbody = GetComponent<Rigidbody>();
        originalColor = new Color(0f, 0.17647058823f, 0.30588235294f);
        carColor.color = originalColor;

        bumpers = new GameObject[2];
    }

    public void SetWeapon(GameObject obj)
    {
        if (weaponObject != null)
        {
            Destroy(weaponObject);
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

    public void SetCarmor(Item item)
    {
        if (carmor != null)
        {
            carColor.color = originalColor;
            damagable.damageThreshhold -= carmor.thresholdBonus;
            rigidbody.mass -= carmor.mass;
        }
        carmor = item;

        if (carmor != null)
        {
            carColor.color = carmor.carmorColor;
            damagable.damageThreshhold += carmor.thresholdBonus;
            rigidbody.mass += carmor.mass;
        }
    }

    public void SetBumpers(GameObject bumper)
    {
        if (bumpers[0] != null)
        {
            Destroy(bumpers[0]);
            Destroy(bumpers[1]);
            bumpers[0] = null;
            bumpers[1] = null;
        }

        if (bumper != null)
        {
            bumpers[0] = Instantiate(bumper, bumperFrontPos);
            bumpers[1] = Instantiate(bumper, bumperBackPos);
            bumpers[0].GetComponent<Joint>().connectedBody = this.GetComponent<Rigidbody>();
            bumpers[1].GetComponent<Joint>().connectedBody = this.GetComponent<Rigidbody>();
        }
    }
}
