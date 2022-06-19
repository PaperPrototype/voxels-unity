using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainGenerator : MonoBehaviour
{
    public int ChunkResolution = 16;
    public int Seed = 1337;

    public VoxelType[] VoxelTypes;
    public Layer[] Layers;

    private FastNoiseLite noise;

    public abstract VoxelType GetVoxelType(int x, int y, int z);

    public void Initialize()
    {
        noise = new FastNoiseLite(Seed);

        // init mesh layers
        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i].Initialize(ChunkResolution);
        }

    }

    public void Complete()
    {
        // complete and set mesh
        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i].Complete();
        }
    }

    // get noise height in range of floorHeight to maxHeight
    public float GetNoiseHeight(float maxHeight, float floorHeight, float x, float z)
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

    // gives numbers in range of 0 to 1
    public float GetNoiseCaves(float scale, int x, int y, int z)
    {
        return (noise.GetNoise(x * scale, y * scale, z * scale) + 1) / 2;
    }

}
