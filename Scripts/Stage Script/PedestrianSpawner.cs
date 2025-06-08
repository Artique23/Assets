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

    public void Respawn()
    {
        SpawnPedestrian();
    }

    void Start()
    {
        SpawnPedestrian();
    }
}
