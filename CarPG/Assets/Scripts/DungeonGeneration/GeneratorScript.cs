using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*TO DO*/
/*
-Create the function that places hallways between connected rooms (optional)
-make the script delete itself and see what happens (optional)
-add the item-based room spawning
*/

public class GeneratorScript : MonoBehaviour
{
    int testTicks = 0;

    //Dungeon Variables
    public int roomCount;
    public int roomMax;

    public List<GameObject> spawnableRooms = new List<GameObject>();
    public List<int> roomEnums = new List<int>(); //will be used to keep track of how many times specific rooms have been spawned
    public List<GameObject> wingRooms = new List<GameObject>();
    public List<GameObject> specialRooms = new List<GameObject>();
    public List<GameObject> spawnedRooms = new List<GameObject>();
    public List<RoomScript> roomScripts = new List<RoomScript>();
    public List<int> generatableRooms = new List<int>();
    public GameObject hallwayStandin;
    public List<GameObject> placedHallways = new List<GameObject>();
    public List<GameObject> doorTypes = new List<GameObject>();

    public GameObject destructionParent;

    public List<Item> chestItems = new List<Item>();

    //navmesh variables
    public NavMeshSurface surface;
    bool reMeshed = false;

    // Start is called before the first frame update
    void Start()
    {
        //Random.seed = 50;
        roomCount = 0;
        GenerateDungeon();
        //surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        //set up the nav mesh (for some reason it doesn't work right when run from Start())
        if(!reMeshed)
        {
            surface.BuildNavMesh();
            reMeshed = true;
            Debug.Log("reMeshed");
            //CombineMeshes();
        }
        testTicks++;
        if (testTicks > 250)
        {
            
            
            testTicks = 0;
        }
    }

    //create the floor layout
    void GenerateDungeon()
    {   
        /*TO DO*/
        //later on check here for items that the player has
        //and add rooms to spawnable rooms based on the items

        //choose the new item for the floor
        /*right now the only item is wings*/
        int itemIndex = 0;
        for(int i = 0; i < spawnableRooms.Count; i++)
        {
            roomEnums.Add(0);
        }

        //spawn the first room
        spawnedRooms.Add (spawnableRooms[0]);
		spawnedRooms [0] = Instantiate (spawnedRooms [0]);
        roomScripts.Add(spawnedRooms[0].GetComponent<RoomScript> ());
        roomScripts[0].dataSetup();
        roomScripts[0].transform.position = new Vector3(0,0,0);
        roomScripts[0].Orientation = Random.Range(0,4);
        roomScripts[0].transform.rotation = Quaternion.Euler(0,90 * roomScripts[0].Orientation,0);
        roomScripts[0].transform.SetParent(transform);
        roomCount++;

        //spawn two branching paths from the original room, leave the other sides open for more branches
        for (int i = 0; i < 2; i++)
        {
            NextRoomSolver(0);
        }

        //spawn the rest of the rooms randomly
        while (roomCount < roomMax)
        {
            //if the list of rooms to branch from is empty, refind branchable rooms
            if(generatableRooms.Count == 0)
            {
                for(int i = 0; i < roomScripts.Count; i++)
                {
                    if(roomScripts[i].checkConnectable().Count > 0)
                    {
                        generatableRooms.Add(i);
                    }
                }
            }

            /*TO DO*/
            //finish this by adding multiple types of special rooms and spawnable items

            //add the item rooms to the equation at the halfway mark
            if(roomCount == (roomMax / 2))
            {
                //add the room that spawns the special item

                //add the rooms that utilize the special item
                for(int i = 0; i < wingRooms.Count; i++)
                {
                    spawnableRooms.Add(wingRooms[i]);
                    roomEnums.Add(0);
                }
            }

            //choose a random available room
            int randomGrab = Random.Range(0, generatableRooms.Count);
            int nextRoom = generatableRooms[randomGrab];
            //choose an amount of rooms to extend off that room
            int extentionCount;
            if(roomScripts[nextRoom].checkConnectable().Count == 1)
            {
                extentionCount = 1;
            }
            else
            {
                extentionCount = Random.Range(1, roomScripts[nextRoom].checkConnectable().Count + 1);
            }
            
            for(int i = 0; i < extentionCount; i++)
            {
                //if the function is successful add to room count
                if(NextRoomSolver(nextRoom))
                {
                    roomCount++;
                }
                else//if not, no other iterations will be successful
                {
                    break;
                }
            }
            //remove room from the list
            generatableRooms.RemoveAt (randomGrab);
        }

        //spawn special rooms
        int specialIndex = 0;
        while(specialIndex < specialRooms.Count)
        {
            //choose a random available room
            if(generatableRooms.Count == 0)
            {
                for(int i = 0; i < roomScripts.Count; i++)
                {
                    if(roomScripts[i].checkConnectable().Count > 0)
                    {
                        generatableRooms.Add(i);
                    }
                }
            }
            int randomGrab = Random.Range(0, generatableRooms.Count);
            int nextRoom = generatableRooms[randomGrab];

            //if the function is successful add to room count
            if(NextRoomSolver(nextRoom, specialIndex))
            {
                specialIndex++;
            }
            generatableRooms.RemoveAt (randomGrab);
        }

        //run the room setup for all the spawned rooms
        for(int i = 0; i < spawnedRooms.Count; i++)
        {
            //if the room is a secret room, give it a chest item
            if(roomScripts[i].isSecret)
            {
                int giveItem = Random.Range(0,chestItems.Count);
                roomScripts[i].itemChest.GetComponent<ItemHolder>().SetContent(chestItems[giveItem]);
                //remove the chest item from the list so no more of that item can appear
                chestItems.RemoveAt(giveItem);
            }
            roomScripts[i].RoomSetup(destructionParent);
        }

        Debug.Log("Dungeon Generated");
    }

