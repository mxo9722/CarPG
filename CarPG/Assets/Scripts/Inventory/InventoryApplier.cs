using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class InventoryApplier : MonoBehaviour
{

    private CarController controller;
    private Damagable _damagable;

    protected Damagable damagable
    {
        get {
            if (_damagable == null)
                _damagable = GetComponent<Damagable>();
            return _damagable;
        }
        set
        {
            _damagable = value;
        }
    }

    private GameObject weaponObject;
    private Rigidbody _rigidbody;

    protected Rigidbody rigidbody
    {
        get
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            return _rigidbody;
        }
        set
        {
            _rigidbody = value;
        }
    }

    private Item carmor;

    private static Material originalMaterial=null;
    public Renderer renderer;

    public Transform bumperFrontPos;
    public Transform bumperBackPos;

    private GameObject[] bumpers;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CarController>();
        damagable = GetComponent<Damagable>();
        rigidbody = GetComponent<Rigidbody>();
        if(originalMaterial==null)
            originalMaterial = renderer.material;

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
            renderer.material = originalMaterial;
            damagable.damageThreshhold -= carmor.thresholdBonus;
            rigidbody.mass -= carmor.mass;
        }
        carmor = item;

        if (carmor != null)
        {
            renderer.material = carmor.carmorMaterial;
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
            //bumpers[0].GetComponent<Joint>().connectedBody = this.GetComponent<Rigidbody>();
            //bumpers[1].GetComponent<Joint>().connectedBody = this.GetComponent<Rigidbody>();
        }
    }
}
