using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    int sceneIndex;
    public Inventory inv;
    public float health;
    public float[] position;
    public LockedDoor doorStatus;

    public PlayerData(Inventory inv, Damagable dmg, LockedDoor door)
    {
        this.inv = inv;
        health = dmg.health;

        doorStatus = door;
        position = new float[3];
        position[0] = dmg.transform.position.x;
        position[1] = dmg.transform.position.y;
        position[2] = dmg.transform.position.z;
    }
}
