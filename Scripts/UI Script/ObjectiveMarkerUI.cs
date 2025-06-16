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

    private int currentObjectiveIndex = 0;
    private Transform player;
    private Tweener rotationTween;
    private RectTransform distanceTextRect;

    void Start()
    {
        if (objectives.Length > 0)
            SetTarget(objectives[0]);
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Tag your player
        
        // Get the text's RectTransform for positioning
        if (distanceText != null)
            distanceTextRect = distanceText.rectTransform;
        
        // Start the rotation animation
        StartRotationAnimation();
    }

    void StartRotationAnimation()
    {
        // Kill any existing rotation
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
        
        if (markerRect != null)
        {
            // Calculate full rotation based on direction
            float targetRotation = rotateClockwise ? 360f : -360f;
            
            // Calculate duration properly - higher speed should result in faster rotation
            float duration = 360f / rotationSpeed; // This converts degrees per second to seconds per full rotation
            
            Debug.Log($"Starting rotation with speed: {rotationSpeed} degrees/sec, duration: {duration} seconds per rotation");
            
            // Create infinite rotation tween with corrected duration calculation
            rotationTween = markerRect.DOLocalRotate(
                new Vector3(0, targetRotation, 0), // Rotate around Y axis
                duration, // Corrected duration calculation
                RotateMode.LocalAxisAdd) // Add to current rotation
                .SetEase(Ease.Linear) // Smooth, constant rotation
                .SetLoops(-1, LoopType.Restart) // Infinite looping
                .SetUpdate(true); // Works even when game is paused
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

        // Position the marker
        markerRect.position = clampedPos;
        
        // Position the distance text with offset
        if (distanceTextRect != null)
        {
            distanceTextRect.position = new Vector3(clampedPos.x + textOffset.x, clampedPos.y + textOffset.y, distanceTextRect.position.z);
        }

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
        
        // Also toggle distance text visibility
        if (distanceText != null)
            distanceText.gameObject.SetActive(target != null);
            
        // Restart rotation if setting a new target
        if (target != null && (rotationTween == null || !rotationTween.IsActive()))
        {
            StartRotationAnimation();
        }
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
            
            // Stop rotation when no more objectives
            if (rotationTween != null && rotationTween.IsActive())
                rotationTween.Kill();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up tween when destroyed
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
    }
}