    //method to create new rooms 
    //returns true or false based on if it worked or not
    private bool NextRoomSolver(int roomIndex, int specialIndex = -1)
    {
        int chosenSide = 0;
        List<int> availableSides = roomScripts[roomIndex].checkConnectable();
        //find unused sides for the room
        if(availableSides.Count != 0)
        {
            chosenSide = availableSides[Random.Range(0, availableSides.Count)];
        }
        else
        {
            /*This should never happen*/
            Debug.Log("This should never happen: chosen room has no available sides to branch from");
            return false;
        }
        //choose a door
        int chosenDoor;
        if(roomScripts[roomIndex].connectedDoors[chosenSide].Count == 1)
        {
            chosenDoor = 0;
        }
        else
        {
            chosenDoor = Random.Range(0,roomScripts[roomIndex].connectedDoors[chosenSide].Count);
        }

        //choose a new room
        int newGrab = -1;
        if(specialIndex < 0 || specialIndex >= specialRooms.Count)
        {
            newGrab = Random.Range(0,spawnableRooms.Count);
            spawnedRooms.Add (spawnableRooms[newGrab]);
        }
        else//use the special index if given;
        {
            spawnedRooms.Add (specialRooms[specialIndex]);
        }
        int newRoomIndex = spawnedRooms.Count - 1;
		spawnedRooms [newRoomIndex] = Instantiate (spawnedRooms [newRoomIndex]);
        roomScripts.Add(spawnedRooms[newRoomIndex].GetComponent<RoomScript> ());
        roomScripts[newRoomIndex].dataSetup();

        /*
          If the process fails after this point remember to remove 
          this from the lists and destroy it
        */
        
        //choose a side for the new room
        roomScripts[newRoomIndex].Orientation = Random.Range(0,4);
        roomScripts[newRoomIndex].transform.rotation = Quaternion.Euler(0,90 * roomScripts[newRoomIndex].Orientation,0);
        int newChosenSide = 0;
        newChosenSide = TurnTheRoom(roomIndex, newRoomIndex, chosenSide, true);
        if(newChosenSide == -1)
        {
            //if there are no more available the room is lost
            Debug.Log("This should never happen: the chosen room has no available doors");
            Destroy(spawnedRooms[newRoomIndex]);
            spawnedRooms.RemoveAt(newRoomIndex);
            roomScripts.RemoveAt(newRoomIndex);
            return false;
        }

        //choose the door of the new room
        int newChosenDoor;
        if(roomScripts[newRoomIndex].connectedDoors[newChosenSide].Count == 1)
        {
            newChosenDoor = 0;
        }
        else
        {
            newChosenDoor = Random.Range(0,roomScripts[newRoomIndex].connectedDoors[newChosenSide].Count);
        }

        bool placeGood = false;
        int doorChecks = 1;
        int turnChecks = 1;
        
        //postion the new room
        while(!placeGood)
        {
            Vector3 doorDistance = roomScripts[roomIndex].doorPositions[chosenSide][chosenDoor].GetComponent<Transform> ().position - roomScripts[roomIndex].transform.position;
            Vector3 newDoorDistance = roomScripts[newRoomIndex].doorPositions[newChosenSide][newChosenDoor].GetComponent<Transform> ().position - roomScripts[newRoomIndex].transform.position;
            Vector3 newRoomPosition = roomScripts[roomIndex].transform.position;

            if(roomScripts[roomIndex].doorDirections[chosenSide].x != 0)//movement in the x direction
            {
                newRoomPosition.z += doorDistance.z;
                newRoomPosition += roomScripts[roomIndex].doorDirections[chosenSide] * (roomScripts[roomIndex].getXWidth + roomScripts[newRoomIndex].getXWidth + 60);
                newRoomPosition.z -= newDoorDistance.z;
            }
            else//movemnet in the z direction
            {
                newRoomPosition.x += doorDistance.x;
                newRoomPosition += roomScripts[roomIndex].doorDirections[chosenSide] * (roomScripts[roomIndex].getZWidth + roomScripts[newRoomIndex].getZWidth + 60);
                newRoomPosition.x -= newDoorDistance.x;
            }
            roomScripts[newRoomIndex].transform.position = newRoomPosition;

            //check if it's colliding with anything
            bool isColliding = false;
            for(int i = 0; i < spawnedRooms.Count - 1; i++)
            {
                Vector3 checkDistance = roomScripts[newRoomIndex].transform.position - roomScripts[i].transform.position;
                checkDistance.x = Mathf.Abs(checkDistance.x);
                checkDistance.z = Mathf.Abs(checkDistance.z);
                //if rooms are colliding try a different door
                if(checkDistance.x < (roomScripts[newRoomIndex].getXWidth + roomScripts[i].getXWidth + 20) && checkDistance.z < (roomScripts[newRoomIndex].getZWidth + roomScripts[i].getZWidth + 20))
                {
                    isColliding = true;
                    break;
                }
            }

            //if the room is colliding try some different things
            if(isColliding)
            {
                //try a new door on the same side
                if(doorChecks < roomScripts[newRoomIndex].connectedDoors[newChosenSide].Count)
                {
                    doorChecks++;
                    newChosenDoor++;
                    if(newChosenDoor >= roomScripts[newRoomIndex].connectedDoors[newChosenSide].Count)
                    {
                        newChosenDoor = 0;
                    }
                    continue;
                }
                else //if(turnChecks < roomScripts[newRoomIndex].checkConnectable().Count)//try turning the room
                {
                    if(turnChecks >= roomScripts[newRoomIndex].checkConnectable().Count)
                    {
                        //if there are no more available the room is lost
                        Destroy(spawnedRooms[newRoomIndex]);
                        spawnedRooms.RemoveAt(newRoomIndex);
                        roomScripts.RemoveAt(newRoomIndex);
                        Debug.Log("ran out of turns");
                        return false;
                    }
                    newChosenSide = TurnTheRoom(roomIndex, newRoomIndex, chosenSide, false);
                    if(newChosenSide == -1)
                    {
                        //if there are no more available the room is lost
                        Destroy(spawnedRooms[newRoomIndex]);
                        spawnedRooms.RemoveAt(newRoomIndex);
                        roomScripts.RemoveAt(newRoomIndex);
                        Debug.Log("no more sides available");
                        return false;
                    }
                    doorChecks = 0;
                    turnChecks++;
                    if(roomScripts[newRoomIndex].connectedDoors[newChosenSide].Count == 1)
                    {
                        newChosenDoor = 0;
                    }
                    else
                    {
                        newChosenDoor = Random.Range(0,roomScripts[newRoomIndex].connectedDoors[newChosenSide].Count);
                    }
                    continue;
                }
            }
            
            placeGood = true;
        }

        //mark the doors and sides as used
        roomScripts[roomIndex].connectedSides[chosenSide] = true;
        roomScripts[roomIndex].connectedDoors[chosenSide][chosenDoor] = true;
        roomScripts[roomIndex].actualDoors[chosenSide] = doorTypes[specialIndex + 1];
        roomScripts[newRoomIndex].connectedSides[newChosenSide] = true;
        roomScripts[newRoomIndex].connectedDoors[newChosenSide][newChosenDoor] = true;
        roomScripts[newRoomIndex].actualDoors[newChosenSide] = doorTypes[specialIndex + 1];
        roomScripts[newRoomIndex].name = "room_" + newRoomIndex;

        roomScripts[newRoomIndex].transform.SetParent(transform);

        if(newGrab != -1)
        {
            roomEnums[newGrab]++;
        }

        /*TO DO*/
        //check roomEnums to see if the most recently used room has gone over the limit of uses
        /*only do this once there are more rooms (enough to actually fill the dungeon)*/ 

        //connect the rooms via hallways
        //HallwaySolver(roomScripts[roomIndex].doorPositions[chosenSide][chosenDoor].GetComponent<Transform> ().position, roomScripts[roomIndex].doorDirections[chosenSide], roomScripts[newRoomIndex].doorPositions[newChosenSide][newChosenDoor].GetComponent<Transform> ().position, true);
        Vector3 piecePosition;
        int hallwayIndex;
        piecePosition = roomScripts[roomIndex].doorPositions[chosenSide][chosenDoor].GetComponent<Transform>().position;

        for(int i = 1; i < 4; i++)
        {
            placedHallways.Add (hallwayStandin);
            hallwayIndex = placedHallways.Count - 1;
		    placedHallways [hallwayIndex] = Instantiate (placedHallways [hallwayIndex]);
            HallwayScript hallwaysScript = placedHallways[hallwayIndex].GetComponent<HallwayScript> ();
            hallwaysScript.transform.position = piecePosition + (roomScripts[roomIndex].doorDirections[chosenSide] * ((i * 20) - 10));
            hallwaysScript.hallwayDirection = roomScripts[roomIndex].doorDirections[chosenSide];
            if(i == 2)
            {
                hallwaysScript.litHallway = true;
            }
            hallwaysScript.HallwaySetup();
            placedHallways[hallwayIndex].transform.SetParent(transform);
        }

        Debug.Log("Room " + roomIndex + " connected to room " + newRoomIndex + " between " + roomScripts[roomIndex].doorPositions[chosenSide][chosenDoor].name + " and " + roomScripts[newRoomIndex].doorPositions[newChosenSide][newChosenDoor].name);

        //if the room has available sides, add it to the list of rooms to be branched from
        if(roomScripts[newRoomIndex].checkConnectable().Count != 0)
        {
            generatableRooms.Add(newRoomIndex);
        }
        //room creation was successful
        return true;
    }

