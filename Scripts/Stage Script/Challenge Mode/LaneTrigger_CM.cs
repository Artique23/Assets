using UnityEngine;

public class LaneTrigger_CM : MonoBehaviour
{
    public string laneName; // Optional for debugging
    public int laneIndex; // 0 or 1, set in inspector
    public ChallengeModeManager challengeManager; // Assign in Inspector

    private static int lastLane = -1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var carLight = other.GetComponentInChildren<CarlightController>();
            if (lastLane != -1 && lastLane != laneIndex)
            {
                bool signaled = carLight != null && (carLight.LeftSignalIsOn() || carLight.RightSignalIsOn());

                if (signaled)
                {
                    StageScoreManager.Instance.AddPoints(100); // ✅ Reward for signaling
                }
                else
                {
                    if (challengeManager != null)
                        challengeManager.ApplyPunishment(); // ❌ Lose life instead of losing points
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
            // Optional: clear state or reset logic
        }
    }
}
