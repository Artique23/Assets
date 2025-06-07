using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneTrigger_Challenge : MonoBehaviour
{
    public string laneName; // Set in Inspector ("Lane 0" or "Lane 1")
    public int laneIndex; // 0 or 1, set in inspector
    public int rewardPoints = 10;

    private static int lastLane = -1;
    private ChallengeModeManager challengeModeManager;

    void Start()
    {
        challengeModeManager = FindObjectOfType<ChallengeModeManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var carLight = other.GetComponentInChildren<CarlightController>();
            // Only act if actually switching lanes
            if (lastLane != -1 && lastLane != laneIndex)
            {
                bool signaled = carLight != null && (carLight.LeftSignalIsOn() || carLight.RightSignalIsOn());
                if (signaled)
                {
                    challengeModeManager.AddChallengePoints(rewardPoints);
                }
                else
                {
                    challengeModeManager.LoseLife();
                }
                Debug.Log("Player switched from lane " + lastLane + " to lane " + laneIndex + (signaled ? " WITH SIGNAL" : " NO SIGNAL"));
            }
            lastLane = laneIndex;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Optionally: reset state if player leaves both lanes
        }
    }
}
