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

                for (int i = 0; i < path.Count; i++)
                {
                    tilemap.SetTileFlags(new Vector3Int(path[i].coord_x, path[i].coord_y, 0), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(path[i].coord_x, path[i].coord_y, 0), Color.blue);

                    Debug.Log("Tile " + i + " of path is: " + path[i]);
                }

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

    private bool isTargetVisible(Vector3 start, Vector3 end)
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
}


public class MapInfo
{
    GridInfo[,] map { get; }
    Vector3Int playerSpawn { get; set; }

    List<EnemyInfo> enemies;
    List<ObjectInfo> objects;

    public MapInfo(int rows, int cols)
    {
        map = new GridInfo[rows, cols];
        enemies = new List<EnemyInfo>();
        objects = new List<ObjectInfo>();
    }

    public void updateMapFakeInfo()
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                if (j == 0 || j == map.GetLength(1) - 1 || i == 0 || i == map.GetLength(0) - 1)
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.WALL, false);
                }
                else if (j == Mathf.FloorToInt(map.GetLength(1) / 2) && i == Mathf.FloorToInt(map.GetLength(0) / 2))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false, false, 0, true, 0);
                }
                else
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false);
                }

                // Setting Neighs
                if (i > 0)
                {
                    map[i, j].SetNeighbor(Direction.O, map[i - 1, j]);
                }

                if (j > 0)
                {
                    map[i, j].SetNeighbor(Direction.S, map[i, j - 1]);
                }

            }
        }

        map[(int)map.GetLength(0) / 2, 1].tileType = (int)TileTypes.FLOOR;
        map[(int)map.GetLength(0) / 2, 1].initPlayerPos = true;

        map[4, map.GetLength(1) - 3].hasObject = true;
        map[4, map.GetLength(1) - 3].objectType = 1;

        map[3, map.GetLength(1) - 2].hasEnemy = true;
        map[3, map.GetLength(1) - 2].enemyType = 0;

        //map[map.GetLength(0) - 3, map.GetLength(1) - 2].hasEnemy = true;
        //map[map.GetLength(0) - 3, map.GetLength(1) - 2].enemyType = 0;
    }


    public void generateMapInfo(Color[,] colorMap)
    {

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                Color color = colorMap[j, i];

                if (color.Equals(Color.black))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.WALL_TOP, false);
                }
                else if (color.Equals(Color.blue))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, true);
                }
                else if (color.Equals(Color.white))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false);
                }
                else if (color.Equals(Color.red))
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.FLOOR, false, true, Random.Range(0, 2)); // TODO: Fix this cause it sucks
                }
                else
                {
                    map[i, j] = new GridInfo(i, j, (int)TileTypes.WALL, false);
                }

                // Setting Neighs
                if (i > 0)
                {
                    map[i, j].SetNeighbor(Direction.O, map[i - 1, j]);
                }

                if (j > 0)
                {
                    map[i, j].SetNeighbor(Direction.S, map[i, j - 1]);
                }
            }
        }
    }

    public void drawTileMap(Tilemap tm, List<Tile> tileList)
    {

        //        Debug.Log("Map Size x: " + map.GetLength(0) + " y: " + map.GetLength(1));

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GridInfo info = map[i, j];
                if (info.tileType != (int)TileTypes.EMPTY)
                {
                    Tile tile = tileList[info.tileType];

                    tm.SetTile(new Vector3Int(i, j, 0), tile);

                    if (info.initPlayerPos)
                    {
                        playerSpawn = new Vector3Int(i, j, 0);
                    }

                    if (info.hasEnemy)
                    {
                        enemies.Add(new EnemyInfo(new Vector3Int(i, j, 0), info.enemyType));
                    }

                    if (info.hasObject)
                    {
                        objects.Add(new ObjectInfo(new Vector3Int(i, j, 0), info.objectType));
                    }
                }
            }
        }

    }

    public Vector3Int getPlayerSpawn()
    {
        return playerSpawn;
    }

    public List<EnemyInfo> getEnemiesList()
    {
        return enemies;
    }

    public List<ObjectInfo> getObjectsList()
    {
        return objects;
    }

    public bool isTileWalkable(int x, int y)
    {
        return map[x, y].walkable;
    }

    public bool isTileEnemy(int x, int y)
    {
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
                else
                {
                    tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(i, j, 0), Color.yellow);
                }
            }
        }
    }

    public void setTileWalkableState(int x, int y, bool isWalkable)
    {
        map[x, y].walkable = isWalkable;
    }

    public void setTileEnemyState(int x, int y, bool isEnemy)
    {
        map[x, y].hasEnemy = isEnemy;
    }

    public GridInfo getGridInfo(int x, int y)
    {
        return map[x, y];
    }
}


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

