using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInfo
{
    GridInfo[,] map { get; }
    Vector3Int playerSpawn { get; set; }

    List<EnemyInfo> enemies;
    List<ObjectInfo> objects;

    public MapInfo(int rows, int cols)
    {
        map = new GridInfo[rows, cols];
        enemies = new List<EnemyInfo>();
        objects = new List<ObjectInfo>();
    }

    public void updateMapFakeInfo()
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                if (j == 0 || j == map.GetLength(1) - 1 || i == 0 || i == map.GetLength(0) - 1)
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.WALL, false);
                }
                else if (j == Mathf.FloorToInt(map.GetLength(1) / 2) && i == Mathf.FloorToInt(map.GetLength(0) / 2))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false, false, 0, true, 0);
                }
                else
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false);
                }

                // Setting Neighs
                if (i > 0)
                {
                    map[i, j].SetNeighbor(Direction.O, map[i - 1, j]);
                }

                if (j > 0)
                {
                    map[i, j].SetNeighbor(Direction.S, map[i, j - 1]);
                }

            }
        }

        map[(int)map.GetLength(0) / 2, 1].tileType = (int)TileTypes.FLOOR;
        map[(int)map.GetLength(0) / 2, 1].initPlayerPos = true;

        map[4, map.GetLength(1) - 3].hasObject = true;
        map[4, map.GetLength(1) - 3].objectType = 1;

        map[3, map.GetLength(1) - 2].hasEnemy = true;
        map[3, map.GetLength(1) - 2].enemyType = 0;

        //map[map.GetLength(0) - 3, map.GetLength(1) - 2].hasEnemy = true;
        //map[map.GetLength(0) - 3, map.GetLength(1) - 2].enemyType = 0;
    }


    public void generateMapInfo(Color[,] colorMap)
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                Color color = colorMap[j, i];

                if (color.Equals(Color.black))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.WALL_TOP, false);
                }
                else if (color.Equals(Color.blue))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, true);
                }
                else if (color.Equals(Color.white))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false);
                }
                else if (color.Equals(Color.red))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false, true, Random.Range(0, 2)); // TODO: Fix this cause it sucks
                }
                else if (color.Equals(Color.green)) { 
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false, false, -1, true, 2);
                }
                else
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.WALL, false);
                }


                // Setting Neighs
                if (i > 0)
                {
                    map[i, j].SetNeighbor(Direction.O, map[i - 1, j]);
                }

                if (j > 0)
                {
                    map[i, j].SetNeighbor(Direction.S, map[i, j - 1]);
                }
            }
        }
    }

    public void drawTileMap(Tilemap tm, List<Tile> tileList)
    {

        //        Debug.Log("Map Size x: " + map.GetLength(0) + " y: " + map.GetLength(1));

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GridInfo info = map[i, j];
                if (info.tileType != (int)TileTypes.EMPTY)
                {
                    Tile tile = tileList[info.tileType];

                    tm.SetTile(new Vector3Int(i, j, 0), tile);

                    if (info.initPlayerPos)
                    {
                        playerSpawn = new Vector3Int(i, j, 0);
                    }

                    if (info.hasEnemy)
                    {
                        enemies.Add(new EnemyInfo(new Vector3Int(i, j, 0), info.enemyType));
                    }

                    if (info.hasObject)
                    {
                        objects.Add(new ObjectInfo(new Vector3Int(i, j, 0), info.objectType));
                    }
                }
            }
        }

    }

    public Vector3Int getPlayerSpawn()
    {
        return playerSpawn;
    }

    public List<EnemyInfo> getEnemiesList()
    {
        return enemies;
    }

    public List<ObjectInfo> getObjectsList()
    {
        return objects;
    }

    public bool isTileWalkable(int x, int y)
    {
        return map[x, y].walkable;
    }

    public bool isTileEnemy(int x, int y)
    {
        return map[x, y].hasEnemy;
    }

    public void markNonWalkableTiles(Tilemap tilemap)
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GridInfo info = map[i, j];

                if (!info.walkable)
                {
                    tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(i, j, 0), Color.red);
                }
                else
                {
                    tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(i, j, 0), Color.yellow);
                }
            }
        }
    }

    public void setTileWalkableState(int x, int y, bool isWalkable)
    {
        map[x, y].walkable = isWalkable;
    }

    public void setTileEnemyState(int x, int y, bool isEnemy)
    {
        map[x, y].hasEnemy = isEnemy;
    }

    public GridInfo getGridInfo(int x, int y)
    {
        return map[x, y];
    }
}

