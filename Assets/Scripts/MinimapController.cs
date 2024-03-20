using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Sprite[] tiles;
    public RawImage mapImage;
    public RawImage mapMask;

    private void OnEnable()
    {
        MazeGenerator.MazeTextureGenerated += DrawNewMinimap;
    }

    private void OnDisable()
    {
        MazeGenerator.MazeTextureGenerated -= DrawNewMinimap;
    }

    private void DrawNewMinimap(Texture2D mazeTexture)
    {
        Vector2Int spriteSize = new Vector2Int((int)tiles[0].rect.width, (int)tiles[0].rect.height);
        Texture2D minimapTexture = new Texture2D(mazeTexture.width * spriteSize.x, mazeTexture.height * spriteSize.y);
        minimapTexture.filterMode = FilterMode.Point;

        for (int y = 0; y < mazeTexture.height; y++)
        {
            for(int x = 0; x < mazeTexture.width; x++)
            {
                Color currentCell = mazeTexture.GetPixel(x, y);

                if (currentCell.r == 1)
                {
                    minimapTexture.SetPixels(x * spriteSize.x, y * spriteSize.y, spriteSize.x, spriteSize.y, tiles[0].texture.GetPixels(0,0, spriteSize.x, spriteSize.y));
                    minimapTexture.Apply();
                }
            }
        }

        mapImage.texture = minimapTexture;
    }
}
