using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



enum TileTypes { 
    WALL,
    FLOOR,
    OBJECT,
    SWORD,
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

<<<<<<< HEAD
    [SerializeField]
    private GameObject enemy;
=======
    private Vector3Int playerSpawn;

    [SerializeField]
    private List<GameObject> enemyTypesList;

>>>>>>> ea4b168b5bf8c8bd904a3f52e23f1e824e801e78

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

        playerSpawn = mapInfo.getPlayerSpawn();
        Vector3 center = tilemap.GetCellCenterWorld(playerSpawn);

        Instantiate(player, center, Quaternion.identity);
<<<<<<< HEAD
        Instantiate(enemy, new Vector3(center.x - 3, center.y, 0),Quaternion.identity);
=======


        List<EnemyInfo> enemies = mapInfo.getEnemiesList();

        for (int i = 0; i < enemies.Count; i++) {

            EnemyInfo enemy = enemies[i];

            Vector3 enemyPos = tilemap.GetCellCenterWorld(enemy.position);
            GameObject enemyPrefab = enemyTypesList[enemy.enemyType];

            Instantiate(enemyPrefab, enemyPos, Quaternion.identity);

        }
>>>>>>> ea4b168b5bf8c8bd904a3f52e23f1e824e801e78
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
    Vector3Int playerSpawn { get; set; }

    List<EnemyInfo> enemies;

    public MapInfo(int rows, int cols) {
        map = new GridInfo[rows, cols];
        enemies = new List<EnemyInfo>();
    }

    public void updateMapFakeInfo() {

        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++) {

                if (j == 0 || j == map.GetLength(1) - 1 || i == 0 || i == map.GetLength(0) - 1)
                {
                    map[i, j] = new GridInfo((int)TileTypes.WALL, false);
                }
                else if (j == Mathf.FloorToInt(map.GetLength(1) / 2) && i == Mathf.FloorToInt(map.GetLength(0) / 2))  {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false);
                }
                else {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false);
                }

            }
        }

        map[(int)map.GetLength(0) / 2, 1] = new GridInfo((int)TileTypes.FLOOR, true);

        map[3, map.GetLength(1) - 2] = new GridInfo((int)TileTypes.FLOOR, false, true, 0);
        map[map.GetLength(0) - 3, map.GetLength(1) - 2] = new GridInfo((int)TileTypes.FLOOR, false, true, 0);
    }

    public void drawTileMap(Tilemap tm, List<Tile> tileList) {

        Debug.Log("Map Size x: " + map.GetLength(0) + " y: " + map.GetLength(1));

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GridInfo info = map[i, j];
                if (info.tileType != (int)TileTypes.EMPTY) {
                    Debug.Log(info.tileType);
                    Tile tile = tileList[info.tileType];

                    tm.SetTile(new Vector3Int(i, j, 0), tile);

                    if (info.initPlayerPos) {
                        playerSpawn = new Vector3Int(i, j, 0);
                    }

                    if (info.hasEnemy) {
                        enemies.Add(new EnemyInfo(new Vector3Int(i, j, 0), info.enemyType));
                    }
                }
            }
        }

    }

    public Vector3Int getPlayerSpawn() {
        return playerSpawn;
    }

    public List<EnemyInfo> getEnemiesList() {
        return enemies;
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

    public bool hasEnemy { get; set; }
    public int enemyType { get; set; }



    public GridInfo(int type, bool isPlayerPos, bool isEnemy = false, int eType = -1) {
        tileType = type;
        initPlayerPos = isPlayerPos;
        walkable = tileType == (int) TileTypes.FLOOR;
        hasEnemy = isEnemy;
        enemyType = eType;
    }
}


public class EnemyInfo {
    public Vector3Int position { get; set; }
    public int enemyType { get; set; }


    public EnemyInfo(Vector3Int pos, int type) {
        position = pos;
        enemyType = type;
    }
}