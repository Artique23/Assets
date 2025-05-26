using UnityEngine;

public class FitWorkgroundToCamera : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera to adjust (defaults to main camera if not set)")]
    [SerializeField] private Camera targetCamera;

    [Header("Configuration")]
    [Tooltip("Reference resolution width for which your scene was designed")]
    [SerializeField] private float referenceWidth = 1920f;
    [Tooltip("Reference resolution height for which your scene was designed")]
    [SerializeField] private float referenceHeight = 1080f;
    [Tooltip("Reference orthographic size for your reference resolution")]
    [SerializeField] private float referenceOrthographicSize = 5f;
    [Tooltip("Whether to adjust on Start or continuously in Update")]
    [SerializeField] private bool adjustOnlyOnStart = true;
    [Tooltip("Minimum orthographic size allowed")]
    [SerializeField] private float minOrthographicSize = 3f;
    [Tooltip("Maximum orthographic size allowed")]
    [SerializeField] private float maxOrthographicSize = 10f;

    [Header("Perspective Camera Settings")]
    [Tooltip("Whether this is a perspective camera")]
    [SerializeField] private bool isPerspectiveCamera = false;
    [Tooltip("Reference field of view for perspective camera")]
    [SerializeField] private float referenceFieldOfView = 60f;
    [Tooltip("Minimum field of view allowed")]
    [SerializeField] private float minFieldOfView = 40f;
    [Tooltip("Maximum field of view allowed")]
    [SerializeField] private float maxFieldOfView = 80f;

    // Reference aspect ratio
    private float referenceAspect;

    private void Awake()
    {
        // If no camera assigned, use main camera
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Calculate reference aspect ratio
        referenceAspect = referenceWidth / referenceHeight;
    }

    private void Start()
    {
        if (adjustOnlyOnStart)
            AdjustCamera();
    }

    private void Update()
    {
        if (!adjustOnlyOnStart)
            AdjustCamera();
    }

    public void AdjustCamera()
    {
        if (targetCamera == null)
        {
            Debug.LogError("No camera assigned to FitWorkgroundToCamera!");
            return;
        }

        // Get the current aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;

        if (isPerspectiveCamera)
        {
            AdjustPerspectiveCamera(currentAspect);
        }
        else
        {
            AdjustOrthographicCamera(currentAspect);
        }
    }

    private void AdjustOrthographicCamera(float currentAspect)
    {
        // Calculate the orthographic size adjustment factor
        float adjustment = referenceAspect / currentAspect;

        // If current aspect is wider than reference, we need to increase the orthographic size
        // If it's taller, we need to maintain the same width visibility
        float newSize = referenceOrthographicSize;

        // Wider screens (like phones in landscape) - increase size to show more height
        if (currentAspect > referenceAspect)
        {
            // No adjustment needed - we'll see more on the sides
        }
        // Taller screens (like tablets or phones in portrait) - increase size to maintain width visibility
        else
        {
            newSize *= adjustment;
        }

        // Clamp the size within reasonable bounds
        newSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);

        // Apply the new orthographic size
        targetCamera.orthographicSize = newSize;

        Debug.Log($"Adjusted orthographic camera - Aspect: {currentAspect:F2}, Size: {newSize:F2}");
    }

    private void AdjustPerspectiveCamera(float currentAspect)
    {
        // Similar adjustment logic but for field of view
        float adjustment = referenceAspect / currentAspect;

        // Start with reference FOV
        float newFOV = referenceFieldOfView;

        // For wider screens - maintain the vertical FOV
        if (currentAspect > referenceAspect)
        {
            // No adjustment needed for wider screens in perspective camera
        }
        // For taller screens - increase FOV to maintain width visibility
        else
        {
            // The FOV adjustment for perspective cameras is different
            // This approximation helps maintain similar width visibility
            newFOV = Mathf.Atan(Mathf.Tan(referenceFieldOfView * Mathf.Deg2Rad * 0.5f) * adjustment) * 2f * Mathf.Rad2Deg;
        }

        // Clamp the FOV within reasonable bounds
        newFOV = Mathf.Clamp(newFOV, minFieldOfView, maxFieldOfView);

        // Apply the new field of view
        targetCamera.fieldOfView = newFOV;

        Debug.Log($"Adjusted perspective camera - Aspect: {currentAspect:F2}, FOV: {newFOV:F2}");
    }

    // Editor button to test adjustment
    [ContextMenu("Force Adjust Camera")]
    public void ForceAdjustCamera()
    {
        AdjustCamera();
    }

    // Method to manually set target camera (useful for dynamic camera changes)
    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
        AdjustCamera();
    }
}