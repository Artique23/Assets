using UnityEngine;
using TMPro;
using DG.Tweening;

public class ObjectiveMarkerUI : MonoBehaviour
{
    public Transform[] objectives; // Assign all your ObjectiveTarget transforms here in order
    public Camera mainCamera; // Assign your main camera
    public RectTransform canvasRect; // Assign the global UI Canvas RectTransform
    public RectTransform markerRect; // Assign this marker's RectTransform
    public TextMeshProUGUI distanceText;
    public float screenEdgeBuffer = 40f;
    public float objectiveReachRadius = 3f; // Distance for "interaction"

    [Header("Marker Rotation")]
    public float rotationSpeed = 120f; // Degrees per second
    public bool rotateClockwise = true; // Direction of rotation

    [Header("Distance Text Settings")]
    public Vector2 textOffset = new Vector2(0, -30); // Adjust to position text relative to marker

    [Header("Stage 4 (Optional)")]
    public Stage4Manager stage4Manager; // Assign only in Stage 4

    private int currentObjectiveIndex = 0;
    private Transform player;
    private Tweener rotationTween;
    private RectTransform distanceTextRect;
    private Transform target;

    void Start()
    {
        if (objectives.Length > 0)
            SetTarget(objectives[0]);
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (distanceText != null)
            distanceTextRect = distanceText.rectTransform;

        StartRotationAnimation();
    }

    void StartRotationAnimation()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();

        if (markerRect != null)
        {
            float targetRotation = rotateClockwise ? 360f : -360f;
            float duration = 360f / rotationSpeed;

            rotationTween = markerRect.DOLocalRotate(
                new Vector3(0, targetRotation, 0),
                duration,
                RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);
        }
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

        if (distanceTextRect != null)
        {
            distanceTextRect.position = new Vector3(clampedPos.x + textOffset.x, clampedPos.y + textOffset.y, distanceTextRect.position.z);
        }

        float distance = Vector3.Distance(player.position, target.position);
        distanceText.text = Mathf.RoundToInt(distance) + "m";

        if (distance < objectiveReachRadius)
        {
            NextObjective();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        markerRect.gameObject.SetActive(target != null);

        if (distanceText != null)
            distanceText.gameObject.SetActive(target != null);

        if (target != null && (rotationTween == null || !rotationTween.IsActive()))
        {
            StartRotationAnimation();
        }
    }

    void NextObjective()
    {
        // Stage 4 support: notify manager before advancing
        if (stage4Manager != null)
        {
            stage4Manager.OnObjectiveReached();
        }

        currentObjectiveIndex++;
        if (currentObjectiveIndex < objectives.Length)
        {
            SetTarget(objectives[currentObjectiveIndex]);
        }
        else
        {
            SetTarget(null);

            if (rotationTween != null && rotationTween.IsActive())
                rotationTween.Kill();
        }
    }

    private void OnDestroy()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
    }
}
