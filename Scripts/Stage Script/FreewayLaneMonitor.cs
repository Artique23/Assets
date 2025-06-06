using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreewayLaneMonitor : MonoBehaviour
{
    public StageBaseManager stageBaseManager;
    public CarlightController carLightController;

    private string currentLane = "";

    void OnTriggerEnter(Collider other)
    {
        // Only care about lane triggers
        if (other.name == "Lane 0 trigger" || other.name == "Lane 1 trigger")
        {
            // If actually switching lanes
            if (currentLane != "" && currentLane != other.name)
            {
                bool signaled = carLightController != null &&
                    (carLightController.LeftSignalIsOn() || carLightController.RightSignalIsOn());
                if (signaled)
                {
                    StageScoreManager.Instance.AddPoints(10);
                    stageBaseManager.ShowWade("Good job! You signaled before changing lanes. (+10)");
                }
                else
                {
                    StageScoreManager.Instance.AddPoints(-10);
                    stageBaseManager.ShowWade("You changed lanes without signaling! (-10)");
                }
                Debug.Log("Switched from " + currentLane + " to " + other.name + (signaled ? " WITH SIGNAL" : " NO SIGNAL"));
            }
            currentLane = other.name;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.name == "Lane 0 trigger" || other.name == "Lane 1 trigger") && currentLane == other.name)
        {
            currentLane = "";
            Debug.Log("Exited " + other.name);
        }
    }
}
