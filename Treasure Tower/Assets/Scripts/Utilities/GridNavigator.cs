using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

            //mapInfo.markNonWalkableTiles(tilemap);

            initializeValidChildren(currentNode, children, openList, closedList, goal);


            /*for (int i = 0; i < closedList.Count; i++)
            {
                tilemap.SetTileFlags(new Vector3Int(closedList[i].GridInfo.coord_x, closedList[i].GridInfo.coord_y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(closedList[i].GridInfo.coord_x, closedList[i].GridInfo.coord_y, 0), Color.green);
            }*/
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

                    if(child.F < 40)
                    {
                        openList.Add(child);
                    }
                }
            }
        }
    }

}

