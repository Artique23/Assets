using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObjectiveManager1 : MonoBehaviour
{
     public Transform[] targets;
    public GameObject markerUIPrefab;
    public RectTransform minimapRect;
    public Camera minimapCamera;

    void Start()
    {
        foreach (Transform t in targets)
        {
            GameObject m = Instantiate(markerUIPrefab, minimapRect);
            var marker = m.GetComponent<MinimapObjectiveMarker>();
            marker.minimapCamera = minimapCamera;
            marker.minimapRect = minimapRect;
            marker.SetTarget(t);
        }
    }
}
