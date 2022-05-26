using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TestChunk : MonoBehaviour
{
    public int ChunkResolution = 16;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private int vertexOffset = 0;
    private int triangleOffset = 0;

    private void Start()
    {
        vertices = new Vector3[24 * ChunkResolution * ChunkResolution * ChunkResolution];
        triangles = new int[36 * ChunkResolution * ChunkResolution * ChunkResolution];
        uvs = new Vector2[24 * ChunkResolution * ChunkResolution * ChunkResolution];

        for (int x = 0; x < ChunkResolution; x++)
        {
            for (int y = 0; y < ChunkResolution; y++)
            {
                for (int z = 0; z < ChunkResolution; z++)
                {
                    MeshVoxel(x, y, z);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    private void MeshVoxel(int x, int y, int z)
    {
        Vector3 voxelPos = new Vector3(x, y, z);

        for (int side = 0; side < 6; side++)
        {
            vertices[vertexOffset + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]] + voxelPos;
            vertices[vertexOffset + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]] + voxelPos;
            vertices[vertexOffset + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]] + voxelPos;
            vertices[vertexOffset + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]] + voxelPos;

            triangles[triangleOffset + 0] = vertexOffset + 0;
            triangles[triangleOffset + 1] = vertexOffset + 1;
            triangles[triangleOffset + 2] = vertexOffset + 2;
            triangles[triangleOffset + 3] = vertexOffset + 2;
            triangles[triangleOffset + 4] = vertexOffset + 1;
            triangles[triangleOffset + 5] = vertexOffset + 3;

            uvs[vertexOffset + 0] = new Vector2(0, 0);
            uvs[vertexOffset + 1] = new Vector2(0, 1);
            uvs[vertexOffset + 2] = new Vector2(1, 0);
            uvs[vertexOffset + 3] = new Vector2(1, 1);

            triangleOffset += 6;
            vertexOffset += 4;
        }
    }
}
