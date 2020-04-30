using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectorTether : MonoBehaviour
{

    private LineRenderer lineRenderer;

    private GameObject boss;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.Find("Boss_Fight(Clone)");
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] points = {transform.position,boss.transform.position};
        lineRenderer.SetPositions(points);
    }
}
