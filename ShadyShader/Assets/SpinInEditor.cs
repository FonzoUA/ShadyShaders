using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinInEditor : MonoBehaviour
{

    public float speed = 1.0f;
    private Transform t;

    private void Start()
    {
        t = this.GetComponent<Transform>();
    }

    void Update ()
    {

        t.Rotate(0, 0, speed * Time.deltaTime);
	}
}
