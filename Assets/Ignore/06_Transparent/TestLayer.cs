using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TestLayer
{
    public string name = "default";
    public bool useCollider = true;
    public Material material;
    public int atlasSize = 1; // x and y atlas size of texture

    // we add serialize field so we can debug... and feel satisfied :)
    [SerializeField] private GameObject gameObject;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;

    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;
    [SerializeField] private Vector2[] uvs;

    [SerializeField] private int vertexOffset = 0;
    [SerializeField] private int triangleOffset = 0;

    public void Init(int chunkResolution)
    {
        // intialize gameObject for this layer
        gameObject = new GameObject();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        if (useCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        // initialize arrays
        vertices = new Vector3[24 * chunkResolution * chunkResolution * chunkResolution];
        triangles = new int[36 * chunkResolution * chunkResolution * chunkResolution];
        uvs = new Vector2[24 * chunkResolution * chunkResolution * chunkResolution];
    }

    // mesh quad on specific side of voxel
    public void MeshQuad(Vector3 offsetPos, TestVoxelType5 voxelType, int side)
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

        // apply offset and add uv corner offsets
        // then divide by the atlas size to get a smaller section (AKA our individual texture)
        uvs[vertexOffset + 0] = (voxelType.uvOffset + new Vector2(0, 0)) / atlasSize;
        uvs[vertexOffset + 1] = (voxelType.uvOffset + new Vector2(0, 1)) / atlasSize;
        uvs[vertexOffset + 2] = (voxelType.uvOffset + new Vector2(1, 0)) / atlasSize;
        uvs[vertexOffset + 3] = (voxelType.uvOffset + new Vector2(1, 1)) / atlasSize;

        triangleOffset += 6;
        vertexOffset += 4;
    }
    
    public void Complete()
    {
        // instantiate new mesh and set arrays
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // set the mesh to the mesh filter
        meshFilter.mesh = mesh;

        // if we want collision
        if (useCollider)
        {
            meshCollider.sharedMesh = mesh;
        }
    }
}
