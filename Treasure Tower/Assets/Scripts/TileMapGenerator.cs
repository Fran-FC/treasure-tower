using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



enum TileTypes { 
    WALL,
    FLOOR,
    OBJECT,
    EMPTY,
}

public class TileMapGenerator : MonoBehaviour
{
    [SerializeField]
    private List<Tile> tileList;
    [SerializeField]
    private int rows, cols;

    [SerializeField]
    private GameObject player;

    private MapInfo mapInfo;



    // Start is called before the first frame update
    void Start()
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        Tilemap tilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
        tilemap.gameObject.AddComponent<TilemapRenderer>();

        tilemap.transform.SetParent(grid.transform);


        tilemap.transform.position = new Vector2(-rows / 2, -cols / 2);


        mapInfo = new MapInfo(rows, cols);

        mapInfo.updateMapFakeInfo();
        mapInfo.drawTileMap(tilemap, tileList);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        worldPos.z = 0;

        Vector3Int tilemapPos = tilemap.WorldToCell(worldPos);
        Vector3 center = tilemap.GetCellCenterWorld(tilemapPos);


        Instantiate(player, center, Quaternion.identity);
    }



    public bool isTileWalkable(int x, int y) {
        return mapInfo.isTileWalkable(x, y);
    }
}


public class MapInfo 
{
    GridInfo[,] map { get; }

    public MapInfo(int rows, int cols) {
        map = new GridInfo[rows, cols];
    }

    public void updateMapFakeInfo() {

        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++) {

                if (j == 0 || j == map.GetLength(1) - 1 || i == 0 || i == map.GetLength(0) - 1)
                {
                    map[i, j] = new GridInfo((int)TileTypes.WALL, false);
                }
                else { 
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false);
                }

            }
        }

    }

    public void drawTileMap(Tilemap tm, List<Tile> tileList) {

        Debug.Log("Map Size x: " + map.GetLength(0) + " y: " + map.GetLength(1));

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GridInfo info = map[i, j];
                Tile tile = tileList[info.tileType];

                tm.SetTile(new Vector3Int(i, j, 0), tile);
            }
        }

    }


    public bool isTileWalkable(int x, int y) {
        return map[x, y].walkable;
    } 
}


public class GridInfo 
{
    public int tileType {  get; set; }
    public bool walkable { get; set; }
    public bool playerPos { get; set; }


    public GridInfo(int type, bool isPlayerPos) {
        tileType = type;
        playerPos = isPlayerPos;
        walkable = tileType == (int) TileTypes.FLOOR;
    }
}
