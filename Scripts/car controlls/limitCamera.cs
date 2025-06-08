using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limitCamera : MonoBehaviour
{
    public GameObject car;

    private void LateUpdate()
    {
        if (car != null)
        {
            transform.position = new Vector3(car.transform.position.x, transform.position.y, car.transform.position.z);
        }
    }
}
