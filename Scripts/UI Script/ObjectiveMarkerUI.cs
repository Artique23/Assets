using UnityEngine;
using TMPro;

public class ObjectiveMarkerUI : MonoBehaviour
{
    public Transform[] objectives; // Assign all your ObjectiveTarget transforms here in order
    public Camera mainCamera; // Assign your main camera
    public RectTransform canvasRect; // Assign the global UI Canvas RectTransform
    public RectTransform markerRect; // Assign this marker's RectTransform
    public TextMeshProUGUI distanceText;
    public float screenEdgeBuffer = 40f;
    public float objectiveReachRadius = 3f; // Distance for "interaction"

    private int currentObjectiveIndex = 0;
    private Transform player;

    void Start()
    {
        if (objectives.Length > 0)
            SetTarget(objectives[0]);
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Tag your player
    }

    void Update()
    {
        if (target == null || mainCamera == null || markerRect == null || canvasRect == null || player == null)
            return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        if (screenPos.z < 0)
            screenPos *= -1;

        Vector2 clampedPos = screenPos;
        clampedPos.x = Mathf.Clamp(clampedPos.x, screenEdgeBuffer, Screen.width - screenEdgeBuffer);
        clampedPos.y = Mathf.Clamp(clampedPos.y, screenEdgeBuffer, Screen.height - screenEdgeBuffer);

        markerRect.position = clampedPos;

        float distance = Vector3.Distance(player.position, target.position);
        distanceText.text = Mathf.RoundToInt(distance) + "m";

        // Check for interaction
        if (distance < objectiveReachRadius)
        {
            NextObjective();
        }
    }

    private Transform target;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        markerRect.gameObject.SetActive(target != null);
    }

    void NextObjective()
    {
        currentObjectiveIndex++;
        if (currentObjectiveIndex < objectives.Length)
        {
            SetTarget(objectives[currentObjectiveIndex]);
        }
        else
        {
            // All objectives complete
            SetTarget(null); // Hide marker
        }
    }
}
