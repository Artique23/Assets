using System.Collections;
using UnityEngine;

public class CrowdTrigger : MonoBehaviour
{
    public Animator[] crowdAnimators;
    public GameObject crowdParent;
    public string walkTriggerName = "WalkAway";
    public int rewardPoints = 50;

    public CanvasGroup blackscreen; // 🔹 Assign in Inspector

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
        // Fade to black (0.5s)
        yield return StartCoroutine(FadeBlackScreen(1f, 0.5f));

        // 🔹 Hold black screen for 2 seconds (you can change this!)
        yield return new WaitForSeconds(2f);

        // Trigger crowd animation
        foreach (Animator anim in crowdAnimators)
        {
            if (anim != null)
                anim.SetTrigger(walkTriggerName);
        }

        StageScoreManager.Instance.AddPoints(rewardPoints);

        if (crowdParent != null)
            Destroy(crowdParent, -1f);

        // Fade out to clear (0.5s)
        yield return StartCoroutine(FadeBlackScreen(0f, 0.5f));
    }


    private IEnumerator FadeBlackScreen(float targetAlpha, float duration)
    {
        if (blackscreen == null)
        {
            Debug.LogWarning("Blackscreen CanvasGroup not assigned!");
            yield break;
        }

        float startAlpha = blackscreen.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            blackscreen.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        blackscreen.alpha = targetAlpha;
    }
}
