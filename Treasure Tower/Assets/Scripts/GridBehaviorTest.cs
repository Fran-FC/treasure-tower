using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviorTest : MonoBehaviour
{
    public int rows;
    public int columns;
    public int scale = 1;
    public GameObject gridPrefab;
    public Vector3 leftBottonLocation = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GenerateGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for(int j=0; j < rows; j++)
            {
                GameObject obj = Instantiate(gridPrefab, new Vector3(leftBottonLocation.x + scale*i ,leftBottonLocation.y + scale*j,0), Quaternion.identity);
            }
        }
    }
}
