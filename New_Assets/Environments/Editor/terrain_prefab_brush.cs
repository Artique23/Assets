using UnityEngine;
using UnityEditor;

public class TerrainPrefabBrush : EditorWindow
{
    GameObject prefabToPaint;
    float brushRadius = 5f;
    int density = 5;
    Vector2 scaleRange = new Vector2(1f, 1f);
    Vector2 rotationRange = new Vector2(0f, 360f);
    float heightOffset = 0f;
    bool isPainting = false;
    bool isErasing = false;
    LayerMask paintLayer = ~0;

    [MenuItem("Tools/Terrain Prefab Brush")]
    static void Init()
    {
        TerrainPrefabBrush window = (TerrainPrefabBrush)GetWindow(typeof(TerrainPrefabBrush));
        window.titleContent = new GUIContent("Prefab Brush");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Brush Settings", EditorStyles.boldLabel);

        prefabToPaint = (GameObject)EditorGUILayout.ObjectField("Prefab to Paint", prefabToPaint, typeof(GameObject), false);
        brushRadius = EditorGUILayout.FloatField("Brush Radius", brushRadius);
        density = EditorGUILayout.IntSlider("Density (per click)", density, 1, 50);
        scaleRange = EditorGUILayout.Vector2Field("Scale Range", scaleRange);
        rotationRange = EditorGUILayout.Vector2Field("Y Rotation Range", rotationRange);
        heightOffset = EditorGUILayout.FloatField("Y Position Offset", heightOffset);

        isPainting = EditorGUILayout.Toggle("Enable Painting", isPainting);
        isErasing = EditorGUILayout.Toggle("Enable Erasing", isErasing);

        if (prefabToPaint == null && isPainting)
            EditorGUILayout.HelpBox("Assign a prefab to paint.", MessageType.Warning);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            // Draw brush preview
            Handles.color = new Color(0, 1, 0, 0.25f);
            Handles.DrawSolidDisc(hit.point, Vector3.up, brushRadius);

            if (isPainting && prefabToPaint != null && e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                PaintPrefabs(hit.point);
                e.Use();
            }
            else if (isErasing && e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                ErasePrefabs(hit.point);
                e.Use();
            }
        }

        SceneView.RepaintAll();
    }

    void PaintPrefabs(Vector3 center)
    {
        for (int i = 0; i < density; i++)
        {
            Vector2 offset = Random.insideUnitCircle * brushRadius;
            Vector3 position = center + new Vector3(offset.x, 0, offset.y);

            if (Physics.Raycast(new Vector3(position.x, 999, position.z), Vector3.down, out RaycastHit hit))
            {
                Vector3 spawnPoint = hit.point + new Vector3(0, heightOffset, 0);
                Quaternion rot = Quaternion.Euler(0, Random.Range(rotationRange.x, rotationRange.y), 0);
                float scale = Random.Range(scaleRange.x, scaleRange.y);

                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPaint);
                obj.transform.position = spawnPoint;
                obj.transform.rotation = rot;
                obj.transform.localScale = Vector3.one * scale;

                Undo.RegisterCreatedObjectUndo(obj, "Paint Prefab");
            }
        }
    }

    void ErasePrefabs(Vector3 center)
    {
        Collider[] colliders = Physics.OverlapSphere(center, brushRadius);

        foreach (Collider col in colliders)
        {
            GameObject go = col.gameObject;
            if (PrefabUtility.GetCorrespondingObjectFromSource(go) == prefabToPaint || go.name.Contains(prefabToPaint.name))
            {
                Undo.DestroyObjectImmediate(go);
            }
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
