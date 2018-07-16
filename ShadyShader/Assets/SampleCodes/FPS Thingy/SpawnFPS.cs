using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFPS : MonoBehaviour
{
    public float spawnInterval;
    public float spawnDistance;
    public TestFPS[] objectPrefabs;

    [Header("Objects spawned")] public float totalObjects = 0;

    private float elapsedTime;

    private void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= spawnInterval)
        {
            elapsedTime -= spawnInterval;
            Spawn();
            totalObjects++;
        }
    }

    private void Spawn()
    {
        TestFPS temp = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        TestFPS spawn = Instantiate<TestFPS>(temp);
        spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
