using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BakeSplineMesh : MonoBehaviour
{
    [Header("Bake Settings")]
    public bool bakeOnStart = true;
    public bool makeInvisible = true;

    void Start()
    {
        if (bakeOnStart)
        {
            BakeMesh();
        }
    }

    public void BakeMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("No mesh found to bake.");
            return;
        }

        // Duplicate the mesh
        Mesh bakedMesh = Instantiate(mf.sharedMesh);
        bakedMesh.name = "BakedSplineMesh";

        // Assign to a new GameObject
        GameObject bakedGO = new GameObject("StaticSplineMesh");
        bakedGO.transform.position = transform.position;
        bakedGO.transform.rotation = transform.rotation;
        bakedGO.transform.localScale = transform.localScale;
        bakedGO.isStatic = true;

        // Add MeshFilter and assign the duplicated mesh
        MeshFilter newMF = bakedGO.AddComponent<MeshFilter>();
        newMF.sharedMesh = bakedMesh;

        // Optional: disable rendering
        if (!makeInvisible)
        {
            MeshRenderer oldMR = GetComponent<MeshRenderer>();
            if (oldMR)
            {
                MeshRenderer newMR = bakedGO.AddComponent<MeshRenderer>();
                newMR.sharedMaterial = oldMR.sharedMaterial;
            }
        }

        // Add collider
        MeshCollider mc = bakedGO.AddComponent<MeshCollider>();
        mc.sharedMesh = bakedMesh;
        mc.convex = false;

        Debug.Log("Spline mesh baked into static collider object.");
    }
}
