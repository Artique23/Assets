using UnityEngine;
using UnityEditor;

public class BakeSplineMesh
{
    [MenuItem("Tools/Bake Selected Spline Mesh")]
    static void BakeSelectedSplineMesh()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogError("❌ No GameObject selected.");
            return;
        }

        MeshFilter meshFilter = selected.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("❌ Selected object has no MeshFilter or mesh to bake.");
            return;
        }

        // Duplicate the mesh
        Mesh bakedMesh = Object.Instantiate(meshFilter.sharedMesh);
        string path = "Assets/Baked_" + selected.name + ".asset";

        // Save mesh asset
        AssetDatabase.CreateAsset(bakedMesh, path);
        AssetDatabase.SaveAssets();

        // Assign baked mesh to MeshFilter
        meshFilter.sharedMesh = bakedMesh;

        // Add or get MeshCollider component
        MeshCollider meshCollider = selected.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = selected.AddComponent<MeshCollider>();
        }

        // Assign baked mesh to the MeshCollider
        meshCollider.sharedMesh = bakedMesh;

        Debug.Log("✅ Baked mesh saved and MeshCollider added using: " + path);
    }
}
