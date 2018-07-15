using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform point;
    [Range(10, 1000)] public int numPoints;
    private Transform[] temp;

    private void Awake()
    {
        temp = new Transform[numPoints];
        float step = 2f / numPoints;
        Vector3 scale = Vector3.one * step;
        Vector3 position;
        position.y = 0f;
        position.z = 0;
        for (int i = 0; i < numPoints; i++)
        {
            temp[i] = Instantiate(point);
            position.x = (i + 0.5f) * step - 1f;
            temp[i].localPosition = position;
            temp[i].localScale = scale;
            temp[i].SetParent(transform, false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < numPoints; i++)
        {
            Transform t = temp[i];
            Vector3 position = t.localPosition;
            position.y = Mathf.Cos(Mathf.PI * position.x + Time.time) ;
            t.localPosition = position;
        }
    }

}
