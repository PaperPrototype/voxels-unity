using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TestChunk1 : MonoBehaviour
{
    public int ChunkResolution = 16;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private int vertexOffset = 0;
    private int triangleOffset = 0;

    void Start()
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
                    if (IsSolid(x, y, z))
                    {
                        MeshVoxel(x, y, z);
                    }
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
        Vector3 offsetPos = new Vector3(x, y, z);

        for (int side = 0; side < 6; side++)
        {
            if (!IsNeighborSolid(x, y, z, side))
            {
                vertices[vertexOffset + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]] + offsetPos;
                vertices[vertexOffset + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]] + offsetPos;
                vertices[vertexOffset + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]] + offsetPos;
                vertices[vertexOffset + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]] + offsetPos;

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

    private bool IsNeighborSolid(int x, int y, int z, int side)
    {
        int3 gridPosition = new int3(x, y, z) + Tables.NeighborOffsets[side];

        return IsSolid(gridPosition.x, gridPosition.y, gridPosition.z);
    }
    
    private bool IsSolid(int x, int y, int z)
    {
        if (y > 8)
        {
            return false;
        }

        return true;
    }
}
