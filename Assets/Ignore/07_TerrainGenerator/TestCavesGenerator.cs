using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCavesGenerator : TestTerrainGenerator
{
    public override VoxelType GetVoxelType(int x, int y, int z)
    {
        float caves = GetNoiseCaves(5f, x, y, z);

        if (caves > 0.3)
        {
            return VoxelTypes[0]; // air
        }

        return VoxelTypes[1]; // dirt
    }
}
