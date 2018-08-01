using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public int zSize;
    public int roundness;

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
        mesh.name = "Procedural Cube";

        CreateVerts();
        CreateTris();
        CreateColliders();
    }

    private void CreateVerts()
    {
        int corverVerts = 8;
        int edgeVerts = (xSize + ySize + zSize - 3) * 4;
        int faceVerts = ((xSize - 1) * (ySize - 1) +
                         (xSize - 1) * (zSize - 1) +
                         (ySize - 1) * (zSize - 1)) * 2;

        verts = new Vector3[corverVerts + edgeVerts + faceVerts];
        normals = new Vector3[verts.Length];
        cubeUV = new Color32[verts.Length];
       

        int v = 0;
        // Bott left as a ancor point

        // go layer by layer
        for (int y = 0; y <= ySize; y++)
        {
            // Front face
            for (int x = 0; x <= xSize; x++)
                SetVertex(v++, x, y, 0);
            // Right face 
            for (int z = 1; z <= zSize; z++)
                SetVertex(v++, xSize, y, z);
            // Back face
            for (int x = xSize - 1; x >= 0; x--)
                SetVertex(v++, x, y, zSize);
            // Left face
            for (int z = zSize - 1; z > 0; z--)
                SetVertex(v++, 0, y, z);
        }


        // fill the top cap
        for (int z = 1; z < zSize; z++)
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, ySize, z);

        // fill the bott cap
        for (int z = 1; z < zSize; z++)
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, 0, z);

        mesh.vertices = verts;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    private void CreateTris()
    {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;

        // Sub-mesh this boy
        int[] trigZ = new int[(xSize * ySize) * 12];
        int[] trigX = new int[(ySize * zSize) * 12];
        int[] trigY = new int[(xSize * zSize) * 12];

        int ring = (xSize + zSize) * 2;
        int tZ = 0;
        int tX = 0;
        int tY = 0;
        int v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trigZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize; q++, v++)
            {
                tX = SetQuad(trigX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trigZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize - 1; q++, v++)
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
        AddBoxCollider(xSize, ySize - roundness * 2, zSize - roundness * 2);
        AddBoxCollider(xSize - roundness * 2, ySize, zSize - roundness * 2);
        AddBoxCollider(xSize - roundness * 2, ySize - roundness * 2, zSize);

        Vector3 min = Vector3.one * roundness;
        Vector3 half = new Vector3(xSize, ySize, zSize) * 0.5f;
        Vector3 max = new Vector3(xSize, ySize, zSize) - min;

        AddCapsuleCollider(0, half.x, min.y, min.z);
        AddCapsuleCollider(0, half.x, min.y, max.z);
        AddCapsuleCollider(0, half.x, max.y, min.z);
        AddCapsuleCollider(0, half.x, max.y, max.z);

        AddCapsuleCollider(1, min.x, half.y, min.z);
        AddCapsuleCollider(1, min.x, half.y, max.z);
        AddCapsuleCollider(1, max.x, half.y, min.z);
        AddCapsuleCollider(1, max.x, half.y, max.z);

        AddCapsuleCollider(2, min.x, min.y, half.z);
        AddCapsuleCollider(2, min.x, max.y, half.z);
        AddCapsuleCollider(2, max.x, min.y, half.z);
        AddCapsuleCollider(2, max.x, max.y, half.z);
    }
    private void AddBoxCollider(float x, float y, float z)
    {
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        c.size = new Vector3(x, y, z);
    }
    private void AddCapsuleCollider(int dir, float x, float y, float z)
    {
        CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
        c.center = new Vector3(x, y, z);
        c.direction = dir;
        c.radius = roundness;
        c.height = c.center[dir] * 2f;
        
    }

    private int CreateTopFace(int[] trigs, int t, int ring)
    {
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            t = SetQuad(trigs, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(trigs, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(trigs, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    trigs, t,
                    vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }
            t = SetQuad(trigs, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(trigs, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(trigs, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(trigs, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] trigs, int t, int ring)
    {
        int v = 1;
        int vMid = verts.Length - (xSize - 1) * (zSize - 1);
        t = SetQuad(trigs, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(trigs, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(trigs, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(trigs, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    trigs, t,
                    vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            }
            t = SetQuad(trigs, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(trigs, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
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
        verts[i] = new Vector3(x, y, z);
        Vector3 inner = verts[i];

        if (x < roundness)
            inner.x = roundness;
        else if (x > xSize - roundness)
            inner.x = xSize - roundness;
        if (y < roundness)
            inner.y = roundness;
        else if (y > ySize - roundness)
            inner.y = ySize - roundness;
        if (z < roundness)
            inner.z = roundness;
        else if (z > zSize - roundness)
            inner.z = zSize - roundness;


        normals[i] = (verts[i] - inner).normalized;
        verts[i] = inner + normals[i] * roundness;
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
