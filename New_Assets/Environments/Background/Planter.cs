using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> BuildingPrefabs;
    [SerializeField]
    private List<GameObject> Buildings;

    [SerializeField]
    private float scaleWidth, scaleHeight, distance;

    [ContextMenu("Generate Buildings")]
    private void Createbuildings()
    {
        foreach (var Building in Buildings)
        {
            DestroyImmediate(Building);
        }

        Buildings = new List<GameObject>();

        int prefabIndex = 0;
        float angle = 0f;

        for (int i = 0; i < 8; i++)
        {
            Buildings.Add(Instantiate(BuildingPrefabs[prefabIndex], transform));

            Buildings[i].transform.Rotate(new Vector3(0f, angle, 0f));

            angle += 45f;
            prefabIndex++;

            if (prefabIndex >= BuildingPrefabs.Count)
            {
                prefabIndex = 0;
            }
        }
    }


    [ContextMenu("SetBuildingScale")]
    private void SetBuildingScale()
    {
        if (Buildings == null || Buildings.Count == 0) return;
        var firstRenderer = Buildings[0]?.GetComponent<SpriteRenderer>();
        if (firstRenderer == null) return;

        float spriteLength = firstRenderer.bounds.size.x;
        distance = spriteLength / 2f + (Mathf.Sqrt(2) / 2) * spriteLength;


        foreach (var bdgs in Buildings)
        {
            if (bdgs == null) continue;
            bdgs.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1f);
            bdgs.transform.position = transform.position + distance * bdgs.transform.forward;
        }
    }

    private void OnValidate()
    {
        if (Buildings != null && Buildings.Count == 8)
            SetBuildingScale();
    }
}
