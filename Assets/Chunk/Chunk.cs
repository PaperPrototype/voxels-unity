using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

public class Chunk : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    private void Start()
    {
        // initialize layers
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

        // complete and set the mesh for each layer
        terrainGenerator.Complete();
    }

    private void MeshVoxel(int x, int y, int z)
    {
        Vector3 offsetPos = new Vector3(x, y, z);

        // update this
        var voxelType = terrainGenerator.GetVoxelType(x, y, z);

        for (int side = 0; side < 6; side++)
        {
            if (!IsNeighborSolid(x, y, z, side, voxelType))
            {
                try
                {
                    // update this
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

        // update this
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
        // update this
        // if outside of the chunk
        if (x < 0 || x >= terrainGenerator.ChunkResolution ||
            y < 0 || y >= terrainGenerator.ChunkResolution ||
            z < 0 || z >= terrainGenerator.ChunkResolution)
        {
            return false; // air
        }

        // update this
        return terrainGenerator.GetVoxelType(x, y, z).isSolid;
    }
}