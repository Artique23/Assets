using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundaboutExitEvent : MonoBehaviour
{
    public StageBaseManager stageBaseManager; // Assign in Inspector
    private bool messageShown = false;

    void OnTriggerEnter(Collider other)
    {
        if (!messageShown && other.CompareTag("Player"))
        {
            messageShown = true;
            stageBaseManager.ShowWade("Signal before exiting the roundabout!");
            StartCoroutine(HideWadeAfterDelay());
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Player exited the roundabout exit trigger!"); // Debug line

        if (other.CompareTag("Player"))
        {
            // Use GetComponentInChildren in case CarlightController is not on root
            var carLightController = other.GetComponentInChildren<CarlightController>();
            bool hasSignaled = carLightController != null && (carLightController.LeftSignalIsOn() || carLightController.RightSignalIsOn());

            if (hasSignaled)
            {
                StageScoreManager.Instance.AddPoints(600);
                stageBaseManager.ShowWade("Great job signaling before exiting the roundabout! (+600 points)");
            }
            else
            {
                StageScoreManager.Instance.AddPoints(-250);
                stageBaseManager.ShowWade("You forgot to signal before exiting! (-250 points)");
            }
            StartCoroutine(HideWadeAfterDelayWithReset());
        }
    }

    IEnumerator HideWadeAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        stageBaseManager.HideWade();
    }

    IEnumerator HideWadeAfterDelayWithReset()
    {
        yield return new WaitForSeconds(3f);
        stageBaseManager.HideWade();
        messageShown = false;
    }
}
