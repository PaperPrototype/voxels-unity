using UnityEngine;
using Unity.Mathematics;

public class TestChunk6 : MonoBehaviour
{
    public int ChunkResolution = 16;

    //                                       by default create at least 1
    public TestVoxelType5[] voxelTypes = new TestVoxelType5[1];
    public TestLayer[] layers = new TestLayer[1];

    FastNoiseLite noise;

    void Start()
    {
        noise = new FastNoiseLite();

        // init mesh layers
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Init(ChunkResolution);
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

    private bool IsNeighborSolid(int x, int y, int z, int side, TestVoxelType5 selfVoxelType)
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
        // if outside of chunk
        if (x < 0 || x >= ChunkResolution ||
            y < 0 || y >= ChunkResolution ||
            z < 0 || z >= ChunkResolution)
        {
            return false; // air
        }

        return GetVoxelType(x, y, z).isSolid;
    }

    private TestVoxelType5 GetVoxelType(int x, int y, int z)
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
            return new TestVoxelType5();
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