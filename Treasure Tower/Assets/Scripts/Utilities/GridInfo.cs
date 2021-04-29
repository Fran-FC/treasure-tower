using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridInfo
{
    public int tileType { get; set; }
    public bool walkable { get; set; }
    public bool initPlayerPos { get; set; }

    public bool hasEnemy { get; set; }
    public int enemyType { get; set; }

    public bool hasObject { get; set; }
    public int objectType { get; set; }

    public int coord_x { get; set; }
    public int coord_y { get; set; }

    private GridInfo[] neighbors = new GridInfo[4];


    public GridInfo(int x, int y, int type, bool isPlayerPos, bool isEnemy = false, int eType = -1, bool isObject = false, int oType = -1)
    {

        coord_x = x;
        coord_y = y;

        tileType = type;
        initPlayerPos = isPlayerPos;
        walkable = tileType == (int)TileTypes.FLOOR;
        hasEnemy = isEnemy;
        enemyType = eType;

        hasObject = isObject;
        objectType = oType;
    }

    public void SetNeighbor(Direction direction, GridInfo cell)
    {
        if (this.walkable && cell.walkable)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }
    }

    public GridInfo GetNeighbor(Direction direction)
    {
        return neighbors[(int)direction];
    }

    public override string ToString()
    {
        return "GridInfo: Coords( " + coord_x + ", " + coord_y + " )";
    }

    public override bool Equals(object obj)
    {
        if (obj is GridInfo)
        {
            GridInfo o = (GridInfo)obj;
            return this.coord_x == o.coord_x && this.coord_y == o.coord_y;
        }

        return false;
    }
}


