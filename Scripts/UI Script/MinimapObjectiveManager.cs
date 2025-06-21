using UnityEngine;
using UnityEngine.UI;

public class MinimapObjectiveMarker : MonoBehaviour
{
    public Transform target;
    public Camera minimapCamera;
    public RectTransform minimapRect;

    private RectTransform markerRect;
    public float borderBuffer = 15f; // Keeps icon slightly inside the border

    void Start()
    {
        markerRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null || minimapCamera == null || markerRect == null || minimapRect == null)
            return;

        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(target.position);

        Vector2 minimapSize = minimapRect.rect.size;
        Vector2 anchoredPos = new Vector2(
            (viewportPos.x - 0.5f) * minimapSize.x,
            (viewportPos.y - 0.5f) * minimapSize.y
        );

        // Clamp position to inside minimap
        Vector2 clamped = anchoredPos;
        float halfW = minimapSize.x / 2f - borderBuffer;
        float halfH = minimapSize.y / 2f - borderBuffer;
        clamped.x = Mathf.Clamp(clamped.x, -halfW, halfW);
        clamped.y = Mathf.Clamp(clamped.y, -halfH, halfH);
        markerRect.anchoredPosition = clamped;

        // Rotate icon to point toward target (optional)
        Vector2 dir = anchoredPos - clamped;
        if (dir.sqrMagnitude > 1f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            markerRect.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 to align arrow tip
        }
        else
        {
            markerRect.rotation = Quaternion.identity;
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
