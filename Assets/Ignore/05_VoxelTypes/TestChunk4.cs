using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TestChunk4 : MonoBehaviour
{
    public int ChunkResolution = 16;
    public TestVoxelType4[] voxelTypes;
    public int atlasSize = 1; // x and y size

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private int vertexOffset = 0;
    private int triangleOffset = 0;

    FastNoiseLite noise;

    void Start()
    {
        noise = new FastNoiseLite();

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

                // var is a shortcut for the type
                // instead of TestVoxelType4 we can just say `var` and it will figure out we want a TestVoxelType4
                var voxelType = GetVoxelType(x, y, z);

                // apply offset and add uv corner offsets
                // then divide by the atlas size to get a smaller section (AKA our individual texture)
                uvs[vertexOffset + 0] = (voxelType.uvOffset + new Vector2(0, 0)) / atlasSize;
                uvs[vertexOffset + 1] = (voxelType.uvOffset + new Vector2(0, 1)) / atlasSize;
                uvs[vertexOffset + 2] = (voxelType.uvOffset + new Vector2(1, 0)) / atlasSize;
                uvs[vertexOffset + 3] = (voxelType.uvOffset + new Vector2(1, 1)) / atlasSize;

                triangleOffset += 6;
                vertexOffset += 4;
            }
        }
    }

    private bool IsNeighborSolid(int x, int y, int z, int side)
    {
        int3 offset = Tables.NeighborOffsets[side];

        return IsSolid(x + offset.x, y + offset.y, z + offset.z);
    }

    private bool IsSolid(int x, int y, int z)
    {
        // if outside of chunk
        if (x < 0 || x >= ChunkResolution ||
            y < 0 || y >= ChunkResolution ||
            z < 0 || z >= ChunkResolution)
        {
            return false; // air
        }

        return GetVoxelType(x, y, z).isSolid;
    }

    private TestVoxelType4 GetVoxelType(int x, int y, int z)
    {
        float terrainHeight = GetNoiseHeight(8, 1, x, z);

        try
        {
            if (y == 5)
            {
                return voxelTypes[3]; // sand
            }

            if (y < terrainHeight)
            {
                return voxelTypes[1]; // dirt
            }

            if (y < (terrainHeight + 1))
            {
                return voxelTypes[2]; // grass
            }
            else if (y < 8)
            {
                return voxelTypes[4]; // water
            }

            return voxelTypes[0]; // air
        }
        catch
        {
            Debug.LogError("That voxel type does not exist. You are trying to access a voxel type that is outside of the voxelTypes array.");
            return new TestVoxelType4();
        }
    }

    // get noise height in range of floorHeight to maxHeight
    private float GetNoiseHeight(float maxHeight, float floorHeight, float x, float z)
    {
        return // floorHeight to maxHeight
        (
            (
                (
                    noise.GetNoise(x, z)    // range -1           to 1
                    + 1                     // range  0           to 2
                ) / 2                       // range  0           to 1
            ) * (maxHeight - floorHeight)   // range  0           to (maxHeight - floorHeight)
        ) + floorHeight;                    /* range  floorHeight to ((maxHeight - floorHeight) + floorHeight) 
                                             * simplifies to ===> floorHeight to maxHeight 
                                             */
    }
}