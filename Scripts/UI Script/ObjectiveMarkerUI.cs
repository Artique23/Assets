using UnityEngine;
using TMPro;

public class ObjectiveMarkerUI : MonoBehaviour
{
    public Transform target; // Assign your ObjectiveTarget here
    public Camera mainCamera; // Assign your main camera
    public RectTransform canvasRect; // Assign the global UI Canvas RectTransform
    public RectTransform markerRect; // Assign this marker's RectTransform
    public TextMeshProUGUI distanceText;
    public float screenEdgeBuffer = 40f;

    void Update()
    {
        if (target == null || mainCamera == null || markerRect == null || canvasRect == null)
            return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // Flip marker to front if it's behind camera
        if (screenPos.z < 0)
            screenPos *= -1;

        // Clamp to screen edge
        Vector2 clampedPos = screenPos;
        clampedPos.x = Mathf.Clamp(clampedPos.x, screenEdgeBuffer, Screen.width - screenEdgeBuffer);
        clampedPos.y = Mathf.Clamp(clampedPos.y, screenEdgeBuffer, Screen.height - screenEdgeBuffer);

        markerRect.position = clampedPos;

        float distance = Vector3.Distance(mainCamera.transform.position, target.position);
        distanceText.text = Mathf.RoundToInt(distance) + "m";
    }
}
