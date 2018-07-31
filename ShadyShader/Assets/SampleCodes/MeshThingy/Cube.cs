using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public int zSize;

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
        mesh.name = "Procedural Cube";

        CreateVerts();
        CreateTris();
    }

    private void CreateVerts()
    {
        int corverVerts = 8;
        int edgeVerts = (xSize + ySize + zSize - 3) * 4;
        int faceVerts = ((xSize - 1) * (ySize - 1) +
                         (xSize - 1) * (zSize - 1) +
                         (ySize - 1) * (zSize - 1)) * 2;

        verts = new Vector3[corverVerts + edgeVerts + faceVerts];

        int v = 0;
        // Bott left as a ancor point

        // go layer by layer
        for (int y = 0; y <= ySize; y++)
        {
            // Front face
            for (int x = 0; x <= xSize; x++)
                verts[v++] = AddToPos(x, y, 0);
            // Right face 
            for (int z = 1; z <= zSize; z++)
                verts[v++] = AddToPos(xSize, y, z);
            // Back face
            for (int x = xSize - 1; x >= 0; x--)
                verts[v++] = AddToPos(x, y, zSize);
            // Left face
            for (int z = zSize - 1; z > 0; z--)
                verts[v++] = AddToPos(0, y, z);
        }


        // fill the top cap
        for (int z = 1; z < zSize; z++)
            for (int x = 1; x < xSize; x++)
                verts[v++] = AddToPos(x, ySize, z);

        // fill the bott cap
        for (int z = 1; z < zSize; z++)
            for (int x = 1; x < xSize; x++)
                verts[v++] = AddToPos(x, 0, z);

        mesh.vertices = verts;
    }

    private void CreateTris()
    {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] trigs = new int[quads * 6];   

        int ring = (xSize + zSize) * 2;
        int t = 0;
        int v = 0;

        for (int q = 0; q < ring; q++, v++)
        {
            t = SetQuad(trigs, t, v, v + 1, v + ring, v + ring + 1);
        }


        mesh.triangles = trigs;
    }

    private Vector3 AddToPos(int x, int y, int z)
    {
        return new Vector3(this.transform.position.x + x, this.transform.position.y + y, this.transform.position.z + z);
    }

    private static int SetQuad(int[] trigs, int i, int bl, int br, int tl, int tr)
    {
        // Clock-wise winding
        // bl - bottleft, tl - topleft, etc...
        /*  tl -------- tr
         *  |  \        |
         *  |      \    |
         *  |          \|
         *  bl----------br
         */
        trigs[i] = bl;

        trigs[i + 4] = tl;
        trigs[i + 1] = trigs[i + 4];

        trigs[i + 3] = br;
        trigs[i + 2] = trigs[i + 3];

        trigs[i + 5] = tr;

        return i + 6;
    }
    

    private void OnDrawGizmos()
    {
        if (verts == null)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < verts.Length; i++)
            Gizmos.DrawSphere(verts[i], 0.1f);
    }


}
