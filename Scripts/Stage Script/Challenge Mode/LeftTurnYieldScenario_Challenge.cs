using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftTurnYieldScenario_Challenge : MonoBehaviour
{
    public CarlightController carlightController; // Assign the player's CarlightController script
    public Transform[] aiCars;      // Drag blue-path AI cars here
    public float dangerZone = 15f;  // Distance to consider "too close"
    public float waitTimeThreshold = 1.5f; // How long the player should wait in the zone

    public int rewardPoints = 100;

    private bool playerInZone = false;
    private float playerWaitTime = 0f;
    private bool signalWasOnInZone = false; // Track if left signal was ever on in zone

    private ChallengeModeManager challengeModeManager;

    void Start()
    {
        challengeModeManager = FindObjectOfType<ChallengeModeManager>();
    }

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

            // Check if signal was EVER on while in the zone!
            bool didSignal = signalWasOnInZone;
            bool waitedLongEnough = playerWaitTime > waitTimeThreshold;
            bool danger = IsAICarApproaching();

            if (challengeModeManager != null)
            {
                // In Challenge Mode: punish by life, reward by points, no dialog!
                if (!didSignal || danger)
                {
                    challengeModeManager.LoseLife();
                }
                else if (waitedLongEnough)
                {
                    challengeModeManager.AddChallengePoints(rewardPoints);
                }
                else
                {
                    challengeModeManager.AddChallengePoints(rewardPoints);
                }
            }
            else
            {
                // Regular mode (not Challenge) — you can copy your original Wade dialog/score logic here if needed
                // (Or just leave this empty for pure Challenge Mode)
            }
        }
    }

    void Update()
    {
        if (playerInZone)
        {
            playerWaitTime += Time.deltaTime;
            // If signal is ON, mark it as true (remains true after)
            if (carlightController != null && carlightController.LeftSignalIsOn())
                signalWasOnInZone = true;
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
