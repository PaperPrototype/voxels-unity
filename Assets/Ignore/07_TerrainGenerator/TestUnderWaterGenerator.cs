using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnderWaterGenerator : TestTerrainGenerator
{
    public override VoxelType GetVoxelType(int x, int y, int z)
    {
        float terrainHeight = GetNoiseHeight(16, 1, x, z);
        float caves = GetNoiseCaves(1f, x, y, z);

        try
        {
            // if under ground
            if (y < (terrainHeight + 1))
            {
                // under water caves
                if (caves > 0.75 && y < 10)
                {
                    return VoxelTypes[2]; // water
                }

                if (y < terrainHeight)
                {
                    if (y < (terrainHeight - 4))
                    {
                        return VoxelTypes[4]; // rock
                    }

                    return VoxelTypes[1]; // dirt
                }

                if (y > 9)
                {
                    return VoxelTypes[5]; // grass
                }

                return VoxelTypes[3]; // sand
            }

            if (y < 10)
            {
                return VoxelTypes[2]; // water
            }

            return VoxelTypes[0]; // air
        }
        catch
        {
            Debug.LogError("That voxel type does not exist. You are offsetting outside of the voxelTypes array.");

            // give back new VoxelType (will use defaults we set in the VoxelType class)
            return new VoxelType();
        }
    }
}
