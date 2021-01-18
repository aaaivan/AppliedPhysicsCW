using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAxes : MonoBehaviour
{
    // draw axes in scene view for debugging
    void Update()
    {
        Debug.DrawRay(Vector3.zero, 1000 * Vector3.right, Color.red);
        Debug.DrawRay(Vector3.zero, -1000 * Vector3.right, Color.red);
        Debug.DrawRay(Vector3.zero, 1000 * Vector3.forward, Color.blue);
        Debug.DrawRay(Vector3.zero, -1000 * Vector3.forward, Color.blue);
        Debug.DrawRay(Vector3.zero, 1000 * Vector3.up, Color.green);
        Debug.DrawRay(Vector3.zero, -1000 * Vector3.up, Color.green);
    }
}