    //checks the sides of a room to see if they're compatable
    //returns the index of the chosen side
    private int TurnTheRoom(int roomIndex, int newRoomIndex, int chosenSide, bool testFirst)
    {
        int sideIndex = 0;
        int newRoomTurns = 0;
        List<int> newRoomSides = roomScripts[newRoomIndex].checkConnectable();
        
        while(newRoomTurns < 3)
        {
            if(!testFirst)
            {
                //turn the room
                if(roomScripts[newRoomIndex].Orientation == 0)
                {
                    roomScripts[newRoomIndex].Orientation = 3;
                }   
                else
                {
                    roomScripts[newRoomIndex].Orientation = roomScripts[newRoomIndex].Orientation - 1;
                }
                roomScripts[newRoomIndex].transform.rotation = Quaternion.Euler(0,90 * roomScripts[newRoomIndex].Orientation,0);
                newRoomTurns++; 
            }
            else
            {
                testFirst = false;
            }
            sideIndex = 0;
            while(sideIndex < newRoomSides.Count)
            {
                //find the side that's facing the original side
                if(roomScripts[newRoomIndex].doorDirections[newRoomSides[sideIndex]] == (roomScripts[roomIndex].doorDirections[chosenSide] * -1.0f))
                {
                    //return the side if found
                    return newRoomSides[sideIndex];
                }
                sideIndex++;
            }
            
        }
        return -1;
    }

