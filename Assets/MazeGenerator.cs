using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public Vector2Int gridSize = new Vector2Int(16,16);
    public List<Vector2Int> visited = new List<Vector2Int>();
    public List<Vector2Int> walls = new List<Vector2Int>();
    public SpriteRenderer debugSpriteRenderer;
    private Color[] cells;
    private Texture2D generatedTexture;

    private void Start()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        cells = new Color[gridSize.x * gridSize.y];
        for (int i = 0; i < cells.Length; i++)
            cells[i] = Color.black;

        int startX = Mathf.FloorToInt((float)Random.Range(0, gridSize.x - 1)/2)*2;
        int startY = Mathf.FloorToInt((float)Random.Range(0, gridSize.y - 1)/2)*2;
        Vector2Int start = new Vector2Int(startX, startY);

        cells[start.y * gridSize.x + start.x] = Color.red;

        visited.Add(start);
        AddWalls(start);

        while(walls.Count > 0)
        {
            var randomWall = walls[Random.Range(0, walls.Count - 1)];

            if(randomWall.x % 2 == 1 && (visited.Contains(randomWall + Vector2Int.left) ^ visited.Contains(randomWall + Vector2Int.right)))
            {
                cells[randomWall.y * gridSize.x + randomWall.x] = Color.white;
                if (visited.Contains(randomWall + Vector2Int.left))
                    AddVisited(randomWall + Vector2Int.right);
                else
                    AddVisited(randomWall + Vector2Int.left);
            }
            else if (visited.Contains(randomWall + Vector2Int.up) ^ visited.Contains(randomWall + Vector2Int.down))
            {
                cells[randomWall.y * gridSize.x + randomWall.x] = Color.white;
                if (visited.Contains(randomWall + Vector2Int.up))
                    AddVisited(randomWall + Vector2Int.down);
                else
                    AddVisited(randomWall + Vector2Int.up);
            }

            walls.Remove(randomWall);
        }

        generatedTexture = new Texture2D(gridSize.x, gridSize.y);
        generatedTexture.filterMode = FilterMode.Point;
        generatedTexture.SetPixels(cells);
        generatedTexture.Apply();
        Sprite sprite = Sprite.Create(generatedTexture, new Rect(0, 0, gridSize.x, gridSize.y), Vector2.zero, 16f);
        debugSpriteRenderer.sprite = sprite;
    }

    private void AddVisited(Vector2Int newCell)
    {
        cells[newCell.y * gridSize.x + newCell.x] = Color.white;
        visited.Add(newCell);
        AddWalls(newCell);
    }

    private void AddWalls(Vector2Int currentCell)
    {
        if (currentCell.x - 2 >= 0 && !walls.Contains(currentCell + Vector2Int.left))
            walls.Add(currentCell + Vector2Int.left);

        if (currentCell.y - 2 >= 0 && !walls.Contains(currentCell + Vector2Int.down))
            walls.Add(currentCell + Vector2Int.down);

        if (currentCell.x + 2 < gridSize.x && !walls.Contains(currentCell + Vector2Int.right))
            walls.Add(currentCell + Vector2Int.right);

        if (currentCell.y + 2 < gridSize.y && !walls.Contains(currentCell + Vector2Int.up))
            walls.Add(currentCell + Vector2Int.up);
    }
}
