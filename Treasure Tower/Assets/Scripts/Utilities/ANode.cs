using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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