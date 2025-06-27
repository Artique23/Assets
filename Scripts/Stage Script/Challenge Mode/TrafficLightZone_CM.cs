using System.Collections;
using UnityEngine;

public class TrafficLightZone_CM : MonoBehaviour
{
    public RedLightStatus redLightStatus; // Assign the correct RedLightStatus (the light for the player's lane)
    public float requiredWaitTime = 2f;
    public int rewardPoints = 500;

    private bool playerInside = false;
    private float waitTimer = 0f;
    private bool rewarded = false;
    private bool penalized = false;

    public ChallengeModeManager challengeManager; // Assign in Inspector

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
                break;
            }
            t = t.parent;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        while (t != null)
        {
            if (t.CompareTag("Player"))
            {
                if (IsRed() && !rewarded && !penalized)
                {
                    ApplyPunishment(); // ❌ Replace penalty points with life deduction
                }
                playerInside = false;
                waitTimer = 0f;
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
        }
        else
        {
            if (waitTimer >= requiredWaitTime && !rewarded)
            {
                RewardPlayer(); // ✅ Still gives points
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
        StageScoreManager.Instance.AddPoints(rewardPoints); // ✅ Keep reward system
    }

    void ApplyPunishment()
    {
        penalized = true;
        if (challengeManager != null)
            challengeManager.ApplyPunishment(); // ❌ Life loss instead of minus points
    }
}
