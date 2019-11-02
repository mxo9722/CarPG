using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity;

    public void CreateProjectile(Vector3 targetPos, float speed)
    {
        velocity = Vector3.Normalize(targetPos - transform.position);
        velocity *= speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }
}
