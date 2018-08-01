using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{
    public int gridSize;
    public float radius = 1;

    private Mesh mesh;
    private Vector3[] verts;
    private Vector3[] normals;
    private Color32[] cubeUV;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Procedural Sphere";

        CreateVerts();
        CreateTris();
        CreateColliders();
    }

    private void CreateVerts()
    {
        int corverVerts = 8;
        int edgeVerts = (gridSize + gridSize + gridSize - 3) * 4;
        int faceVerts = ((gridSize - 1) * (gridSize - 1) +
                         (gridSize - 1) * (gridSize - 1) +
                         (gridSize - 1) * (gridSize - 1)) * 2;

        verts = new Vector3[corverVerts + edgeVerts + faceVerts];
        normals = new Vector3[verts.Length];
        cubeUV = new Color32[verts.Length];
       

        int v = 0;
        // Bott left as a ancor point

        // go layer by layer
        for (int y = 0; y <= gridSize; y++)
        {
            // Front face
            for (int x = 0; x <= gridSize; x++)
                SetVertex(v++, x, y, 0);
            // Right face 
            for (int z = 1; z <= gridSize; z++)
                SetVertex(v++, gridSize, y, z);
            // Back face
            for (int x = gridSize - 1; x >= 0; x--)
                SetVertex(v++, x, y, gridSize);
            // Left face
            for (int z = gridSize - 1; z > 0; z--)
                SetVertex(v++, 0, y, z);
        }


        // fill the top cap
        for (int z = 1; z < gridSize; z++)
            for (int x = 1; x < gridSize; x++)
                SetVertex(v++, x, gridSize, z);

        // fill the bott cap
        for (int z = 1; z < gridSize; z++)
            for (int x = 1; x < gridSize; x++)
                SetVertex(v++, x, 0, z);

        mesh.vertices = verts;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    private void CreateTris()
    {
        int quads = (gridSize * gridSize + gridSize * gridSize + gridSize * gridSize) * 2;

        // Sub-mesh this boy
        int[] trigZ = new int[(gridSize * gridSize) * 12];
        int[] trigX = new int[(gridSize * gridSize) * 12];
        int[] trigY = new int[(gridSize * gridSize) * 12];

        int ring = (gridSize + gridSize) * 2;
        int tZ = 0;
        int tX = 0;
        int tY = 0;
        int v = 0;

        for (int y = 0; y < gridSize; y++, v++)
        {
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trigZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize; q++, v++)
            {
                tX = SetQuad(trigX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trigZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize - 1; q++, v++)
            {
                tX = SetQuad(trigX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = SetQuad(trigX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateTopFace(trigY, tY, ring);
        tY = CreateBottomFace(trigY, tY, ring);

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trigZ, 0);
        mesh.SetTriangles(trigX, 1);
        mesh.SetTriangles(trigY, 2);
    }

    private void CreateColliders()
    {
        gameObject.AddComponent<SphereCollider>();
    }

    private int CreateTopFace(int[] trigs, int t, int ring)
    {
        int v = ring * gridSize;
        for (int x = 0; x < gridSize - 1; x++, v++)
        {
            t = SetQuad(trigs, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(trigs, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (gridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(trigs, t, vMin, vMid, vMin - 1, vMid + gridSize - 1);
            for (int x = 1; x < gridSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    trigs, t,
                    vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);
            }
            t = SetQuad(trigs, t, vMid, vMax, vMid + gridSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(trigs, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(trigs, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(trigs, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] trigs, int t, int ring)
    {
        int v = 1;
        int vMid = verts.Length - (gridSize - 1) * (gridSize - 1);
        t = SetQuad(trigs, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < gridSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(trigs, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(trigs, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= gridSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(trigs, t, vMin, vMid + gridSize - 1, vMin + 1, vMid);
            for (int x = 1; x < gridSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    trigs, t,
                    vMid + gridSize - 1, vMid + gridSize, vMid, vMid + 1);
            }
            t = SetQuad(trigs, t, vMid + gridSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(trigs, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(trigs, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(trigs, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
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

    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;
        normals[i] = v.normalized;
        verts[i] = normals[i] * radius;
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    //private void OnDrawGizmos()
    //{
    //    if (verts == null)
    //        return;
        
    //    for (int i = 0; i < verts.Length; i++)
    //    {
    //        Gizmos.color = Color.black;
    //        Gizmos.DrawSphere(verts[i], 0.1f);
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawRay(verts[i], normals[i]);
    //    }
    //}


}
