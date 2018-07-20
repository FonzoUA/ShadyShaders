using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize;
    public int ySize;

    private Mesh mesh;
    private Vector3[] verts;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "ProceduralGrid";

        verts = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[verts.Length];
        Vector4[] tangents = new Vector4[verts.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        // y = row
        // x = col
        int index = 0;
        for (int row = 0; row <= ySize; row++)
        {
            for (int col = 0; col <= xSize; col++)
            {
                verts[index] = new Vector3(col, row);
                uv[index] = new Vector2((float)col / xSize, (float)row / ySize);
                tangents[index] = tangent;
                index++;
            }
        }
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    /*
    private void OnDrawGizmos()
    {
        if (verts == null)
            return;

        Gizmos.color = Color.black;
        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
        }
    }
    */
}