public class GridNavigator
{

    MapInfo mapInfo;
    Tilemap tilemap;

    public GridNavigator(MapInfo map, Tilemap tmap)
    {
        mapInfo = map;
        this.tilemap = tmap;
    }


    public List<GridInfo> GeneratePath(Vector3Int start, Vector3Int objective)
    {

        List<ANode> openList = new List<ANode>();
        List<ANode> closedList = new List<ANode>();
        List<GridInfo> path;

        GridInfo current = mapInfo.getGridInfo(start.x, start.y);
        GridInfo goal = mapInfo.getGridInfo(objective.x, objective.y);


        //Debug.Log("Tile de inicio: " + current);
        //Debug.Log("Tile objetivo: " + goal);


        ANode initialNode = new ANode(current, 0f, 0f);
        ANode destinationNode = new ANode(goal, 0f, 0f);

        openList.Add(initialNode);

        //Debug.Log(openList.Count);
        //Debug.Log(closedList.Count);

        while (openList.Count > 0)
        {

            //Debug.Log(openList.Count);
            //Debug.Log(closedList.Count);

            ANode currentNode = openList[0];

            //Get current node
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            // Remove current node from open and add to closed
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (checkIfPathFound(currentNode, destinationNode, out path))
            {
                return path;
            }

            //Create Children
            List<ANode> children = CreateCurrentNodeChildren(currentNode);

            mapInfo.markNonWalkableTiles(tilemap);

            initializeValidChildren(currentNode, children, openList, closedList, goal);


            for (int i = 0; i < closedList.Count; i++)
            {
                tilemap.SetTileFlags(new Vector3Int(closedList[i].GridInfo.coord_x, closedList[i].GridInfo.coord_y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(closedList[i].GridInfo.coord_x, closedList[i].GridInfo.coord_y, 0), Color.green);
            }
        }

        return null;
    }



    private bool checkIfPathFound(ANode current, ANode dest, out List<GridInfo> path)
    {
        path = null;

        if (current.Equals(dest))
        {

            path = new List<GridInfo>();
            ANode cur = current;

            while (cur != null)
            {
                path.Add(cur.GridInfo);
                cur = cur.GetParentNode();
            }

            path.Reverse();
            return true;
        }

        return false;
    }

    private List<ANode> CreateCurrentNodeChildren(ANode current)
    {
        List<ANode> children = new List<ANode>();

        for (Direction d = Direction.N; d <= Direction.O; d++)
        {

            GridInfo neighborCell = current.GridInfo.GetNeighbor(d);

            if (neighborCell != null)
            {
                ANode child = new ANode(neighborCell, 0, 0);
                child.SetParentNode(current);
                children.Add(child);
            }

        }

        return children;
    }


    private void initializeValidChildren(ANode currentNode, List<ANode> children, List<ANode> openList, List<ANode> closedList, GridInfo target)
    {
        foreach (ANode child in children)
        {
            if (closedList.IndexOf(child) == -1)
            {
                if (openList.IndexOf(child) == -1)
                {
                    child.G = currentNode.G + 1;

                    if (child.GridInfo.walkable || child.GridInfo.Equals(target))
                    {
                        float deltaX = (child.GridInfo.coord_x - target.coord_x);
                        float deltaY = (child.GridInfo.coord_y - target.coord_y);
                        child.H =
                           (deltaX * deltaX) +
                           (deltaY * deltaY)
                       ;
                    }
                    else
                    {
                        child.H = float.MaxValue;
                    }


                    openList.Add(child);
                }
            }
        }
    }

}


public class ANode
{
    public GridInfo GridInfo { get; set; }

    // total cost of node
    public float F
    {
        get
        {
            return H + G;
        }
    }
    //distance between current node and start node
    public float G { get; set; }
    //heuristic
    public float H { get; set; }

    private ANode parentNode = null;

    public ANode(GridInfo gridInfo, float g, float h)
    {
        GridInfo = gridInfo;
        G = g;
        H = h;
    }

    public ANode GetParentNode()
    {
        return parentNode;
    }

    public void SetParentNode(ANode node)
    {
        this.parentNode = node;
    }

    public override bool Equals(object obj)
    {
        if (obj is ANode)
        {
            ANode o = (ANode)obj;
            return this.GridInfo.Equals(o.GridInfo);
        }

        return false;
    }
}

public enum Direction
{
    N, E, S, O
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        return (int)direction < 2 ? (direction + 2) : (direction - 2);
    }

}