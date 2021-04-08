using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationCells : MonoBehaviour
{
    enum gridSpace
    {
        empty,
        floor,
        wall
    };

    gridSpace[,] grid;
    private int roomHeight;
    private int roomWidth;
    private Vector2 roomSizeWorldUnits = new Vector2(500, 500);
    private float worldUnitsInOneGridCell = 1;

    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }

    private List<walker> walkers;
    private float chanceWalkerChangeDir = 0.5f;
    private float chanceWalkerSpawn = 0.01f;
    private float chanceWalkerDestoy = 0.01f;
    private int maxWalkers = 3;
    private float percentToFill = 0.025f;
    public GameObject wallObj;
    public GameObject floorObj;

    private void Start()
    {
        Setup();
        CreateFloors();
        CreateWalls();
        RemoveSingleWalls();
        SpawnLevel();
    }

    void Setup()
    {
        //fing grip size
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        //create grip
        grid = new gridSpace[roomWidth, roomHeight];
        //set grid's default state
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //make every cell "empty"
                grid[x,y] = gridSpace.empty;
            }
        }

        //set first walker
        //init list
        walkers = new List<walker>();
        //create a walker
        walker newWalker = new walker {dir = RandomDirection()};
        //find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f), Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;
        //add walker to list
        walkers.Add(newWalker);
    }

    private Vector2 RandomDirection()
    {
        //pick random int between 0 and 3
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        //use thet int to chose a direction
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }

    void CreateFloors()
    {
        int iteration = 0;
        do
        {
            //craete floor at position of every walker
            foreach (var walker in walkers)
            {
                grid[(int) walker.pos.x, (int) walker.pos.y] = gridSpace.floor;
            }

            //chance: destroy walker
            int numberChecks = walkers.Count;
            //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if its not the only one, and at a low chance
                if (Random.value < chanceWalkerDestoy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break;
                    //only destroy one per interation
                }
            }

            //chance: walker pick new directory
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }

            //chance: spawn new walker
            numberChecks = walkers.Count;
            //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if # of walkers < max, and at a low chance
                if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                {
                    //create a walker
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }
            //move walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }
            //avold boarder of grid
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                //clamp x,y to leave a 1 space boarder: leave rom for walls
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }

            //check to exit loop
            if ((float) NumberOfFloors() / (float) grid.Length > percentToFill)
            {
                break;
            }
            iteration++;
        } while (iteration < 10000);
    }

    private int NumberOfFloors()
    {
        int count = 0;
        foreach (var gridSpace in grid)
        {
            if (gridSpace == gridSpace.floor)
            {
                count++;
            }
        }
        return count;
    }

    void SpawnLevel()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x,y])
                {
                    case gridSpace.empty:
                        break;
                    case gridSpace.floor:
                        Spawn(x, y, floorObj);
                        break;
                    case gridSpace.wall:
                        Spawn(x, y, wallObj);
                        break;
                }
            }
        }
    }

    void Spawn(float x, float y, GameObject toSpawn)
    {
        //find the position to spawn
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;
        //sapwn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }

    void CreateWalls()
    {
        //loop though every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight-1; y++)
            {
                if (grid[x,y] == gridSpace.floor)
                {
                    if (grid[x,y+1] == gridSpace.empty)
                    {
                        grid[x,y+1] = gridSpace.wall;
                    }
                    if (grid[x,y-1] == gridSpace.empty)
                    {
                        grid[x,y-1] = gridSpace.wall;
                    }
                    if (grid[x+1, y] == gridSpace.empty)
                    {
                        grid[x+1,y] = gridSpace.wall;
                    }
                    if (grid[x-1,y] == gridSpace.empty)
                    {
                        grid[x-1,y] = gridSpace.wall;
                    }
                }
            }
        }
    }

    void RemoveSingleWalls()
    {
        //loop though every grid space
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            { 
                //if theres a wall, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {
                    //assume all space around wall are floors
                    bool allFloors = true;
                    //check each side to see if they are all floors
                    for (int checkX = -1; checkX <= 1; checkX++)
                    {
                        for (int checkY = -1; checkY <= 1; checkY++)
                        {
                            if(x+checkX<0 || x+ checkX>roomWidth-1 || y+checkY<0 || y+checkY > roomWidth-1)
                            {
                                //skip check that are out of range
                                continue;
                            }

                            if (checkX != 0 && checkY != 0 || checkX == 0 && checkY == 0)
                            {
                                //skip corners and center
                                continue;
                            }

                            if (grid[x + checkX, y + checkY] != gridSpace.floor)
                            {
                                allFloors = false;
                            }
                        }
                    }

                    if (allFloors)
                    {
                        grid[x, y] = gridSpace.floor;
                    }
                }
                
            }
        }
    }
}