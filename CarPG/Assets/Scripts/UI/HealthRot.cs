using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRot : MonoBehaviour
{
    GameObject car;
    Damagable cDamagable;
    RectTransform rTransform;

    float startRot;

    public float speed = 0;

    private void Start()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].transform.parent?.tag != "Player")
            {
                car = players[i];
                break;
            }
        }

        cDamagable = car.GetComponent<Damagable>();
        rTransform = GetComponent<RectTransform>();

        startRot = GetComponent<RectTransform>().rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        rTransform.rotation = Quaternion.Euler(0, 0, startRot - (cDamagable.health * 1.868f * 1.4f));
    }
}
