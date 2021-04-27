using UnityEngine;

public class DebugRoomRendererFromSprite : MonoBehaviour
{
    //defines
    private const int UP = 0;
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;

    private static int roomH = 15;
    private static int roomW = 20;
    public Texture2D room;

    // Start is called before the first frame update
    void Start()
    {
        //generateLevel();
    }

    private void OnDrawGizmos()
    {
        //generateLevel();
    }

    void generateLevel()
    {
        for(int x=0; x < room.width; x++)
        {
            for(int y=0; y<room.height; y++)
            {
                generateTile(x, y);
            }
        }
    }

    void generateTile(int x, int y)
    {
        Color pixelColor = room.GetPixel(x, y);
        if(pixelColor.a == 0)
        {
            //the pixel is transparent, so we do nothing
            return;
        }
        //the pixel has color, so we draw it
        Vector3 pos = new Vector3(-roomW / 2 + x + .5f, -roomH / 2 + y + .5f, 0);
        Gizmos.color = pixelColor;
        Gizmos.DrawCube(transform.position + pos, Vector3.one);
    }

    public void populateColorMap(ref Color[,] colorMap, int w, int h)
    {
        //populate my colors to the main map at room w, h
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                Color pixelColor = room.GetPixel(x, y);
                if (pixelColor.a == 0)
                {
                    //the pixel is transparent, so we do nothing
                    continue;
                }
                colorMap[(h * 15) + y, (w * 20) + x] = pixelColor;
            }
        }
    }
    
    public void closeConectionAt(ref Color[,] colorMap, int w, int h, int dir)
    {
        switch (dir)
        {
            case UP:
                colorMap[(h * 15) + 14, (w * 20) + 8] = Color.black;
                colorMap[(h * 15) + 14, (w * 20) + 9] = Color.black;
                colorMap[(h * 15) + 14, (w * 20) + 10] = Color.black;
                colorMap[(h * 15) + 14, (w * 20) + 11] = Color.black;
                
                colorMap[(h * 15) + 13, (w * 20) + 8] = Color.gray;
                colorMap[(h * 15) + 13, (w * 20) + 9] = Color.gray;
                colorMap[(h * 15) + 13, (w * 20) + 10] = Color.gray;
                colorMap[(h * 15) + 13, (w * 20) + 11] = Color.gray;
                break;
            case RIGHT:
                colorMap[(h * 15) + 6, (w * 20) + 19] = Color.black;
                colorMap[(h * 15) + 7, (w * 20) + 19] = Color.black;
                colorMap[(h * 15) + 8, (w * 20) + 19] = Color.black;
                colorMap[(h * 15) + 9, (w * 20) + 19] = Color.black;
                break;
            case DOWN:
                colorMap[(h * 15), (w * 20) + 8] = Color.black;
                colorMap[(h * 15), (w * 20) + 9] = Color.black;
                colorMap[(h * 15), (w * 20) + 10] = Color.black;
                colorMap[(h * 15), (w * 20) + 11] = Color.black;
                break;
            case LEFT:
                colorMap[(h * 15) + 6, (w * 20)] = Color.black;
                colorMap[(h * 15) + 7, (w * 20)] = Color.black;
                colorMap[(h * 15) + 8, (w * 20)] = Color.black;
                colorMap[(h * 15) + 9, (w * 20)] = Color.black;
                break;

        }
    }

}
