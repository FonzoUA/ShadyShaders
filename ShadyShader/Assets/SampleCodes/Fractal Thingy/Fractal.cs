using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    public float childScale;
    public float waitTime;
    public float rotationSpeed;
    public float lilTwist;
    [Range(0.0f, 1.0f)]public float spawnProbability;
    [Range(1, 5)] public int maxDepth;

    private int curDepth;
    private float curRotationSpeed;

    private static Vector3[] childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] childOrientations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f,0f, -90f),
        Quaternion.Euler(0f,0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };

    private void Start()
    {
        this.gameObject.AddComponent<MeshFilter>().mesh = mesh;
        this.gameObject.AddComponent<MeshRenderer>().material = material;

        curRotationSpeed = Random.Range(-rotationSpeed, rotationSpeed);
        transform.Rotate(Random.Range(-lilTwist, lilTwist), 0f, 0f);

        if (curDepth < maxDepth)
            StartCoroutine(CreateChildren());
    }

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(waitTime + Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Baby").AddComponent<Fractal>().InitFractal(this, childDirections[i], childOrientations[i]);
            }
        }
       
    }

    private void InitFractal(Fractal parent, Vector3 direction, Quaternion orientation)
    {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;

        curDepth = parent.curDepth + 1;
        
        spawnProbability = parent.spawnProbability;
        waitTime = parent.waitTime;
        rotationSpeed = parent.rotationSpeed;
        lilTwist = parent.lilTwist; 
        childScale = parent.childScale;

        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = direction * (0.5f + 0.5f * childScale);
        transform.localRotation = orientation;
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

}
