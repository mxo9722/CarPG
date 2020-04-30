using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public List<GameObject> keys = new List<GameObject>();
    public int locks;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Damagable>().enabled = false;
        locks = keys.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Unlock(GameObject cube)
    {
        if (keys.Contains(cube))
        {
            locks--;
            if(locks <= 0)
            {
                GetComponent<Damagable>().enabled = true;
                SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
        
    }
}
