using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Damagable damagable;
    void Start()
    {
       damagable = GetComponent<Damagable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (damagable.health > 0)
            GUI.Box(new Rect(10, 10, damagable.health * 3, 20), damagable.health + "/" + damagable.maxHealth);
        else
            GUI.TextArea(new Rect(10, 10, 40, 20), "DEAD");
    }
}
