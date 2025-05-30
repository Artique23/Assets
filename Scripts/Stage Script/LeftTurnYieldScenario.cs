using System.Collections;
using UnityEngine;

public class LeftTurnYieldScenario : MonoBehaviour
{
    public Stage1TutorialManager tutorialManager;
    public CarControls carControls; // Assign the player's CarControls script
    public Transform[] aiCars;      // Drag blue-path AI cars here
    public float dangerZone = 15f;  // Distance to consider "too close"
    public float waitTimeThreshold = 1.5f; // How long the player should wait in the zone

    // NEW: Points for reward/punish
    public int rewardPoints = 100;
    public int penaltyPoints = -50;
    public int playerScore = 0; // Replace with your global score manager if you have one

    private bool playerInZone = false;
    private float playerWaitTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerWaitTime = 0f;
            if (tutorialManager != null)
                tutorialManager.ShowWade("Use your LEFT turn signal and wait for oncoming traffic to clear before turning left!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerInZone)
        {
            playerInZone = false;

            bool didSignal = carControls != null && carControls.leftSignalOn;
            bool waitedLongEnough = playerWaitTime > waitTimeThreshold;
            bool danger = IsAICarApproaching();

            string msg = "";

            if (!didSignal && danger)
            {
                msg = "You must use your LEFT turn signal and yield to oncoming traffic! -" + Mathf.Abs(penaltyPoints) + " points";
                playerScore += penaltyPoints; // Punish
            }
            else if (!didSignal)
            {
                msg = "Don't forget your LEFT turn signal when turning left. -" + Mathf.Abs(penaltyPoints) + " points";
                playerScore += penaltyPoints; // Punish
            }
            else if (danger)
            {
                msg = "Watch out! Yield to oncoming traffic before turning left.";
                // No reward or penalty (optional: mild penalty)
            }
            else if (waitedLongEnough)
            {
                msg = "Excellent! You signaled and waited for a safe gap before turning. +" + rewardPoints + " points!";
                playerScore += rewardPoints; // Reward
            }
            else
            {
                msg = "Good signal! But always double-check for traffic before turning. +" + rewardPoints + " points!";
                playerScore += rewardPoints; // Optionally give small reward, or none
            }

            if (tutorialManager != null)
                tutorialManager.ShowWade(msg);

            Debug.Log("Current player score: " + playerScore);
            StartCoroutine(HideWadeAfterDelay(3f));
        }
    }

    void Update()
    {
        if (playerInZone)
            playerWaitTime += Time.deltaTime;
    }

    bool IsAICarApproaching()
    {
        foreach (var car in aiCars)
        {
            if (car == null) continue;
            float dist = Vector3.Distance(car.position, transform.position);
            if (dist < dangerZone)
                return true;
        }
        return false;
    }

    private IEnumerator HideWadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tutorialManager != null)
            tutorialManager.HideWade();
    }
}
