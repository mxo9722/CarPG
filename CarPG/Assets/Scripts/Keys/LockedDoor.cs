using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public List<KeyUnlock> keys = new List<KeyUnlock>();
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

    public void Unlock(KeyUnlock cube)
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
