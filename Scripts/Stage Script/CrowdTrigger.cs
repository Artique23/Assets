using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdTrigger : MonoBehaviour
{
     public Animator[] crowdAnimators;         // All NPC animators
    public GameObject crowdParent;            // The whole crowd group (optional, to disable later)
    public string walkTriggerName = "WalkAway"; // The trigger used in Animator
    public int rewardPoints = 50;

    private bool playerInside = false;
    private bool cleared = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    public void OnHornPressed()
    {
        if (!cleared && playerInside)
        {
            cleared = true;

            foreach (Animator anim in crowdAnimators)
            {
                if (anim != null)
                    anim.SetTrigger(walkTriggerName); // Must match trigger name in Animator
            }

            StageScoreManager.Instance.AddPoints(rewardPoints);

            // Optional: destroy or hide crowd after 2 seconds
            if (crowdParent != null)
                Destroy(crowdParent, 2f);
        }
    }
}
