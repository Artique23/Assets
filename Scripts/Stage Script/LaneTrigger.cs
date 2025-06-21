using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneTrigger : MonoBehaviour
{
    public string laneName; // Set in Inspector ("Lane 0" or "Lane 1")
    public StageBaseManager stageBaseManager;
    public int laneIndex; // 0 or 1, set in inspector

    // Static variable to remember where the player was last detected
    private static int lastLane = -1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var carLight = other.GetComponentInChildren<CarlightController>();
            // Only react if switching from the other lane
            if (lastLane != -1 && lastLane != laneIndex)
            {
                bool signaled = carLight != null && (carLight.LeftSignalIsOn() || carLight.RightSignalIsOn());
                if (signaled)
                {
                    StageScoreManager.Instance.AddPoints(100);
                    stageBaseManager.ShowWade("Good job! You signaled before changing lanes. (+100)");
                    StartCoroutine(HideWadeAfterDelay(1f)); // Hide after 1 second
                }
                else
                {
                    StageScoreManager.Instance.AddPoints(-100);
                    stageBaseManager.ShowWade("You changed lanes without signaling! (-100)");
                    StartCoroutine(HideWadeAfterDelay(1f)); // Hide after 1 second
                }
                Debug.Log("Player switched from lane " + lastLane + " to lane " + laneIndex + (signaled ? " WITH SIGNAL" : " NO SIGNAL"));
            }
            lastLane = laneIndex; // Update last lane
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Optionally: reset state if player leaves both lanes
        }
    }

    // Coroutine to hide Wade's dialog after a delay
    IEnumerator HideWadeAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        stageBaseManager.HideWade();
    }
}
