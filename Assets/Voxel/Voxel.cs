using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Voxel : MonoBehaviour
{
    void Start()
    {
        // number of quads in a voxel
        const int quadsNum = 6;

        // vertices and triangles for a single voxel
        var vertices = new Vector3[4 * quadsNum];
        var triangles = new int[6 * quadsNum];
        var uvs = new Vector2[4 * quadsNum];

        int vertexOffset = 0;
        int triangleOffset = 0;

        // for each side of the voxel
        for (int side = 0; side < quadsNum; side++)
        {
            // create mesh for quad on this side of the voxel

            // offset using currentVertex
            vertices[vertexOffset + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]];
            vertices[vertexOffset + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]];
            vertices[vertexOffset + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]];
            vertices[vertexOffset + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]];

            // 0 1 2 2 1 3 <- triangle numbers
            triangles[triangleOffset + 0] = vertexOffset + 0;
            triangles[triangleOffset + 1] = vertexOffset + 1;
            triangles[triangleOffset + 2] = vertexOffset + 2;
            triangles[triangleOffset + 3] = vertexOffset + 2;
            triangles[triangleOffset + 4] = vertexOffset + 1;
            triangles[triangleOffset + 5] = vertexOffset + 3;

            uvs[vertexOffset + 0] = new Vector2(0, 0);
            uvs[vertexOffset + 1] = new Vector2(0, 1);
            uvs[vertexOffset + 2] = new Vector2(1, 0);
            uvs[vertexOffset + 3] = new Vector2(1, 1);

            triangleOffset += 6;
            vertexOffset += 4;
        }

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }
}
