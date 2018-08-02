using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    public float springForce = 20f;
    public float damping = 5f;

    private float uniformScale = 1f;
    Mesh deformingMesh;
    Vector3[] originalVerts;
    Vector3[] displacedVerts;
    Vector3[] vertVelocities;

    private void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;

        originalVerts = deformingMesh.vertices;
        displacedVerts = new Vector3[originalVerts.Length];

        for (int i = 0; i < originalVerts.Length; i++)
            displacedVerts[i] = originalVerts[i];

        vertVelocities = new Vector3[originalVerts.Length];
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        Debug.DrawLine(Camera.main.transform.position, point);

        // world to local space
        point = transform.InverseTransformPoint(point);
        // Add deforming force to each vert individually
        for (int i = 0; i < displacedVerts.Length; i++)
        {
            AddForceToVert(i, point, force);
        }
    }

    private void AddForceToVert(int i, Vector3 point, float force)
    {
        // We need both the direction and the distance of the deforming force per vertex
        Vector3 pointToVert = displacedVerts[i] - point;
        // adjust for scaling
        pointToVert *= uniformScale;
        // Use inverse-square law to find attenuated force aka original force / distance squared
        // Add one so the force is STRONG at the distance zero
        float attenuatedForce = force / (1f + pointToVert.sqrMagnitude);
        // convert this into velocity delta
        // first convert force into acceleration via a = F/m (assuming m = 1)
        // the change in velocity is deltaV = a*deltaTime
        // thus deltaV = F * deltaTime 
        // AKA this
        float velocity = attenuatedForce * Time.deltaTime;
        // find direction by normalizing the vector we started with then add to the vert velocity
        vertVelocities[i] += pointToVert.normalized * velocity;
    }

    private void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displacedVerts.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVerts;
        deformingMesh.RecalculateNormals();
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = vertVelocities[i];
        // adjust vertex position via deltaPosition = velocity * deltaTime;
        displacedVerts[i] += velocity * Time.deltaTime;
        // use displacement vector as velocity adjustment scaled by spring force
        Vector3 displacement = displacedVerts[i] - originalVerts[i];
        // adjust for scaling
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        // damp the deformation oscillation by slowing the vertices down (dampening)
        // math is dampedVelocity = velocity(1 - damping * deltaTime)
        // the higher the dampening, the more sluggish the effect is 
        velocity *= 1f - damping * Time.deltaTime;
        vertVelocities[i] = velocity;
        // adjust vertex movement based on scale
        displacedVerts[i] += velocity * (Time.deltaTime / uniformScale);
    }
}
