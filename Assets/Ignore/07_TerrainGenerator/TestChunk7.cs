using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TestChunk7 : MonoBehaviour
{
    public TestTerrainGenerator terrainGenerator;

    private void Start()
    {
        terrainGenerator.Initialize();

        for (int x = 0; x < terrainGenerator.ChunkResolution; x++)
        {
            for (int y = 0; y < terrainGenerator.ChunkResolution; y++)
            {
                for (int z = 0; z < terrainGenerator.ChunkResolution; z++)
                {
                    if (IsSolid(x, y, z))
                    {
                        MeshVoxel(x, y, z);
                    }
                }
            }
        }

        terrainGenerator.Complete();
    }

    private void MeshVoxel(int x, int y, int z)
    {
        Vector3 offsetPos = new Vector3(x, y, z);

        var voxelType = terrainGenerator.GetVoxelType(x, y, z);

        for (int side = 0; side < 6; side++)
        {
            if (!IsNeighborSolid(x, y, z, side, voxelType))
            {
                try
                {
                    terrainGenerator.Layers[voxelType.layer].MeshQuad(offsetPos, voxelType, side);
                }
                catch
                {
                    Debug.LogError("Voxel type is accessing layer that does not exist and is out of bounds of the layers array.");
                }
            }
        }
    }

    private bool IsNeighborSolid(int x, int y, int z, int side, VoxelType selfVoxelType)
    {
        int3 offset = Tables.NeighborOffsets[side];

        var neighborVoxelType = terrainGenerator.GetVoxelType(x + offset.x, y + offset.y, z + offset.z);

        // if from different layers
        if (neighborVoxelType.layer != selfVoxelType.layer)
        {
            return false; // treat eachother as air
        }

        return IsSolid(x + offset.x, y + offset.y, z + offset.z);
    }

    private bool IsSolid(int x, int y, int z)
    {
        // if outside of the chunk
        if (x < 0 || x >= terrainGenerator.ChunkResolution ||
            y < 0 || y >= terrainGenerator.ChunkResolution ||
            z < 0 || z >= terrainGenerator.ChunkResolution)
        {
            return false; // air
        }

        return terrainGenerator.GetVoxelType(x, y, z).isSolid;
    }
}