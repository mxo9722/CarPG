using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FitObstacleToCollider : MonoBehaviour
{
    void Start()
    {
        var obstacle = GetComponent<NavMeshObstacle>();

        var meshs = GetComponents<Renderer>();
        foreach (Renderer r in meshs)
        {
            var bounds = r.bounds;
            //obstacle.center = bounds.center;
            obstacle.size = bounds.size;
            float z = obstacle.size.x;

            obstacle.size = new Vector3(obstacle.size.z, obstacle.size.y, obstacle.size.x)/3*2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
