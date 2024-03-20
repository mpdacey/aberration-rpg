using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public RawImage mapImage;
    public RawImage mapMask;
    public Color fillColour;
    public Color outlineColour;
    private Vector2 startPosition;
    private Texture2D mapMaskTexture;
    private float mazeRatio = 1f;

    private void OnEnable()
    {
        MazeGenerator.MazeTextureGenerated += DrawNewMinimap;
        FieldMovementController.PlayerPositionChanged += UpdatePlayerPosition;
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

        mazeRatio = mazeTexture.width / 8f;
        mapMask.transform.localScale = Vector2.one * 2 * mazeRatio;

        Color[] mapCells = new Color[mazeTexture.width * mazeTexture.height * 36];
        Color[] mazeCells = mazeTexture.GetPixels();
        Texture2D minimapTexture = new Texture2D(mazeTexture.width * 6, mazeTexture.height * 6);

        for (int y = 0; y < mazeTexture.height; y++)
        {
            for(int x = 0; x < mazeTexture.width; x++)
            {
                Color currentCell = mazeCells[y * mazeTexture.width + x];

                if (currentCell.r == 1)
                {
                    Vector2Int currentCoords = new Vector2Int(x, y);
                    FillCell(ref mapCells, currentCoords, mazeTexture.width);
                    if (x == 0 || mazeCells[y * mazeTexture.width + x - 1].r == 0) DrawOutline(ref mapCells, currentCoords*6, mazeTexture.width, false);
                    if (x == mazeTexture.width-1 || mazeCells[y * mazeTexture.width + x + 1].r == 0) DrawOutline(ref mapCells, (currentCoords*6) + (Vector2Int.right*5), mazeTexture.width, false);
                    if (y == 0 || mazeCells[(y-1) * mazeTexture.width + x].r == 0) DrawOutline(ref mapCells, currentCoords*6, mazeTexture.width, true);
                    if (y == mazeTexture.height - 1 || mazeCells[(y+1) * mazeTexture.width + x].r == 0) DrawOutline(ref mapCells, currentCoords* 6 + (Vector2Int.up * 5), mazeTexture.width, true);

                    if (currentCell.g == 0 && currentCell.b == 0)
                        startPosition = new Vector2(x, y);
                }
            }
        }

        minimapTexture.filterMode = FilterMode.Point;
        minimapTexture.SetPixels(mapCells);
        minimapTexture.Apply();
        mapImage.texture = minimapTexture;

        UpdatePlayerPosition(Vector2.zero);
    }

    private void FillCell(ref Color[] cells, Vector2Int currentCell, int baseWidth)
    {
        for (int y = 0; y < 6; y++)
            for (int x = 0; x < 6; x++)
                cells[(currentCell.y * 6 + y) * baseWidth * 6 + currentCell.x * 6 + x] = fillColour;
    }

    private void DrawOutline(ref Color[] cells, Vector2Int topLeft, int baseWidth, bool horizontalLine)
    {
        if (horizontalLine)
            for (int x = 0; x < 6; x++)
                cells[topLeft.y * baseWidth*6 + topLeft.x + x] = outlineColour;
        else
            for (int y = 0; y < 6; y++)
                cells[(topLeft.y+y) * baseWidth * 6 + topLeft.x] = outlineColour;
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
