using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum TileTypes
{
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


    private GridNavigator mapNav;


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

            mapNav = new GridNavigator(mapInfo, tilemap);

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
        if (colorNotWalkableTiles)
        {
            mapInfo.markNonWalkableTiles(tilemap);
        }
    }


    public bool isTileWalkable(float v_x, float v_y)
    {

        int x = (int)Mathf.Floor(v_x);
        int y = (int)Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
        return mapInfo.isTileWalkable(cellPos.x, cellPos.y);
    }
    public bool isTileEnemy(float v_x, float v_y)
    {

        int x = (int)Mathf.Floor(v_x);
        int y = (int)Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
        return mapInfo.isTileEnemy(cellPos.x, cellPos.y);
    }

    public void setTileWalkableState(float v_x, float v_y, bool isWalkable)
    {
        int x = (int)Mathf.Floor(v_x);
        int y = (int)Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
        mapInfo.setTileWalkableState(cellPos.x, cellPos.y, isWalkable);
    }

    public void setTileEnemyState(float v_x, float v_y, bool isEnemy)
    {
        int x = (int)Mathf.Floor(v_x);
        int y = (int)Mathf.Floor(v_y);

        Vector3Int cellPos = tilemap.WorldToCell(new Vector3(x, y, 0));
        mapInfo.setTileEnemyState(cellPos.x, cellPos.y, isEnemy);
    }


    public void spawnEnemies()
    {
        List<EnemyInfo> enemies = mapInfo.getEnemiesList();

        for (int i = 0; i < enemies.Count; i++)
        {

            EnemyInfo enemy = enemies[i];

            Vector3 enemyPos = tilemap.GetCellCenterWorld(enemy.position);
            GameObject enemyPrefab = enemyTypesList[enemy.enemyType];

            Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
        }
    }

    public void spawnObjects()
    {
        List<ObjectInfo> objects = mapInfo.getObjectsList();

        for (int i = 0; i < objects.Count; i++)
        {

            ObjectInfo obj = objects[i];

            Vector3 objPos = tilemap.GetCellCenterWorld(obj.position);

            GameObject objPrefab = objectTypesList[obj.objectType];

            Instantiate(objPrefab, objPos, Quaternion.identity);

        }
    }


    public void drawMap(Color[,] colorMap)
    {

        tilemap.transform.position = new Vector2(-colorMap.GetLength(0) / 2, -colorMap.GetLength(1) / 2);
        mapInfo = new MapInfo(colorMap.GetLength(1), colorMap.GetLength(0));
        mapInfo.generateMapInfo(colorMap);

        mapNav = new GridNavigator(mapInfo, tilemap);

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

    public bool checkFakeInfo()
    {
        return this.useFakeInfo;
    }


    public Vector3 DrawPath(Vector3 start, Vector3 end)
    {

        if (Vector3.Distance(start, end) < 10f && isTargetVisible(start, end))
        {
            Vector3Int s = tilemap.WorldToCell(start);
            Vector3Int e = tilemap.WorldToCell(end);

            List<GridInfo> path = mapNav.GeneratePath(s, e);


            if (path != null)
            {

                //mapInfo.markNonWalkableTiles(tilemap);

                /*for (int i = 0; i < path.Count; i++)
                {
                    //tilemap.SetTileFlags(new Vector3Int(path[i].coord_x, path[i].coord_y, 0), TileFlags.None);
                    //tilemap.SetColor(new Vector3Int(path[i].coord_x, path[i].coord_y, 0), Color.blue);

                    //Debug.Log("Tile " + i + " of path is: " + path[i]);
                }*/

            }
            else
            {
                Debug.Log("Path is null...");
            }

            if (path != null && path.Count > 0)
            {
                GridInfo nextStep = path[1];

                //Debug.Log("Next tile is: " + nextStep);
                return tilemap.GetCellCenterWorld(new Vector3Int(nextStep.coord_x, nextStep.coord_y, 0));
            }
        }

        return start;
    }

    public List<GridInfo> GetValidPath(Vector3 start, Vector3 end) {
        
        Vector3Int s = tilemap.WorldToCell(start);
        Vector3Int e = tilemap.WorldToCell(end);

        List<GridInfo> path = mapNav.GeneratePath(s, e);


        if (path != null)
        {

            //mapInfo.markNonWalkableTiles(tilemap);

            path.RemoveAt(0);

            /*for (int i = 0; i < path.Count; i++)
            {
                tilemap.SetTileFlags(new Vector3Int(path[i].coord_x, path[i].coord_y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(path[i].coord_x, path[i].coord_y, 0), Color.blue);

                //Debug.Log("Tile " + i + " of path is: " + path[i]);
            }*/

        }
        else
        {
            Debug.Log("Path is null...");
        }

        return path;
    }

    public bool isTargetVisible(Vector3 start, Vector3 end)
    {
        //brenham's line algorithm to stablish line of sight
        Vector3Int s = tilemap.WorldToCell(start);
        Vector3Int e = tilemap.WorldToCell(end);
        int dx = e.x - s.x;
        int dy = e.y - s.y;
        float slope;
        if (dx == 0) slope = dy;
        else slope = (float)dy / dx;

        dx = Mathf.Abs(dx);
        dy = Mathf.Abs(dy);

        if(Mathf.Abs(slope) < 1)
        {
            //if the endpoint is behind we swap it
            if(s.x > e.x)
            {
                Vector3Int temp = s;
                s = e;
                e = temp;
            }
            int p = (2 * dy) - dx;
            int x = s.x;
            int y = s.y;

            while (x < e.x)
            {
                x++;
                if (p >= 0)
                {
                    if (slope < 1) { y++; }
                    else { y--; }

                    p = p + (2 * dy) - (2 * dx);
                }
                else
                {
                    p = p + (2 * dy);
                }
                //check coordinates for an unwalkable tile
                GridInfo current = mapInfo.getGridInfo(x, y);
                //TO BE CHANGED, ESTO ES PATATA Y SE VA A ROMPER COMO ESCALEMOS UN POCO LAS NECESIDADES. SOLO COMPRUEBA SI ES CAMINABLE Y SI ES SUELO. SIEMPRE QUE SEA SUELO PODRÁ VERTE 
                if (!current.walkable && !(current.tileType == (int)TileTypes.FLOOR)) { return false; }
            }
        }
        else if(Mathf.Abs(slope) >= 1)
        {
            //if the endpoint is behind we swap it
            if (s.y > e.y)
            {
                Vector3Int temp = s;
                s = e;
                e = temp;
            }
            int p = (2 * dx) - dy;
            int x = s.x;
            int y = s.y;

            while (y < e.y)
            {
                y++;
                if (p >= 0)
                {
                    if (slope >= 1) { x++; }
                    else { x--; }

                    p = p + (2 * dx) - (2 * dy);
                }
                else
                {
                    p = p + (2 * dx);
                }
                //check coordinates for an unwalkable tile
                GridInfo current = mapInfo.getGridInfo(x, y);
                //TO BE CHANGED, ESTO ES PATATA Y SE VA A ROMPER COMO ESCALEMOS UN POCO LAS NECESIDADES. SOLO COMPRUEBA SI ES CAMINABLE Y SI ES SUELO. SIEMPRE QUE SEA SUELO PODRÁ VERTE 
                if (!current.walkable && !(current.tileType == (int)TileTypes.FLOOR)) { return false; }
            }
        }
        //if it has arrived here it means all tiles in the way are walkable, therefore visible
        return true;
    }

    public Vector3 getGridInfoGlobalTransform(GridInfo tile) {
        return tilemap.GetCellCenterWorld(new Vector3Int(tile.coord_x, tile.coord_y, 0));
    }
}
