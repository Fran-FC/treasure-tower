using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



enum TileTypes { 
    WALL,
    FLOOR,
    OBJECT,
    EMPTY,
    WALL_TOP
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

    private Vector3Int playerSpawn;

    [SerializeField]
    private List<GameObject> enemyTypesList;

    [SerializeField]
    private List<GameObject> objectTypesList;

    [SerializeField]
    private CinemachineVirtualCamera cam;

    [SerializeField]
    private bool colorNotWalkableTiles = false;

    [SerializeField]
    private bool useFakeInfo = false;


    private void Awake()
    {
        Messenger.AddListener(GameEvent.SPAWN_ENEMY, spawnEnemies);
       
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.SPAWN_ENEMY, spawnEnemies);
    }

    // Start is called before the first frame update
    void Start()
    {
        Grid grid = new GameObject("Grid").AddComponent<Grid>();
        tilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
        tilemap.gameObject.AddComponent<TilemapRenderer>();

        tilemap.transform.SetParent(grid.transform);
       
        if (useFakeInfo)
        {
            tilemap.transform.position = new Vector2(-rows / 2, -cols / 2);
            mapInfo = new MapInfo(rows, cols);
            mapInfo.updateMapFakeInfo();

            mapInfo.drawTileMap(tilemap, tileList);

            playerSpawn = mapInfo.getPlayerSpawn();

            if (playerSpawn != null)
            {
                Vector3 center = tilemap.GetCellCenterWorld(playerSpawn);
                cam.Follow = Instantiate(player, center, Quaternion.identity).transform;
            }

            spawnObjects();
        }
    }

    private void Update()
    {
        if (colorNotWalkableTiles) { 
            mapInfo.markNonWalkableTiles(tilemap);
        }
    }


    public bool isTileWalkable(float v_x, float v_y) {

        int x = (int) Mathf.Floor(v_x);
        int y = (int) Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x,y,0));
        return mapInfo.isTileWalkable(cellPos.x, cellPos.y);
    }
    public bool isTileEnemy(float v_x, float v_y) {

        int x = (int) Mathf.Floor(v_x);
        int y = (int) Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x,y,0));
        return mapInfo.isTileEnemy(cellPos.x, cellPos.y);
    }

    public void setTileWalkableState(float v_x, float v_y, bool isWalkable) {
        int x = (int)Mathf.Floor(v_x);
        int y = (int)Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
        mapInfo.setTileWalkableState(cellPos.x, cellPos.y, isWalkable);
    }

    public void setTileEnemyState(float v_x, float v_y, bool isEnemy) {
        int x = (int)Mathf.Floor(v_x);
        int y = (int)Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
        mapInfo.setTileEnemyState(cellPos.x, cellPos.y, isEnemy);
    }
   

    public void spawnEnemies() {
        List<EnemyInfo> enemies = mapInfo.getEnemiesList();

        for (int i = 0; i < enemies.Count; i++)
        {

            EnemyInfo enemy = enemies[i];

            Vector3 enemyPos = tilemap.GetCellCenterWorld(enemy.position);
            GameObject enemyPrefab = enemyTypesList[enemy.enemyType];

            Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
        }
    }

    public void spawnObjects() {
        List<ObjectInfo> objects = mapInfo.getObjectsList();

        for (int i = 0; i < objects.Count; i++)
        {

            ObjectInfo obj = objects[i];

            Vector3 objPos = tilemap.GetCellCenterWorld(obj.position);

            GameObject objPrefab = objectTypesList[obj.objectType];

            Instantiate(objPrefab, objPos, Quaternion.identity);

        }
    }


    public void drawMap(Color[,] colorMap) {

        tilemap.transform.position = new Vector2(-colorMap.GetLength(0) / 2, -colorMap.GetLength(1) / 2);
        mapInfo = new MapInfo(colorMap.GetLength(1), colorMap.GetLength(0));
        mapInfo.generateMapInfo(colorMap);

        mapInfo.drawTileMap(tilemap, tileList);

        playerSpawn = mapInfo.getPlayerSpawn();

        if (playerSpawn != null)
        {
            Vector3 center = tilemap.GetCellCenterWorld(playerSpawn);
            cam.Follow = Instantiate(player, center, Quaternion.identity).transform;
        }

        spawnEnemies();
        spawnObjects();
    }

    public bool checkFakeInfo() {
        return this.useFakeInfo;
    }

}


