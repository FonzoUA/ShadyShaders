using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveTest : MonoBehaviour
{
    private Renderer rend;
	void Start ()
    {
        rend = this.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        rend.material.SetFloat("_CutoutThresh", GameManager.Instance.GetDissolveValue());	
	}
}