    //combine the meshes of similar textured objects to lessen the stress of the game
    public void CombineMeshes()
    {
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Mesh finalMesh =  new Mesh();

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        for(int i = 0; i < filters.Length; i++)
        {
            if(filters[i].transform == transform)
                continue;

            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = filters[i].sharedMesh;
            combiners[i].transform = filters[i].transform.localToWorldMatrix;
        }

        finalMesh.CombineMeshes(combiners);

        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        transform.rotation = oldRot;
        transform.position = oldPos;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //does the navmesh stuffs
    // public void BuildNavMesh () 
    // {
    //     int agentTypeCount = NavMesh.GetSettingsCount();
    //     Debug.Log("agentTypeCount = " + agentTypeCount);
    //     return;
        
    //     if (agentTypeCount < 1) 
    //     { 
    //         return; 
    //     } 

    //     for (int i = 0; i < navMeshElements.Count; ++i) 
    //     { 
    //         navMeshElements[i].transform.SetParent(navMeshRoot.transform, true); 
    //     }

    //     for (int i = 0; i < agentTypeCount; ++i) 
    //     {
    //         NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);
    //         NavMeshSurface navMeshSurface = environment.AddComponent<NavMeshSurface>();
    //         navMeshSurface.agentTypeID = settings.agentTypeID;
 
    //         NavMeshBuildSettings actualSettings = navMeshSurface.GetBuildSettings();
    //         navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders; // or you can use RenderMeshes
 
    //         navMeshSurface.BuildNavMesh();
    //     }
 
    // }

    //place the hallways
    // public void HallwaySolver(Vector3 startDoor, Vector3 hallwayDirection, Vector3 endDoor, bool justMade)
    // {
    //     //this part is for the room that were just put down
    //     if(justMade)
    //     {
    //         Vector3 piecePosition = startDoor + (hallwayDirection * 10);
    //         Vector3 pieceGoal = endDoor - (hallwayDirection * 10);
    //         while(piecePosition != pieceGoal)
    //         {
    //             //place the hallway piece
    //             placedHallways.Add (hallwayStandin);
    //             int hallwayIndex = placedHallways.Count - 1;
	// 	        placedHallways [hallwayIndex] = Instantiate (placedHallways [hallwayIndex]);
    //             placedHallways[hallwayIndex].GetComponent<Transform> ().position = piecePosition;
    //             piecePosition += (hallwayDirection * 20);
    //         }
    //         //place the last piece
    //         placedHallways.Add (hallwayStandin);
    //         int lastIndex = placedHallways.Count - 1;
	// 	    placedHallways [lastIndex] = Instantiate (placedHallways [lastIndex]);
    //         placedHallways[lastIndex].GetComponent<Transform> ().position = piecePosition;
    //     }
    //     else//this part is to be implemented later for adding hallways between 
    //     {

    //     }
    // }
}
