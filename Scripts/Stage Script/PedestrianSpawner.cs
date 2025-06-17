using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject npcPrefab; // Assign your walking NPC prefab in Inspector

    public void SpawnPedestrian()
    {
        Transform firstWaypoint = transform.GetChild(0);
        GameObject npc = Instantiate(npcPrefab, firstWaypoint.position, firstWaypoint.rotation);

        PedestrianWalker walker = npc.GetComponent<PedestrianWalker>();
        walker.SetupWaypoints(this);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Draw a sphere at each waypoint (each child)
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform waypoint = transform.GetChild(i);
            Gizmos.DrawSphere(waypoint.position, 0.3f);

            // Draw a line to the next waypoint (if any)
            if (i < transform.childCount - 1)
            {
                Transform nextWaypoint = transform.GetChild(i + 1);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
                Gizmos.color = Color.yellow; // Reset color for spheres
            }
        }
    }

    public void Respawn()
    {
        SpawnPedestrian();
    }

    void Start()
    {
        SpawnPedestrian();
    }
}
