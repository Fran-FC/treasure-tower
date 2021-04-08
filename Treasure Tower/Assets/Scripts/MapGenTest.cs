using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenTest : MonoBehaviour
{
    //defines
    private const int UP    = 0;
    private const int RIGHT = 1;
    private const int DOWN  = 2;
    private const int LEFT  = 3;
    //private readonly string[] output = { "UP", "RIGHT", "DOWN", "LEFT" };
    private readonly Vector3[] directions = { new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector3(-1, 0, 0) };

    //room struct
    struct Room
    {
        public bool[] mainConections { get; set; } //clockwise from the top ({UP, RIGHT, DOWN, LEFT})
        public bool[] secondaryConections { get; set; } //clockwise from the top ({UP, RIGHT, DOWN, LEFT})
        public (int, int) position { get; set; }
        public string tag { get; set; }

        //ONLY USE THIS FOR "START" AND "END" ROOMS - ONLY 1 CONECTION  -   MAYBE FOR PATH WORKS TOO, TOTALLY PLANED
        public Room(int conectionIndex, int h, int w, string t)
        {
            mainConections = new bool[4] { false, false, false, false};
            secondaryConections = new bool[4] { false, false, false, false};
            mainConections[conectionIndex] = true;
            position = (h, w);
            tag = t; 
        }
    }

    //map creation param
    [Range(1, 10)]
    public int height, width;
    [Range(3, 20)]
    public int roomsOnMainPath;
    [Range(3, 20)]
    public int roomsOnSecondaryPath;
    //[Range(0, 100)]
    //public int switchWeight;    //how strong is the desire to change direction
    //[Range(0, 100)]
    //public int distanceWeight;  //how strong is the desire to be far from START
    public string seed;
    public bool randomSeed;

    //internal data
    private Room[,] map;
    private System.Random rng;
    private Stack<(int, int)> path; //main path stack, it's a list cuz I need to peep inside to search for rooms in a determined place on the map

    //temp
    private int startH, startW;

    // Start is called before the first frame update
    void Start() 
    {
        setup();
        findMainPath();
        placeSecondaryRooms();
    }

    private void setup()
    {
        //initiate map
        map = new Room[height, width];

        //initiate the path
        path = new Stack<(int, int)>();

        //setup random number generator
        if (randomSeed) seed = System.DateTime.Now.Ticks.ToString();
        rng = new System.Random(seed.GetHashCode());
    }

    private void findMainPath()
    {
        //select random room to be the starting room
        Room startingRoom = setupStartingRoom();
        //start exploring main path
        bool success = buildPath(startingRoom, Array.IndexOf(startingRoom.mainConections, true));
        if (success) addRoomToMap(startingRoom);
        //if path is successful place it on map, if not then what the fuck did you do, like honestly how do you fuck it up that bad???
    }

    private Room setupStartingRoom()
    {
        //get a random position on the map
        int h = rng.Next(0, height); //Debug.Log("height = " + h);
        int w = rng.Next(0, width); //Debug.Log("width = " + w);
        //check which exits are posible based on map position
        List<int> possibleExits = getConectionsList();
        if      (h <= 0)            possibleExits.Remove(DOWN);
        else if (h >= height - 1)   possibleExits.Remove(UP);
        if      (w <= 0)            possibleExits.Remove(LEFT);
        else if (w >= width - 1)    possibleExits.Remove(RIGHT);
        //select a random exit from the ones available
        int exit = possibleExits[rng.Next(0, possibleExits.Count)];
        Room startingRoom = new Room(exit, h, w, "START");

        return startingRoom;
    }

    private bool buildPath(Room currentRoom, int desiredDirection)
    {
        List<int> possibleExits = getConectionsList();
        //OBJECTIVE -   FIND A NEW DIRECTION TO KEEP MOVING TO  -   RECURSIVE
        int h = currentRoom.position.Item1;
        int w = currentRoom.position.Item2;
        //add current room to the Path stack
        path.Push((h, w));
        //if the path already has the desired length we're done
        if (path.Count >= roomsOnMainPath - 1)
        {
            //DEBUG

            //all rooms are in place, the only room left is "END"
            Room finalRoom;
            switch (desiredDirection)
            {
                case UP:
                    finalRoom = new Room(DOWN, h + 1, w, "END");
                    path.Push((h + 1, w));
                    break;
                case RIGHT:
                    finalRoom = new Room(LEFT, h, w + 1, "END");
                    path.Push((h, w + 1));
                    break;
                case DOWN:
                    finalRoom = new Room(UP, h - 1, w, "END");
                    path.Push((h - 1, w));
                    break;
                case LEFT:
                    finalRoom = new Room(RIGHT, h, w - 1, "END");
                    path.Push((h, w - 1));
                    break;
                default:
                    finalRoom = new Room();
                    Debug.LogError("Something went wrong while building a path; last known room had an invalid target direction");
                    break;
            }
            //place the room on the map and spread the recursive calls we've found a path
            addRoomToMap(finalRoom);
            return true;
        }
        //if not, continue trying to make another room
        //find the new spot and remove from the options the direction you came in
        Room newRoom;
        //List<int> possibleExits = getConectionsList();
        switch (desiredDirection)
        {
            case UP:
                newRoom = new Room(DOWN, h + 1, w, "MAIN PATH");
                possibleExits.Remove(DOWN);
                break;
            case RIGHT:
                newRoom = new Room(LEFT, h, w + 1, "MAIN PATH");
                possibleExits.Remove(LEFT);
                break;
            case DOWN:
                newRoom = new Room(UP, h - 1, w, "MAIN PATH");
                possibleExits.Remove(UP);
                break;
            case LEFT:
                newRoom = new Room(RIGHT, h, w - 1, "MAIN PATH");
                possibleExits.Remove(RIGHT);
                break;
            default:
                newRoom = new Room();   
                Debug.LogError("Something went wrong while building a path; last known room had an invalid target direction");
                break;
        }
        int newH = newRoom.position.Item1;
        int newW = newRoom.position.Item2;

        //remove the options that are outside of bounds
        if      (newH <= 0)           possibleExits.Remove(DOWN);
        else if (newH >= height - 1)  possibleExits.Remove(UP);
        if      (newW <= 0)           possibleExits.Remove(LEFT);
        else if (newW >= width - 1)   possibleExits.Remove(RIGHT);
        //check the stack and remove from the options any direction that may loop to an existing room
        //search the stack for a room with the coordinates of the remaining options, and if found remove the option from the possible directions
        if (path.Contains((newH - 1, newW))) possibleExits.Remove(DOWN);
        if (path.Contains((newH + 1, newW))) possibleExits.Remove(UP);
        if (path.Contains((newH, newW - 1))) possibleExits.Remove(LEFT);
        if (path.Contains((newH, newW + 1))) possibleExits.Remove(RIGHT);
        //shuffle the possible exits to get a random new direction
        if(possibleExits.Count <= 0)
        {
            //there's no more options to explore, go back
            path.Pop();
            return false;
        }
        List<int> shuffledOptions = shuffle(possibleExits);
        for(int i = 0; i<shuffledOptions.Count; i++)
        {
            //try and continue the path, if they return false we continue to the next option, if they return true they've found a path, so we add the room to the map and spread it to the other calls
            bool result = buildPath(newRoom, shuffledOptions[i]);
            if (result)
            {
                //we've found a path!
                newRoom.mainConections[shuffledOptions[i]] = true;
                addRoomToMap(newRoom);
                return true;
            }
        }

        //if we've made it here it means no result was found, so we need to pop ourselves out of the stack and spread it to the other calls
        (int ch, int cw) = path.Pop();
        if (!(ch == h && cw == w)) Debug.LogError("somehow the path stack is not ordered propperly");
        return false; //default, TO BE CHANGED
    }

    private void addRoomToMap(Room r)
    {
        map[r.position.Item1, r.position.Item2] = r;
    }

    private List<int> shuffle(List<int> l)
    {
        for (int i = l.Count; i > 1; i--)
        {
            // Pick random element to swap.
            int j = rng.Next(i); // 0 <= j <= i-1
                                    // Swap.
            int tmp = l[j];
            l[j] = l[i - 1];
            l[i - 1] = tmp;
        }
        return l;
    }

    private List<int> getConectionsList()
    {
        List<int> res = new List<int>();
        res.Add(UP);
        res.Add(RIGHT);
        res.Add(DOWN);
        res.Add(LEFT);
        return res;
    }

    private void placeSecondaryRooms()
    {
        /*
         so here's the plan, 
        for each room to place we search the map for empty neighboring rooms, 
        put them all in a list and select a random one to place the room, 
        and put the conections accordingly of course 
         */
        for(int i = 0; i<roomsOnSecondaryPath; i++)
        {
            List<(int, int)> potentialCoords = new List<(int, int)>();
            for(int h = 0; h < height; h++)
            {
                for(int w = 0; w < width; w++)
                {
                    if (map[h, w].tag != null) continue; //if this room is already allocated we move on
                    else
                    {
                        //this room is empty
                        if (isNeighborToRoom(h, w)) potentialCoords.Add((h, w));
                    }                    
                }
            }
            //we have a list of all empty rooms next to allocated ones, choose a random one to place a room
            if (potentialCoords.Count <= 0) return; //if the list comes back empty we've filled the entire map, no point on keep on ttying
            (int randH, int randW) = potentialCoords[rng.Next(potentialCoords.Count)];
            placeSecRoomAt(randH, randW);
        }
    }

    private void placeSecRoomAt(int h, int w)
    {
        Room room = new Room();
        room.position = (h, w);
        room.tag = "OPTIONAL";
        room.mainConections = new bool[4] { false, false, false, false};
        room.secondaryConections = new bool[4] { false, false, false, false};
        //now check all neigbours
        for(int dir = 0; dir < 4; dir++)
        {
            if(isNeighborAt(h, w, dir))
            {
                //for each neighbor place an optional conection for me and the neighboring room
                room.secondaryConections[dir] = true;
                switch (dir)
                {
                    case UP:
                        map[h + 1, w].secondaryConections[DOWN] = true;
                        break;
                    case RIGHT:
                        map[h, w + 1].secondaryConections[LEFT] = true;
                        break;
                    case DOWN:
                        map[h - 1, w].secondaryConections[UP] = true;
                        break;
                    case LEFT:
                        map[h, w - 1].secondaryConections[RIGHT] = true;
                        break;
                }
            }
        }
        map[h, w] = room;

    }

    private bool isNeighborToRoom(int h, int w)
    {
        if(isNeighborAt(h, w, UP)) return true; //UP
        if(isNeighborAt(h, w, RIGHT)) return true; //RIGHT
        if(isNeighborAt(h, w, DOWN)) return true; //DOWN
        if(isNeighborAt(h, w, LEFT)) return true; //LEFT
        return false;
    }

    private bool isNeighborAt(int h, int w, int dir)
    {
        switch (dir)
        {
            case UP:
                return h + 1 < height && (map[h + 1, w].tag == "MAIN PATH" || map[h + 1, w].tag == "OPTIONAL");
            case RIGHT:
                return w + 1 < width  && (map[h, w + 1].tag == "MAIN PATH" || map[h, w + 1].tag == "OPTIONAL");
            case DOWN:
                return h - 1 >= 0     && (map[h - 1, w].tag == "MAIN PATH" || map[h - 1, w].tag == "OPTIONAL");
            case LEFT:
                return w - 1 >= 0     && (map[h, w - 1].tag == "MAIN PATH" || map[h, w - 1].tag == "OPTIONAL");
            default:
                return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //for each space on map
                    Vector3 pos = new Vector3(-width / 2 + j + .5f, -height / 2 + i + .5f, 0);
                    string tag = map[i, j].tag;
                    if (tag != null)
                    {
                        //a room exists in these coordinates
                        switch (tag)
                        {
                            case "START":
                                Gizmos.color = Color.green;
                                break;
                            case "MAIN PATH":
                                Gizmos.color = Color.blue;
                                break;
                            case "END":
                                Gizmos.color = Color.red;
                                break;
                            case "OPTIONAL":
                                Gizmos.color = Color.yellow;
                                break;
                        }
                        for (int dir = 0; dir < 4; dir++)
                        {
                            //for each cardinal direction, see if the room has a conection
                            if (map[i, j].mainConections[dir] || map[i, j].secondaryConections[dir]) Gizmos.DrawLine(pos, pos + directions[dir]);
                        }
                    }
                    else
                    {
                        Gizmos.color = Color.grey;
                    }
                    
                    Gizmos.DrawCube(pos, Vector3.one/2);
                }
            }
        }
    }
}
