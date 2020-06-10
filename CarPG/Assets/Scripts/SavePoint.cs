using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavePoint : MonoBehaviour
{
    public Inventory inv;
    public LockedDoor door;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<BoxCollider>().enabled = false;
            //SaveSystem.SavePlayer(inv, other.GetComponent<Damagable>(), door, this);
        }
    }
}
