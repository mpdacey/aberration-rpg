using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static event Action<Vector2> GoalLocationFound;
    public static event Action FinishedLevelGeneration;

    public Texture2D levelGeometry;
    public Material groundMaterial;
    public Material wallsMaterial;
    public MeshRenderer wallMeshRenderer;
    public MeshFilter wallMeshFilter;
    public float floorHeight = -1;
    public float wallHeight = 10;
    private const float TILE_RADIUS = 2.5f;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Vector2[] uvs = new Vector2[4]
    {
        Vector2.zero,
        Vector2.right,
        Vector2.up,
        Vector2.one
    };

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = groundMaterial;
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        wallMeshRenderer.sharedMaterial = wallsMaterial;
    }

    private void OnEnable()
    {
        MazeGenerator.MazeTextureGenerated += GenerateTerrain;
    }

    private void OnDisable()
    {
        MazeGenerator.MazeTextureGenerated -= GenerateTerrain;
    }

    private void GenerateTerrain(Texture2D levelTexture)
    {
        levelGeometry = levelTexture;

        Color[] pixels = levelGeometry.GetPixels();
        List<Vector2> walkableTiles = new List<Vector2>();
        List<Vector2[]> edges = new List<Vector2[]>();

        Vector2 startLocation = Vector2.zero;
        Vector2 endLocation = Vector2.zero;

        for(int y = 0; y < levelGeometry.height; y++)
        {
            for(int x = 0; x < levelGeometry.width; x++)
            {
                int index = y * levelGeometry.width + x;
                if (pixels[index].r == 1)
                {
                    Vector2 pixelCoords = new Vector2(x, y);
                    walkableTiles.Add(pixelCoords);
                    if (pixels[index] == Color.red)
                    {
                        startLocation = pixelCoords;
                    }
                    if(pixels[index].g == 1 && pixels[index].b == 0)
                    {
                        endLocation = pixelCoords;
                    }

                    edges.AddRange(FindEdges(pixels, pixelCoords));
                }
            }
        }

        if (GoalLocationFound != null)
            GoalLocationFound.Invoke(endLocation-startLocation);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r == 1)
            {
                walkableTiles.Add(new Vector2(i % levelGeometry.width, i / levelGeometry.height));
                if (pixels[i] == Color.red)
                {
                    startLocation = new Vector2(i % levelGeometry.width, i / levelGeometry.height);
                }
            }
        }

        GenerateFlooring(walkableTiles, startLocation);
        GenerateWalls(edges, startLocation);

        if (FinishedLevelGeneration != null)
            FinishedLevelGeneration.Invoke();
    }

    private List<Vector2[]> FindEdges(Color[] pixels, Vector2 pixelCoords)
    {
        List<Vector2[]> localEdges = new List<Vector2[]>();

        if(pixelCoords.x == 0 || pixels[(int)pixelCoords.y*levelGeometry.width + (int)pixelCoords.x -1].r != 1f)
        {
            localEdges.Add(new Vector2[]{
                    new Vector2(pixelCoords.x-0.5f, pixelCoords.y - 0.5f),
                    new Vector2(pixelCoords.x-0.5f, pixelCoords.y + 0.5f)
                });
        }
        if(pixelCoords.y == 0 || pixels[(int)(pixelCoords.y-1) * levelGeometry.width + (int)pixelCoords.x].r != 1f)
        {
            localEdges.Add(new Vector2[]{
                    new Vector2(pixelCoords.x-0.5f, pixelCoords.y - 0.5f),
                    new Vector2(pixelCoords.x+0.5f, pixelCoords.y - 0.5f)
                });
        }
        if(pixelCoords.x == levelGeometry.width-1 || pixels[(int)pixelCoords.y * levelGeometry.width + (int)pixelCoords.x+1].r != 1f)
        {
            localEdges.Add(new Vector2[]{
                    new Vector2(pixelCoords.x+0.5f, pixelCoords.y + 0.5f),
                    new Vector2(pixelCoords.x+0.5f, pixelCoords.y - 0.5f)
                });
        }
        if(pixelCoords.y == levelGeometry.height-1 || pixels[(int)(pixelCoords.y + 1) * levelGeometry.width + (int)pixelCoords.x].r != 1f)
        {
            localEdges.Add(new Vector2[]{
                    new Vector2(pixelCoords.x+0.5f, pixelCoords.y + 0.5f),
                    new Vector2(pixelCoords.x-0.5f, pixelCoords.y + 0.5f)
                });
        }

        return localEdges;
    }

    private void GenerateFlooring(List<Vector2> floorCoords, Vector2 startLocation)
    {
        Vector3[] vertices = new Vector3[floorCoords.Count * 4];
        int[] tris = new int[floorCoords.Count * 6];
        Vector3[] normals = new Vector3[floorCoords.Count * 4];
        Vector2[] uv = new Vector2[floorCoords.Count * 4];

        Mesh mesh = new Mesh();

        for (int i = 0; i < floorCoords.Count; i++)
        {
            vertices[i * 4] = new Vector3((floorCoords[i].x - startLocation.x)*2*TILE_RADIUS - TILE_RADIUS, floorHeight, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS - TILE_RADIUS);
            vertices[i * 4 + 1] = new Vector3((floorCoords[i].x - startLocation.x) * 2 * TILE_RADIUS + TILE_RADIUS, floorHeight, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS - TILE_RADIUS);
            vertices[i * 4 + 2] = new Vector3((floorCoords[i].x - startLocation.x) * 2 * TILE_RADIUS - TILE_RADIUS, floorHeight, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS + TILE_RADIUS);
            vertices[i * 4 + 3] = new Vector3((floorCoords[i].x - startLocation.x) * 2 * TILE_RADIUS + TILE_RADIUS, floorHeight, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS + TILE_RADIUS);

            tris[i * 6] = i * 4;
            tris[i * 6 + 1] = i * 4 + 2;
            tris[i * 6 + 2] = i * 4 + 1;
            tris[i * 6 + 3] = i * 4 + 2;
            tris[i * 6 + 4] = i * 4 + 3;
            tris[i * 6 + 5] = i * 4 + 1;

            normals[i * 4] = Vector3.back;
            normals[i * 4 + 1] = Vector3.back;
            normals[i * 4 + 2] = Vector3.back;
            normals[i * 4 + 3] = Vector3.back;

            uv[i * 4] = Vector2.zero;
            uv[i * 4 + 1] = Vector2.right;
            uv[i * 4 + 2] = Vector2.up;
            uv[i * 4 + 3] = Vector2.one;
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private void GenerateWalls(List<Vector2[]> edges, Vector2 startLocation)
    {
        Vector3[] vertices = new Vector3[edges.Count * 4* 2];
        int[] tris = new int[edges.Count * 6 * 2];
        Vector3[] normals = new Vector3[edges.Count * 4 * 2];
        Vector2[] uv = new Vector2[edges.Count * 4 * 2];

        Mesh mesh = new Mesh();

        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i][0].x == edges[i][1].x)
            {
                vertices[i * 4 * 2 + 1] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 2] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 3] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);

                uv[i * 4 * 2] = Vector2.one;
                uv[i * 4 * 2 + 1] = Vector2.right;
                uv[i * 4 * 2 + 2] = Vector2.up;
                uv[i * 4 * 2 + 3] = Vector2.zero;
            }
            else
            {
                vertices[i * 4 * 2] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 1] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 3] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 2] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);

                uv[i * 4 * 2] = Vector2.zero;
                uv[i * 4 * 2 + 1] = Vector2.up;
                uv[i * 4 * 2 + 2] = Vector2.right;
                uv[i * 4 * 2 + 3] = Vector2.one;
            }

            tris[i * 6 * 2] = i * 4 * 2;
            tris[i * 6 * 2 + 1] = i * 4 * 2 + 2;
            tris[i * 6 * 2 + 2] = i * 4 * 2 + 1;
            tris[i * 6 * 2 + 3] = i * 4 * 2 + 2;
            tris[i * 6 * 2 + 4] = i * 4 * 2 + 3;
            tris[i * 6 * 2 + 5] = i * 4 * 2 + 1;

            normals[i * 4 * 2] = Vector3.back;
            normals[i * 4 * 2 + 1] = Vector3.back;
            normals[i * 4 * 2 + 2] = Vector3.back;
            normals[i * 4 * 2 + 3] = Vector3.back;

            if (edges[i][0].x == edges[i][1].x)
            {
                vertices[i * 4 * 2 + 4 + 1] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 4] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 4 + 2] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 4 + 3] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);

                uv[i * 4 * 2 + 4] = Vector2.one;
                uv[i * 4 * 2 + 1 + 4] = Vector2.right;
                uv[i * 4 * 2 + 2 + 4] = Vector2.up;
                uv[i * 4 * 2 + 3 + 4] = Vector2.zero;
            }
            else
            {
                vertices[i * 4 * 2 + 4] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 4 + 1] = new Vector3((edges[i][0].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][0].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 4 + 3] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, wallHeight + floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);
                vertices[i * 4 * 2 + 4 + 2] = new Vector3((edges[i][1].x - startLocation.x) * 2 * TILE_RADIUS, floorHeight, (edges[i][1].y - startLocation.y) * 2 * TILE_RADIUS);

                uv[i * 4 * 2 + 4] = Vector2.zero;
                uv[i * 4 * 2 + 1 + 4] = Vector2.up;
                uv[i * 4 * 2 + 2 + 4] = Vector2.right;
                uv[i * 4 * 2 + 3 + 4] = Vector2.one;
            }

            tris[i * 6 * 2 + 6] = i * 4 * 2 + 4;
            tris[i * 6 * 2 + 6 + 1] = i * 4 * 2 + 4 + 1;
            tris[i * 6 * 2 + 6 + 2] = i * 4 * 2 + 4 + 2;
            tris[i * 6 * 2 + 6 + 3] = i * 4 * 2 + 4 + 2;
            tris[i * 6 * 2 + 6 + 4] = i * 4 * 2 + 4 + 1;
            tris[i * 6 * 2 + 6 + 5] = i * 4 * 2 + 4 + 3;

            normals[i * 4 * 2 + 4] = Vector3.forward;
            normals[i * 4 * 2 + 4 + 1] = Vector3.forward;
            normals[i * 4 * 2 + 4 + 2] = Vector3.forward;
            normals[i * 4 * 2 + 4 + 3] = Vector3.forward;
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        wallMeshFilter.mesh = mesh;
    }
}
