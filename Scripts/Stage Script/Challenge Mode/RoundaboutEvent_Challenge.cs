using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundaboutEvent_Challenge : MonoBehaviour
{
    public float rewardPoints = 10f;
    public ChallengeModeManager challengeModeManager; // Assign in Inspector or find at runtime

    private void Start()
    {
        if (challengeModeManager == null)
            challengeModeManager = FindObjectOfType<ChallengeModeManager>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var carLightController = other.GetComponentInChildren<CarlightController>();
            bool hasSignaled = carLightController != null &&
                               (carLightController.LeftSignalIsOn() || carLightController.RightSignalIsOn());

            if (challengeModeManager != null)
            {
                if (hasSignaled)
                    challengeModeManager.AddChallengePoints((int)rewardPoints);
                else
                    challengeModeManager.LoseLife();
            }
            else
            {
                // If used outside challenge mode, fallback to score manager (optional)
                if (hasSignaled)
                    StageScoreManager.Instance.AddPoints((int)rewardPoints);
                else
                    StageScoreManager.Instance.AddPoints(-5); // Or whatever penalty
            }
        }
    }
}
