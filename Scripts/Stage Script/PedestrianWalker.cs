using UnityEngine;
using System.Collections.Generic;

public class PedestrianWalker : MonoBehaviour
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 1.2f;
    public float waypointThreshold = 0.2f;

    private PedestrianSpawner mySpawner;

    public void SetupWaypoints(PedestrianSpawner spawner)
    {
        mySpawner = spawner;
        // Get waypoints from spawner's children
        waypoints = new List<Transform>();
        foreach (Transform child in spawner.transform)
            waypoints.Add(child);
        currentWaypointIndex = 0;
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir;

        if (Vector3.Distance(transform.position, target.position) < waypointThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                if (mySpawner != null)
                    mySpawner.Respawn();
                Destroy(gameObject); // Remove this NPC
            }
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        // Draw lines between waypoints
        Gizmos.color = Color.cyan; // Or any color you like
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        // Draw from last to first if looping
        // Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);

        // Draw spheres at each waypoint
        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints)
        {
            if (wp != null)
                Gizmos.DrawSphere(wp.position, 0.2f);
        }
    }
}
