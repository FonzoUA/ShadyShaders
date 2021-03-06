﻿using System.Collections;
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

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(trigs, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(trigs, t, v, v - ring + 1, v + ring, v + 1);
        }

        t = CreateTopFace(trigs, t, ring);
        t = CreateBottomFace(trigs, t, ring);

        mesh.triangles = trigs;
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

    private Vector3 AddToPos(int x, int y, int z)
    {
        return new Vector3(x, y, z);
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
