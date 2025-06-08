using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCam : MonoBehaviour
{
    public Transform targetCamera; // Your main camera
    public Transform targetCamera2; // Second camera

    void LateUpdate()
    {
        if (targetCamera2 != null && targetCamera2.gameObject.activeInHierarchy)
        {
            transform.rotation = targetCamera2.rotation;
        }
        else if (targetCamera != null && targetCamera.gameObject.activeInHierarchy)
        {
            transform.rotation = targetCamera.rotation;
        }
    }
}