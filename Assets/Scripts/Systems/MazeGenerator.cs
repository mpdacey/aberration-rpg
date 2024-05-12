using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    public static event Action<Texture2D> MazeTextureGenerated;

    public int startingSize = 8;
    public List<Vector2Int> visited = new List<Vector2Int>();
    public List<Vector2Int> walls = new List<Vector2Int>();
    private Vector2Int gridSize = new Vector2Int(16, 16);
    public SpriteRenderer testRenderer;
    private Color[] cells;
    private Texture2D generatedTexture;

    private void OnEnable()
    {
        GoalRiftController.GoalRiftEntered += GenerateMazeTexture;
        SceneController.CombatSceneLoaded += GenerateMazeTexture;
    }

    private void OnDisable()
    {
        GoalRiftController.GoalRiftEntered -= GenerateMazeTexture;
        SceneController.CombatSceneLoaded -= GenerateMazeTexture;
    }

    private void GenerateMazeTexture()
    {
        var newFloorGridSize = startingSize + (GameController.CurrentLevel / 3);
        gridSize = new Vector2Int(newFloorGridSize, newFloorGridSize);
        cells = new Color[gridSize.x * gridSize.y];
        int minimumCellCount;

        do
        {
            cells = GetGeneratedMaze(out minimumCellCount);
        }
        while (minimumCellCount > 0);

        generatedTexture = new Texture2D(gridSize.x, gridSize.y);
        generatedTexture.filterMode = FilterMode.Point;
        generatedTexture.SetPixels(cells);
        generatedTexture.Apply();

        if (testRenderer != null)
        {
            Sprite testSprite = Sprite.Create(generatedTexture, new Rect(0, 0, gridSize.x, gridSize.y), Vector2.zero, 16f);
            testRenderer.sprite = testSprite;
        }

        if (MazeTextureGenerated != null)
            MazeTextureGenerated.Invoke(generatedTexture);
    }

    private Color[] GetGeneratedMaze(out int minimumCount)
    {
        for (int i = 0; i < cells.Length; i++)
            cells[i] = Color.black;

        int startX = Mathf.FloorToInt((float)Random.Range(0, gridSize.x - 1) / 2) * 2;
        int startY = Mathf.FloorToInt((float)Random.Range(0, gridSize.y - 1) / 2) * 2;
        Vector2Int start = new Vector2Int(startX, startY);
        Vector2Int end = new Vector2Int(0, 0);

        int loopFactor = Random.Range(2, 5);

        cells[start.y * gridSize.x + start.x] = Color.red;

        visited.Clear();
        walls.Clear();

        visited.Add(start);
        AddWalls(start);

        minimumCount = gridSize.x / 2;

        while (walls.Count > 0)
        {
            var randomWall = walls[Random.Range(0, walls.Count - 1)];
            walls.Remove(randomWall);
            var canLoop = 0 == Random.Range(0, loopFactor);

            if (randomWall.x % 2 == 1 && (canLoop || (visited.Contains(randomWall + Vector2Int.left) ^ visited.Contains(randomWall + Vector2Int.right))))
            {
                if (randomWall + Vector2Int.left == end || randomWall + Vector2Int.right == end)
                    continue;

                cells[randomWall.y * gridSize.x + randomWall.x] = Color.white;
                if (visited.Contains(randomWall + Vector2Int.left) && visited.Contains(randomWall + Vector2Int.right))
                {
                    int fillSpaceValue = Random.Range(0, 3);
                    if (fillSpaceValue > 1)
                        FillSpace(randomWall + Vector2Int.up);
                    if (fillSpaceValue % 2 == 0)
                        FillSpace(randomWall + Vector2Int.down);
                    continue;
                }

                if (visited.Contains(randomWall + Vector2Int.left))
                {
                    AddVisited(randomWall + Vector2Int.right, ref minimumCount);
                    end = randomWall + Vector2Int.right;
                }
                else
                {
                    AddVisited(randomWall + Vector2Int.left, ref minimumCount);
                    end = randomWall + Vector2Int.left;
                }
            }
            else if (canLoop || (visited.Contains(randomWall + Vector2Int.up) ^ visited.Contains(randomWall + Vector2Int.down)))
            {
                if (randomWall + Vector2Int.up == end || randomWall + Vector2Int.down == end)
                    continue;

                cells[randomWall.y * gridSize.x + randomWall.x] = Color.white;
                if (visited.Contains(randomWall + Vector2Int.up) && visited.Contains(randomWall + Vector2Int.down))
                {
                    int fillSpaceValue = Random.Range(0, 3);
                    if (fillSpaceValue > 1)
                        FillSpace(randomWall + Vector2Int.left);
                    if (fillSpaceValue % 2 == 0)
                        FillSpace(randomWall + Vector2Int.right);
                    continue;
                }

                if (visited.Contains(randomWall + Vector2Int.up))
                {
                    AddVisited(randomWall + Vector2Int.down, ref minimumCount);
                    end = randomWall + Vector2Int.down;
                }
                else
                {
                    AddVisited(randomWall + Vector2Int.up, ref minimumCount);
                    end = randomWall + Vector2Int.up;
                 }
            }
        }

        cells[end.y * gridSize.x + end.x] = Color.red + Color.green;

        return cells;
    }

    private void AddVisited(Vector2Int newCell, ref int visitedCellsLeft)
    {
        cells[newCell.y * gridSize.x + newCell.x] = Color.white;
        visited.Add(newCell);
        AddWalls(newCell);
        visitedCellsLeft--;
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

    private void FillSpace(Vector2Int emptySpace)
    {
        if (emptySpace.x < 0 || emptySpace.y < 0 || emptySpace.x >= gridSize.x || emptySpace.y >= gridSize.y) return;
        if (cells[emptySpace.y * gridSize.x + emptySpace.x] == Color.white)
        if (emptySpace.x > 0 && cells[emptySpace.y * gridSize.x + emptySpace.x - 1] != Color.white) return;
        if (emptySpace.x < gridSize.x-1 && cells[emptySpace.y * gridSize.x + emptySpace.x + 1] != Color.white) return;
        if (emptySpace.y > 0 && cells[(emptySpace.y - 1) * gridSize.x + emptySpace.x] != Color.white) return;
        if (emptySpace.y < gridSize.y - 1 && cells[(emptySpace.y + 1) * gridSize.x + emptySpace.x] != Color.white) return;

        cells[emptySpace.y * gridSize.x + emptySpace.x] = Color.white;
    }
}
