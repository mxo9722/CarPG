using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO DO
/*
-Create the function that places hallways between connected rooms (optional)
-make the script delete itself and see what happens (optional)
-do room Setup (figure out if special rooms need to be separated)
*/

public class GeneratorScript : MonoBehaviour
{
    int testTicks = 0;

    //Dungeon Variables
    public int roomCount;
    public int roomMax;

    public List<GameObject> basicRooms = new List<GameObject>();
    public List<GameObject> specialRooms = new List<GameObject>();
    public List<GameObject> spawnedRooms = new List<GameObject>();
    public List<RoomScript> roomScripts = new List<RoomScript>();
    public List<int> generatableRooms = new List<int>();
    public GameObject hallwayStandin;
    public List<GameObject> placedHallways = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Random.seed = 50;
        roomCount = 0;
        GenerateDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        testTicks++;
        if (testTicks > 100)
        {
            
            testTicks = 0;
        }
    }

    //create the floor layout
    void GenerateDungeon()
    {
        //spawn the first room
        spawnedRooms.Add (basicRooms[0]);
		spawnedRooms [0] = Instantiate (spawnedRooms [0]);
        roomScripts.Add(spawnedRooms[0].GetComponent<RoomScript> ());
        roomScripts[0].dataSetup();
        roomScripts[0].transform.position = new Vector3(0,0,0);
        roomScripts[0].Orientation = Random.Range(0,4);
        roomScripts[0].transform.rotation = Quaternion.Euler(0,90 * roomScripts[0].Orientation,0);
        roomCount++;
        /*
          For now make one side unavailable
          later it will be automatic because 
          the player will not be spawning in 
          this room
        */
        roomScripts[0].connectedSides[Random.Range(0,4)] = true;

        //spawn three rooms off the starting room
        for(int i = 0; i < 3; i++)
        {
            if(NextRoomSolver(0))
            {
                roomCount++;
            }
            else
            {
                break;
            }
        }

        //spawn the rest of the rooms randomly
        while (roomCount < roomMax)
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
            if(SpecialRoomSolver(nextRoom, specialIndex))
            {
                specialIndex++;
            }
            generatableRooms.RemoveAt (randomGrab);
        }

        Debug.Log("Dungeon Generated");
    }

    //method to create new rooms 
    //returns true or false based on if it worked or not
    private bool NextRoomSolver(int roomIndex)
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
        int randomRoom = Random.Range(0,basicRooms.Count);
        //Debug.Log("Room index chosen: " + randomRoom);
        spawnedRooms.Add (basicRooms[randomRoom]);
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
                        Debug.Log("raan out of turns");
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
        roomScripts[newRoomIndex].connectedSides[newChosenSide] = true;
        roomScripts[newRoomIndex].connectedDoors[newChosenSide][newChosenDoor] = true;


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
            placedHallways[hallwayIndex].GetComponent<Transform> ().position = piecePosition + (roomScripts[roomIndex].doorDirections[chosenSide] * ((i * 20) - 10));
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

    private bool SpecialRoomSolver(int roomIndex, int specialIndex)
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

        //Debug.Log("Room index chosen: " + randomRoom);
        //get the special room from the index
        spawnedRooms.Add (specialRooms[specialIndex]);
        int newRoomIndex = spawnedRooms.Count - 1;
		spawnedRooms [newRoomIndex] = Instantiate (spawnedRooms [newRoomIndex]);
        roomScripts.Add(spawnedRooms[newRoomIndex].GetComponent<RoomScript> ());
        roomScripts[newRoomIndex].dataSetup();

        /*
          If the process fails after this point remember to remove 
          this from the lists and destroy it
        */
        
        //choose a side for the new room
        roomScripts[newRoomIndex].Orientation = 0;
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
                        Debug.Log("raan out of turns");
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
        roomScripts[newRoomIndex].connectedSides[newChosenSide] = true;
        roomScripts[newRoomIndex].connectedDoors[newChosenSide][newChosenDoor] = true;


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
            placedHallways[hallwayIndex].GetComponent<Transform> ().position = piecePosition + (roomScripts[roomIndex].doorDirections[chosenSide] * ((i * 20) - 10));
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
