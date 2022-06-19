using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

public class ChunkChallenge : MonoBehaviour
{
    // size of the chunk
    public int ChunkResolution = 16;

    // voxel types and layers
    public VoxelType[] voxelTypes;
    public Layer[] layers;

    // noise
    private FastNoiseLite noise;

    private void Start()
    {
        noise = new FastNoiseLite();

        // init mesh layers
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Initialize(ChunkResolution);
        }

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

        // complete and set mesh
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Complete();
        }
    }

    private void MeshVoxel(int x, int y, int z)
    {
        Vector3 offsetPos = new Vector3(x, y, z);

        var voxelType = GetVoxelType(x, y, z);

        for (int side = 0; side < 6; side++)
        {
            if (!IsNeighborSolid(x, y, z, side, voxelType))
            {
                try
                {
                    layers[voxelType.layer].MeshQuad(offsetPos, voxelType, side);
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

        var neighborVoxelType = GetVoxelType(x + offset.x, y + offset.y, z + offset.z);

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
        if (x < 0 || x >= ChunkResolution ||
            y < 0 || y >= ChunkResolution ||
            z < 0 || z >= ChunkResolution)
        {
            return false; // air
        }

        return GetVoxelType(x, y, z).isSolid;
    }

    private VoxelType GetVoxelType(int x, int y, int z)
    {
        //float terrainHeight = GetNoiseHeight(8, 1, x, z);

        //float caves = GetNoise(1f, x, y, z);

        try
        {
            return voxelTypes[1]; // dirt
        }
        catch
        {
            Debug.LogError("That voxel type does not exist. You are offsetting outside of the voxelTypes array.");

            // give back new VoxelType (will use defaults we set in the VoxelType class)
            return new VoxelType();
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
                                             * simplifies ===> floorHeight to maxHeight 
                                             */
    }

    // get noise for caves and other stuff
    private float GetNoise(float amplitude, float x, float y, float z)
    {
        return noise.GetNoise(x * amplitude, y * amplitude, z * amplitude);
    }
}