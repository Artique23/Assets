using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundaboutEvent : MonoBehaviour
{
    public StageBaseManager stageBaseManager; // Assign in Inspector

    private bool messageShown = false;

    void OnTriggerEnter(Collider other)
    {
        if (!messageShown && other.CompareTag("Player"))
        {
            messageShown = true;
            // Show Wade's initial message (show only once per entry)
            stageBaseManager.ShowWade("Careful now, there are many vehicles merging, signal properly and enter safely.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check signaling when leaving the roundabout trigger
            var carLightController = other.GetComponent<CarlightController>();
            bool hasSignaled = false;
            if (carLightController != null)
            {
                hasSignaled = carLightController.LeftSignalIsOn() || carLightController.RightSignalIsOn();
            }

            if (hasSignaled)
            {
                StageScoreManager.Instance.AddPoints(100);
                stageBaseManager.ShowWade("Great job signaling while leaving the roundabout! (+10 points)");
            }
            else
            {
                StageScoreManager.Instance.AddPoints(-50);
                stageBaseManager.ShowWade("You forgot to signal while leaving the roundabout! (-5 points)");
            }
            // Hide Wade after 3 seconds
            StartCoroutine(HideWadeAfterDelay());
        }
    }

    IEnumerator HideWadeAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        stageBaseManager.HideWade();
        messageShown = false; // Allow the entry message to show again if needed
    }
}
