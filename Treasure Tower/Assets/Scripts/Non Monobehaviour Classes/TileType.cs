using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType
{
    public _TileType tileType;
}

public enum _TileType
{
    WALL,
    FLOOR,
    EMPTY
}
