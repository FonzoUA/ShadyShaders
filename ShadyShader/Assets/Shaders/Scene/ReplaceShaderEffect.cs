using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplaceShaderEffect : MonoBehaviour
{
    public Shader ReplacementShader;
    public Color OverDrawColor;

    private void OnValidate()
    {
        Shader.SetGlobalColor("_OverColor", OverDrawColor);
    }

    private void OnEnable()
    {
        if (ReplacementShader != null)
        {
            GetComponent<Camera>().SetReplacementShader(ReplacementShader, "");
        }
    }

    private void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }

}
