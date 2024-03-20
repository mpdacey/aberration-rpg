using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Sprite[] tiles;
    public RawImage mapImage;
    public RawImage mapMask;
    private Vector2 startPosition;

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

                    if (currentCell.g == 0 && currentCell.b == 0)
                        startPosition = new Vector2(x, y);
                }
            }
        }

        mapMask.transform.localScale = new Vector2((mazeTexture.width + 1) * 1.5f + 1, (mazeTexture.height + 1) * 1.5f + 1);
        mapImage.transform.localScale = new Vector2(1f/spriteSize.x, 1f / spriteSize.y);
        mapImage.texture = minimapTexture;

        mapMask.transform.localPosition = new Vector2(27.5f, 27.5f) + startPosition * -25f;
    }
}
