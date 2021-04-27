using UnityEngine;

public class DebugRoomRendererFromSprite : MonoBehaviour
{
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

}
