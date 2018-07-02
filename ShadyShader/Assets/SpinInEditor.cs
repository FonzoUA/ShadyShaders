using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinInEditor : MonoBehaviour
{
    public bool useDissolve = false;
    public float speed = 1.0f;
    public Vector3 SpinVector;
    private Transform t;

    private void Start()
    {
        t = this.GetComponent<Transform>();
    }

    void Update ()
    {
        if (!useDissolve)
            t.Rotate(SpinVector * (speed * Time.deltaTime));
        else
        {
            if (GameManager.Instance.GetDissolveValue() < 1.01f)
                t.Rotate(SpinVector * ((1 - GameManager.Instance.GetDissolveValue()) * (speed * Time.deltaTime)));
        }
	}
}
