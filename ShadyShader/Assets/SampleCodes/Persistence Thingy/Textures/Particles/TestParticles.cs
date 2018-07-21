using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticles : MonoBehaviour
{
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;

    public SpawnParticlesController particles;

    private List<GameObject> temp;

    private void Awake()
    {
        temp = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Cube);
            t.name = "boi";
            t.transform.localPosition = Random.insideUnitSphere * 5.0f;
            t.transform.localRotation = Random.rotation;
            t.transform.localScale = Vector3.one * Random.Range(0.4f, 1.4f);
            temp.Add(t);


            particles.PlayParticles(t.transform.localPosition, t.transform.localScale.x * 0.5f);

        }
        else if (Input.GetKeyDown(destroyKey))
        {
            if (temp.Count <= 0)
                return;
            int index = Random.Range(0, temp.Count);
            Destroy(temp[index].gameObject);
            int lastIndex = temp.Count - 1;
            temp[index] = temp[lastIndex];
            temp.RemoveAt(lastIndex);
        }
    }

}
