using UnityEngine;
using System;

[Serializable]
public class VoxelType
{
    public string name = "default";
    public bool isSolid = false;
    public Vector2 atlasOffset = Vector2.zero;
    public int layer = 0; // add this
}