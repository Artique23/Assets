using System.Collections;
using UnityEngine;

public class LeftTurnYieldScenario : MonoBehaviour
{
    public StageBaseManager tutorialManager;

    public CarlightController carlightController; // Assign the player's CarlightController script
    public Transform[] aiCars; // Drag blue-path AI cars here
    public float dangerZone = 15f;
    public float waitTimeThreshold = 1.5f;

    public int rewardPoints = 200;
    public int penaltyPoints = -100;
    public int playerScore = 0;

    private bool playerInZone = false;
    private float playerWaitTime = 0f;
    private bool signalWasOnInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerWaitTime = 0f;
            signalWasOnInZone = false;

            if (tutorialManager != null)
                tutorialManager.ShowWade("Use your turn signal and wait for oncoming traffic to clear before turning!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerInZone)
        {
            playerInZone = false;

            bool didSignal = signalWasOnInZone;
            bool waitedLongEnough = playerWaitTime > waitTimeThreshold;
            bool danger = IsAICarApproaching();

            string msg = "";

            if (!didSignal && danger)
            {
                msg = "You must use your turn signal and yield to oncoming traffic! -" + Mathf.Abs(penaltyPoints) + " points";
                StageScoreManager.Instance.AddPoints(penaltyPoints);
            }
            else if (!didSignal)
            {
                msg = "Don't forget your turn signal when turning left. -" + Mathf.Abs(penaltyPoints) + " points";
                StageScoreManager.Instance.AddPoints(penaltyPoints);
            }
            else if (danger)
            {
                msg = "Watch out! Yield to oncoming traffic before turning left.";
            }
            else if (waitedLongEnough)
            {
                msg = "Excellent! You signaled and waited for a safe gap before turning. +" + rewardPoints + " points!";
                StageScoreManager.Instance.AddPoints(rewardPoints);
            }
            else
            {
                msg = "Good signal! But always double-check for traffic before turning. +" + rewardPoints + " points!";
                StageScoreManager.Instance.AddPoints(rewardPoints);
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
        {
            playerWaitTime += Time.deltaTime;

            if (carlightController != null &&
                (carlightController.LeftSignalIsOn() || carlightController.RightSignalIsOn()))
            {
                signalWasOnInZone = true;
            }
        }
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
