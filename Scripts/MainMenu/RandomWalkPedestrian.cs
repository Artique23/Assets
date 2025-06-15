using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkPedestrian : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float distanceLimit = 500f;
    [SerializeField] private float respawnCooldown = 20f;
    
    [Header("Debug Info")]
    [SerializeField] private float currentDistance; // This will be visible in inspector
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float distanceTraveled;
    private bool isRespawning = false;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        Debug.Log($"Initial position set to: {startPosition}");
    }

    void Update()
    {
        if (isRespawning)
            return;

        // Random rotation
        transform.Rotate(Vector3.up, Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime);

        // Move forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Calculate and display distance traveled
        distanceTraveled = Vector3.Distance(startPosition, transform.position);
        currentDistance = distanceTraveled; // Update the inspector value

        // Check if we've reached the distance limit
        if (distanceTraveled >= distanceLimit && !isRespawning)
        {
            Debug.Log($"Distance limit reached: {distanceTraveled:F2} units");
            StartCoroutine(RespawnWithCooldown());
        }
    }

    private IEnumerator RespawnWithCooldown()
    {
        isRespawning = true;
        Debug.Log("Starting respawn cooldown...");
        
        // Hide the model/mesh instead of deactivating the whole GameObject
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        
        yield return new WaitForSeconds(respawnCooldown);
        
        // Reset position and rotation
        transform.position = startPosition;
        transform.rotation = startRotation;
        distanceTraveled = 0f;
        
        // Show the model/mesh again
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
        
        Debug.Log("Respawned at start position");
        isRespawning = false;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(startPosition, distanceLimit);
        }
    }
}
