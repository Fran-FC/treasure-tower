using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyInfo
{
    public Vector3Int position { get; set; }
    public int enemyType { get; set; }


    public EnemyInfo(Vector3Int pos, int type)
    {
        position = pos;
        enemyType = type;
    }
}

