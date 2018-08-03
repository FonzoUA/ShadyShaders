using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;
    public int gridResolution = 10;

    private Transform[] grid;
    private Matrix4x4 transformationMatrix;
    private List<Transformation> transformations;

    #region UnityFunctions
    private void Awake()
    {
        grid = new Transform[gridResolution * gridResolution * gridResolution];

        for (int i = 0, z = 0; z < gridResolution; z++)
            for (int y = 0; y < gridResolution; y++)
                for (int x = 0; x < gridResolution; x++, i++)
                    grid[i] = CreateGridPoint(x, y, z);

        transformations = new List<Transformation>();
    }
    private void Update()
    {
        UpdateTransformation();

        for (int i = 0, z = 0; z < gridResolution; z++)
            for (int y = 0; y < gridResolution; y++)
                for (int x = 0; x < gridResolution; x++, i++)
                    grid[i].localPosition = TransformPoint(x, y, z);
    }
    #endregion

    private void UpdateTransformation()
    {
        // Returns all components of Type type in the GameObject into List results.
        GetComponents<Transformation>(transformations);

        if (transformations.Count > 0)
        {
            transformationMatrix = transformations[0].Matrix;
            for (int i = 1; i < transformations.Count; i++)
            {
                transformationMatrix = transformations[i].Matrix * transformationMatrix;
            }
        }
    }

    private Vector3 TransformPoint (int x, int y, int z)
    {
        // Get the original point coord and apply each transformation
        Vector3 coords = GetCoordinates(x, y, z);
        return transformationMatrix.MultiplyPoint(coords);
    }

    private Transform CreateGridPoint (int x, int y, int z)
    {
        Transform point = Instantiate<Transform>(prefab);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution, 
            (float)y / gridResolution, 
            (float)z / gridResolution);
        return point;
    }
    private Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3(
            x - (gridResolution - 1) * 0.5f, // adjust the position so that the origin is in the center
            y - (gridResolution - 1) * 0.5f, 
            z - (gridResolution - 1) * 0.5f);
    }

}



