using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform followTarget1;
    public Transform followTarget2;
    [SerializeField, Range(-1f, 1f)]
    private float parallaxFactor = 0.05f; // Positive for same direction, negative for opposite
    public float smoothSpeed = 2f;
    private float targetInitialX1;
    private float targetInitialX2;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        if (followTarget1 != null)
            targetInitialX1 = followTarget1.position.x;
        if (followTarget2 != null)
            targetInitialX2 = followTarget2.position.x;
    }

    void LateUpdate()
    {
        if (followTarget1 == null && followTarget2 == null) return;

        float currentX1 = followTarget1 != null ? followTarget1.position.x : targetInitialX1;
        float currentX2 = followTarget2 != null ? followTarget2.position.x : targetInitialX2;

        // Use the average X position of both targets
        float avgInitialX = (targetInitialX1 + targetInitialX2) / 2f;
        float avgCurrentX = (currentX1 + currentX2) / 2f;

        float xOffset = (avgCurrentX - avgInitialX) * parallaxFactor;
        Vector3 targetPosition = new Vector3(
            initialPosition.x + xOffset,
            initialPosition.y,
            initialPosition.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}