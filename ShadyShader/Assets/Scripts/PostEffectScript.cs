using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectScript : MonoBehaviour
{

    public Material PostProcMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // src is the fully rendered scene tht would normaly be sent directly to the monitor;
        // This intercepts it and does work to it

        Graphics.Blit(source, destination, PostProcMaterial);
    }

}
