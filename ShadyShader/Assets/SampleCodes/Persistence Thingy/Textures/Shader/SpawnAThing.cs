using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAThing : MonoBehaviour
{
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;

    public Material customMaterial;

    [Header("Object to spawn")] public PrimitiveType thing;
    

    private List<GameObject> temp;

    private void Awake()
    {
        temp = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            GameObject t = GameObject.CreatePrimitive(thing);
            t.name = "boi";
            t.transform.localPosition = Random.insideUnitSphere * 5.0f;
            t.transform.localRotation = Random.rotation;
            t.transform.localScale = Vector3.one * Random.Range(0.4f, 1.4f);
            temp.Add(t);
            
            // Assign custom material (can be done via prefab)
            t.gameObject.GetComponent<MeshRenderer>().material = customMaterial;
            // Choose random color
            Color randomColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f);
            // Assign color to the material
            t.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", randomColor);

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
