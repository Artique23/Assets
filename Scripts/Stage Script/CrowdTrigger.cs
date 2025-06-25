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
            StartCoroutine(HandleCrowdFadeSequence());
        }
    }

    private IEnumerator HandleCrowdFadeSequence()
    {
        // Start black screen fade
        yield return ScreenFader.Instance.FadeInOut(0.5f, 1f); // Fade in 0.5s, hold 1s, fade out

        // Trigger walk-away animation
        foreach (Animator anim in crowdAnimators)
        {
            if (anim != null)
                anim.SetTrigger(walkTriggerName);
        }

        // Add points
        StageScoreManager.Instance.AddPoints(rewardPoints);

        // Destroy crowd parent after animation plays
        if (crowdParent != null)
            Destroy(crowdParent, 2f);
    }
}