public class MapInfo 
{
    GridInfo[,] map { get; }
    Vector3Int playerSpawn { get; set; }

    List<EnemyInfo> enemies;
    List<ObjectInfo> objects;

    public MapInfo(int rows, int cols) {
        map = new GridInfo[rows, cols];
        enemies = new List<EnemyInfo>();
        objects = new List<ObjectInfo>();
    }

    public void updateMapFakeInfo() {

        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++) {

                if (j == 0 || j == map.GetLength(1) - 1 || i == 0 || i == map.GetLength(0) - 1)
                {
                    map[i, j] = new GridInfo((int)TileTypes.WALL, false);
                }
                else if (j == Mathf.FloorToInt(map.GetLength(1) / 2) && i == Mathf.FloorToInt(map.GetLength(0) / 2))  {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false, false, 0, true, 0);
                }
                else {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false);
                }

            }
        }

        map[(int)map.GetLength(0) / 2, 1] = new GridInfo((int)TileTypes.FLOOR, true);


        map[4, map.GetLength(1) - 3] = new GridInfo((int)TileTypes.FLOOR, false, false, 0, true, 1);

        map[3, map.GetLength(1) - 2] = new GridInfo((int)TileTypes.FLOOR, false, true, 0);
        map[map.GetLength(0) - 3, map.GetLength(1) - 2] = new GridInfo((int)TileTypes.FLOOR, false, true, 0);
    }


    public void generateMapInfo(Color[,] colorMap) {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                Color color = colorMap[j, i];

                if (color.Equals(Color.black))
                {
                    map[i, j] = new GridInfo((int)TileTypes.WALL_TOP, false);
                }
                else if (color.Equals(Color.blue))
                {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, true);
                }
                else if (color.Equals(Color.white))
                {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false);
                }
                else if (color.Equals(Color.red)) 
                {
                    map[i, j] = new GridInfo((int)TileTypes.FLOOR, false, true, Random.Range(0, 2)); // TODO: Fix this cause it sucks
                }
                else
                {
                    map[i, j] = new GridInfo((int)TileTypes.WALL, false);
                }
            }
        }
    }

    public void drawTileMap(Tilemap tm, List<Tile> tileList) {

//        Debug.Log("Map Size x: " + map.GetLength(0) + " y: " + map.GetLength(1));

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GridInfo info = map[i, j];
                if (info.tileType != (int)TileTypes.EMPTY) {
                    Tile tile = tileList[info.tileType];

                    tm.SetTile(new Vector3Int(i, j, 0), tile);

                    if (info.initPlayerPos) {
                        playerSpawn = new Vector3Int(i, j, 0);
                    }

                    if (info.hasEnemy) {
                        enemies.Add(new EnemyInfo(new Vector3Int(i, j, 0), info.enemyType));
                    }

                    if (info.hasObject) {
                        objects.Add(new ObjectInfo(new Vector3Int(i, j, 0), info.objectType));
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

    public List<ObjectInfo> getObjectsList()
    {
        return objects;
    }

    public bool isTileWalkable(int x, int y) {
        return map[x, y].walkable;
    }

    public bool isTileEnemy(int x, int y) {
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
                else {
                    tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(i, j, 0), Color.yellow);
                }
            }
        }
    }

    public void setTileWalkableState(int x, int y, bool isWalkable) {
        map[x, y].walkable = isWalkable;
    }

    public void setTileEnemyState(int x, int y, bool isEnemy) {
        map[x, y].hasEnemy = isEnemy;
    }
}


public class GridInfo 
{
    public int tileType {  get; set; }
    public bool walkable { get; set; }
    public bool initPlayerPos { get; set; }

    public bool hasEnemy { get; set; }
    public int enemyType { get; set; }

    public bool hasObject { get; set; }
    public int objectType { get; set; }


    public GridInfo(int type, bool isPlayerPos, bool isEnemy = false, int eType = -1, bool isObject = false, int oType = -1) {
        tileType = type;
        initPlayerPos = isPlayerPos;
        walkable = tileType == (int) TileTypes.FLOOR;
        hasEnemy = isEnemy;
        enemyType = eType;

        hasObject = isObject;
        objectType = oType;
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


public class ObjectInfo {
    public Vector3Int position { get; set; }
    public int objectType { get; set; }


    public ObjectInfo(Vector3Int pos, int type)
    {
        position = pos;
        objectType = type;
    }
}