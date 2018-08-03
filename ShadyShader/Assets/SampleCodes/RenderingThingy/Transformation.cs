using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformation : MonoBehaviour
{
    // In regards to 0 and 1 in case of 4x4 matrices
    // We know that we give it the value 1 to enable repositioning of points. 
    // If its value were 0, the offset would be ignored, but scaling and rotation would still happen.
    // 1 - point 0 - vector
    public abstract Matrix4x4 Matrix { get; }
    public Vector3 Apply(Vector3 point)
    {
        return Matrix.MultiplyPoint(point);
    }
}
