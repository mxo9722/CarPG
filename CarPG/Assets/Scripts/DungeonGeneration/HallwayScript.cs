using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayScript : MonoBehaviour
{
    //hallway variables
    public List<GameObject> xWalls = new List<GameObject>();
    public List<GameObject> zWalls = new List<GameObject>();
    public List<GameObject> xTorches = new List<GameObject>();
    public List<GameObject> zTorches = new List<GameObject>();
    public Vector3 hallwayDirection;
    public bool litHallway = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HallwaySetup()
    {
        if(hallwayDirection.x != 0)
        {
            for(int i = 1; i > -1; i--)
            {
                Destroy(xWalls[i]);
                xWalls.RemoveAt(i);
                Destroy(xTorches[i]);
                xTorches.RemoveAt(i);
                if(!litHallway)
                {
                    Destroy(zTorches[i]);
                    zTorches.RemoveAt(i);
                }
            }
        }
        else
        {
            for(int i = 1; i > -1; i--)
            {
                Destroy(zWalls[i]);
                zWalls.RemoveAt(i);
                Destroy(zTorches[i]);
                zTorches.RemoveAt(i);
                if(!litHallway)
                {
                    Destroy(xTorches[i]);
                    xTorches.RemoveAt(i);
                }
            }
        }
    }
}
