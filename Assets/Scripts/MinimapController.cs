using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Sprite[] tiles;
    public RawImage mapImage;
    public RawImage mapMask;
    public Transform player;
    private Vector2 startPosition;
    private Texture2D mapMaskTexture;
    private float mazeRatio = 1f;

    private void OnEnable()
    {
        MazeGenerator.MazeTextureGenerated += DrawNewMinimap;
        FieldMovementController.PlayerPositionChanged += UpdatePlayerPosition;
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnDisable()
    {
        MazeGenerator.MazeTextureGenerated -= DrawNewMinimap;
        FieldMovementController.PlayerPositionChanged -= UpdatePlayerPosition;
    }

    private void DrawNewMinimap(Texture2D mazeTexture)
    {
        mapMaskTexture = new Texture2D(mazeTexture.width, mazeTexture.height);
        mapMaskTexture.filterMode = FilterMode.Point;
        Color[] mapMaskCells = mapMaskTexture.GetPixels();

        for (int i = 0; i < mapMaskCells.Length; i++)
            mapMaskCells[i] = Color.clear;

        mapMaskTexture.SetPixels(mapMaskCells);
        mapMaskTexture.Apply();
        mapMask.texture = mapMaskTexture;

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

        mazeRatio = mazeTexture.width / 8f;
        mapMask.transform.localScale = Vector2.one * 2 * mazeRatio;
        mapImage.texture = minimapTexture;

        UpdatePlayerPosition(Vector2.zero);
    }

    private void UpdatePlayerPosition(Vector3 playerPosition)
    {
        Vector2 currentPosition = startPosition + new Vector2(playerPosition.x / 5, playerPosition.z / 5);
        mapMask.transform.localPosition = new Vector2(12.5f, 12.5f) * ((mazeRatio-1)*8+1) + new Vector2(15,15) + currentPosition * -25f;
        UpdateMask(currentPosition);
    }

    private void UpdateMask(Vector2 currentPosition)
    {
        mapMaskTexture.SetPixel(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y), Color.white);
        mapMaskTexture.Apply();
        mapMask.texture = mapMaskTexture;
    }
}
