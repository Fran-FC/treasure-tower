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

    private Tilemap tilemap;

    [SerializeField]
    private GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        tilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
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
        Instantiate(enemy, new Vector3(center.x - 3, center.y, 0),Quaternion.identity);
    }



    public bool isTileWalkable(float v_x, float v_y) {

        int x = (int) Mathf.Floor(v_x);
        int y = (int) Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x,y,0));
        return mapInfo.isTileWalkable(cellPos.x, cellPos.y);
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
                if (info.tileType != (int)TileTypes.EMPTY) {
                    Tile tile = tileList[info.tileType];

                    tm.SetTile(new Vector3Int(i, j, 0), tile);
                }
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
    public bool initPlayerPos { get; set; }


    public GridInfo(int type, bool isPlayerPos) {
        tileType = type;
        initPlayerPos = isPlayerPos;
        walkable = tileType == (int) TileTypes.FLOOR;
    }
}
