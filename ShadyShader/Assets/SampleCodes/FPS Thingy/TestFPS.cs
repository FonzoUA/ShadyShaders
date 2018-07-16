using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestFPS : MonoBehaviour
{
    public float attractionForce;

    private Rigidbody rigbod;

    private void Awake()
    {
        rigbod = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigbod.AddForce(transform.localPosition * -attractionForce);
    }
}
