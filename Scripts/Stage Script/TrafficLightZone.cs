using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightZone : MonoBehaviour
{
    public RedLightStatus redLightStatus; // Assign the correct RedLightStatus (the light for the player's lane)
    public StageBaseManager tutorialManager; // For Wade hints
    public float requiredWaitTime = 2f;
    public int rewardPoints = 100;
    public int penaltyPoints = -50;

    private bool playerInside = false;
    private float waitTimer = 0f;
    private bool rewarded = false;
    private bool penalized = false;
    private bool waitHintShown = false; // Prevent spamming "Wait for green!"

    void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform;
        while (t != null)
        {
            if (t.CompareTag("Player"))
            {
                playerInside = true;
                waitTimer = 0f;
                rewarded = false;
                penalized = false;
                waitHintShown = false;
                Debug.Log("Player entered trigger (found by parent tag)!");
                break;
            }
            Debug.Log("Trigger entered by: " + t.gameObject.name);
            t = t.parent; // Check parent hierarchy for "Player" tag
        }
    }

    void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        while (t != null)
        {
            if (t.CompareTag("Player"))
            {
                // If leaving during red, punish
                if (IsRed() && !rewarded && !penalized)
                {
                    PunishPlayer();
                }
                playerInside = false;
                waitTimer = 0f;
                waitHintShown = false;
                break;
            }
            t = t.parent;
        }
    }

    void Update()
    {
        if (!playerInside || rewarded) return;

        if (IsRed())
        {
            waitTimer += Time.deltaTime;
            if (tutorialManager != null && !waitHintShown)
            {
                tutorialManager.ShowWade("Wait for green!");
                StartCoroutine(HideWadeAfterDelay(2f));
                waitHintShown = true;
            }
        }
        else // Turned green while inside
        {
            if (waitTimer >= requiredWaitTime && !rewarded)
            {
                RewardPlayer();
            }
        }
    }

    bool IsRed()
    {
        return redLightStatus.lightGroupId == redLightStatus.intersection.currentRedLightsGroup;
    }

    void RewardPlayer()
    {
        rewarded = true;
        if (tutorialManager != null)
        {
            tutorialManager.ShowWade("Good job! You waited for green. +" + rewardPoints + " points");
            StartCoroutine(HideWadeAfterDelay(2f));
        }
        StageScoreManager.Instance.AddPoints(rewardPoints); // ADD THIS LINE
    }

    void PunishPlayer()
    {
        penalized = true;
        if (tutorialManager != null)
        {
            tutorialManager.ShowWade("Don't run red lights! " + penaltyPoints + " points");
            StartCoroutine(HideWadeAfterDelay(2f));
        }
        StageScoreManager.Instance.AddPoints(penaltyPoints); // ADD THIS LINE
    }


    private IEnumerator HideWadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tutorialManager != null)
            tutorialManager.HideWade();
    }
}
