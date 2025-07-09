using System.Collections;
using UnityEngine;

public class LeftTurnYieldScenario_CM : MonoBehaviour
{
    public CarlightController carlightController; // Assign the player's CarlightController script
    public Transform[] aiCars; // Drag blue-path AI cars here
    public float dangerZone = 15f;
    public float waitTimeThreshold = 1.5f;

    public int rewardPoints = 200;

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

            if (!didSignal)
            {
                // ❌ Signal missing = life penalty
                ChallengeModeManager.Instance?.ApplyPunishment();
            }
            else if (!danger && waitedLongEnough)
            {
                // ✅ Good behavior = reward points
                StageScoreManager.Instance.AddPoints(rewardPoints);
            }
            else if (!danger)
            {
                // ✅ Good signal but maybe didn't wait long enough = still reward
                StageScoreManager.Instance.AddPoints(rewardPoints);
            }
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
}
