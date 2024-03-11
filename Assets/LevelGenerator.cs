using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D levelGeometry;
    public Material ground;
    private const float TILE_RADIUS = 2.5f;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Vector2[] uvs = new Vector2[4]
    {
        Vector2.zero,
        Vector2.right,
        Vector2.up,
        Vector2.one
    };

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        meshFilter = GetComponent<MeshFilter>();
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        Color[] pixels = levelGeometry.GetPixels();
        List<Vector2> walkableTiles = new List<Vector2>();
        Vector2 startLocation = Vector2.zero;

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
            vertices[i * 4] = new Vector3((floorCoords[i].x - startLocation.x)*2*TILE_RADIUS - TILE_RADIUS, -2, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS - TILE_RADIUS);
            vertices[i * 4 + 1] = new Vector3((floorCoords[i].x - startLocation.x) * 2 * TILE_RADIUS + TILE_RADIUS, -2, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS - TILE_RADIUS);
            vertices[i * 4 + 2] = new Vector3((floorCoords[i].x - startLocation.x) * 2 * TILE_RADIUS - TILE_RADIUS, -2, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS + TILE_RADIUS);
            vertices[i * 4 + 3] = new Vector3((floorCoords[i].x - startLocation.x) * 2 * TILE_RADIUS + TILE_RADIUS, -2, (floorCoords[i].y - startLocation.y) * 2 * TILE_RADIUS + TILE_RADIUS);

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

        meshFilter.mesh = mesh;
    }
}
