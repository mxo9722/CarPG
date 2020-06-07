using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    //room variables
    public List<GameObject> posXDoors = new List<GameObject>();
    public List<GameObject> posZDoors = new List<GameObject>();
    public List<GameObject> negXDoors = new List<GameObject>();
    public List<GameObject> negZDoors = new List<GameObject>();
    public List<GameObject>[] doorPositions;
    public List<bool>[] connectedDoors;
    public bool[] connectedSides;
    public Vector3[] doorDirections;
    // 0 = positive X
    // 1 = positive Z
    // 2 = negative X
    // 3 = negative Z
    //placeholder variables
    public int xWidth;
    public int zWidth;
    public int getXWidth;
    public int getZWidth;
    private int orientation;
    //when orientation is set, set up the direction 
    //vectors for the doors and the relative length and width
    public int Orientation
    {
        get 
        {
            return orientation;
        }
        set
        {
            orientation = value;
            switch (orientation)
            {
                case 0:
                    doorDirections[0] = new Vector3(1,0,0);
                    doorDirections[1] = new Vector3(0,0,1);
                    doorDirections[2] = new Vector3(-1,0,0);
                    doorDirections[3] = new Vector3(0,0,-1);
                    getXWidth = xWidth;
                    getZWidth = zWidth;
                    break;
                case 1:
                    doorDirections[0] = new Vector3(0,0,-1);
                    doorDirections[1] = new Vector3(1,0,0);
                    doorDirections[2] = new Vector3(0,0,1);
                    doorDirections[3] = new Vector3(-1,0,0);
                    getXWidth = zWidth;
                    getZWidth = xWidth;
                    break;
                case 2:
                    doorDirections[0] = new Vector3(-1,0,0);
                    doorDirections[1] = new Vector3(0,0,-1);
                    doorDirections[2] = new Vector3(1,0,0);
                    doorDirections[3] = new Vector3(0,0,1);
                    getXWidth = xWidth;
                    getZWidth = zWidth;
                    break;
                case 3:
                    doorDirections[0] = new Vector3(0,0,1);
                    doorDirections[1] = new Vector3(-1,0,0);
                    doorDirections[2] = new Vector3(0,0,-1);
                    doorDirections[3] = new Vector3(1,0,0);
                    getXWidth = zWidth;
                    getZWidth = xWidth;
                    break;
                default:
                    orientation = value;
                    doorDirections[0] = new Vector3(1,0,0);
                    doorDirections[1] = new Vector3(0,0,1);
                    doorDirections[2] = new Vector3(-1,0,0);
                    doorDirections[3] = new Vector3(0,0,-1);
                    getXWidth = xWidth;
                    getZWidth = zWidth;
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //called by the dungeon generator to set up the room quicker
    public void dataSetup()
    {
        //add the lists of door positions to overall list
        //doors are added within the inspector
        doorPositions = new List<GameObject>[4];
        doorPositions[0] = posXDoors;
        doorPositions[1] = posZDoors;
        doorPositions[2] = negXDoors;
        doorPositions[3] = negZDoors;

        //setup door data
        connectedSides = new bool[4];
        connectedDoors = new List<bool>[4];
        doorDirections = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            //false == unconnected
            connectedSides[i] = false;
            //check if a side has no doors to connect to
            if(doorPositions[i].Count == 0)
            {
                connectedSides[i] = true;
                continue;
            }

            connectedDoors[i] = new List<bool>();
            //add doors to the boolean list based on the doorPositions
            for (int j = 0; j < doorPositions[i].Count; j++)
            {
                connectedDoors[i].Add(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for(int i = 0; i < 4; i++)
        // {
        //     Debug.Log(connectedSides[i]);
        // }
    }

    //For Later
    //spawn enemies, walls, objects, etc., based on room door data and children
    public void RoomSetup()
    {

    }

    //returns a list of indices for the sides available in a room
    public List<int> checkConnectable()
    {
        List<int> checkAvailableSides = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            if(!connectedSides[i])
            {
                checkAvailableSides.Add(i);
            }
        }
        return checkAvailableSides;
    }


}
