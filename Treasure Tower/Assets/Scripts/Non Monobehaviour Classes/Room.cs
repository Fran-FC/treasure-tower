using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
    //basic room layout
    public TileType[][] tiles;

    public string id;
}
