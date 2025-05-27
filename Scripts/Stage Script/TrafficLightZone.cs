using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightZone : MonoBehaviour
{
   public RedLightStatus redLightStatus; // Assign the correct RedLightStatus (the light for the player's lane)
    public Stage1TutorialManager tutorialManager; // For Wade hints
    public float requiredWaitTime = 2f;
    public int rewardPoints = 100;
    public int penaltyPoints = -50;

    private bool playerInside = false;
    private float waitTimer = 0f;
    private bool rewarded = false;
    private bool penalized = false;

    void OnTriggerEnter(Collider other)
    {
         Transform t = other.transform;
        while (t != null)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;
                waitTimer = 0f;
                rewarded = false;
                penalized = false;
                Debug.Log("Player entered trigger (found by parent tag)!");
                break;
            }
            Debug.Log("Trigger entered by: " + other.gameObject.name);
            t = t.parent; // Check parent hierarchy for "Player" tag
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If leaving during red, punish
            if (IsRed() && !rewarded && !penalized)
            {
                PunishPlayer();
            }
            playerInside = false;
            waitTimer = 0f;
        }
    }

    void Update()
    {
        if (!playerInside || rewarded) return;

        if (IsRed())
        {
            waitTimer += Time.deltaTime;
            if (tutorialManager != null)
                tutorialManager.ShowWade("Wait for green!");
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
            tutorialManager.ShowWade("Good job! You waited for green. +" + rewardPoints + " points");
        // Add points to your score system here
    }

    void PunishPlayer()
    {
        penalized = true;
        if (tutorialManager != null)
            tutorialManager.ShowWade("Don't run red lights! " + penaltyPoints + " points");
        // Subtract points here
    }
}
