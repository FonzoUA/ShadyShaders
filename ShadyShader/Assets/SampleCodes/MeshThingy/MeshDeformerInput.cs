using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f;
    [Range(0.0f, 1.0f)] public float forceOffset = 0.1f;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                // Where the ray hit the object
                Vector3 point = hit.point;
                // Move the "force" point slightly away from impact point
                point += hit.normal * forceOffset;
                deformer.AddDeformingForce(point, force);
            }
        }
    }
}
