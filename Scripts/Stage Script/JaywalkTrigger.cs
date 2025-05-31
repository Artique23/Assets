using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JaywalkTrigger : MonoBehaviour
{
    public GameObject npcPrefab; // Pedestrian prefab
    public Transform spawnPoint; // Where the NPC appears
    public Transform endPoint;   // Where the NPC walks to
    public Stage1TutorialManager tutorialManager;
    private bool hasSpawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (other.CompareTag("Player"))
        {
            hasSpawned = true;
            SpawnNPC();
            if (tutorialManager != null)
                tutorialManager.ShowWade("A pedestrian is crossing! Slow down or stop to let them cross safely.");
        }
    }

    void SpawnNPC()
    {
        GameObject npc = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        PedestrianMover mover = npc.GetComponent<PedestrianMover>();
        if (mover != null)
        {
            mover.Init(endPoint.position, tutorialManager);
        }
    }
}
