using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo
{
    public Vector3Int position { get; set; }
    public int objectType { get; set; }


    public ObjectInfo(Vector3Int pos, int type)
    {
        position = pos;
        objectType = type;
    }
}

