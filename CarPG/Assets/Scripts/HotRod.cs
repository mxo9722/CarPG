using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HotRod : Weapon
{

    public GameObject projectile;

    public GameObject source;

    public GameObject fire;

    public float projectileSpeed;

    float timer;
    public float rechargeTime;

    // Start is called before the first frame update
    void Start()
    {
        timer = -rechargeTime;
    }

    // Update is called once per frame
    void Update()
    {
        bool pressed = CrossPlatformInputManager.GetButton("Fire3");

        if (pressed&&Time.time-rechargeTime>timer)
        {
            Projectile newFireball = Instantiate(projectile, source.transform.position, Quaternion.identity).GetComponent<Projectile>();
            newFireball.CreateProjectile((source.transform.position+Camera.main.transform.forward), projectileSpeed, "PlayerParts");
            timer = Time.time;
        }

        var emit = fire.GetComponent<ParticleSystem>().emission;
        emit.enabled = Time.time - rechargeTime > timer;
    }
}
