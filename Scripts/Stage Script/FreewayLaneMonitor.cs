using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreewayLaneMonitor : MonoBehaviour
{
     public StageBaseManager stageBaseManager;
    public float[] laneCenters;
    private int currentLane = -1;
    private int freewayZoneCounter = 0;
    private CarlightController carLightController;

    // --- Buffer to track recent signal use ---
    private float signalUsedTimer = 0f;
    public float requiredSignalTime = 0.5f; // seconds before lane change

    void Start()
    {
        carLightController = GetComponentInChildren<CarlightController>();
        currentLane = GetLaneIndex(transform.position);
    }

    void Update()
    {
        // Track signal use
        if (carLightController != null && (carLightController.LeftSignalIsOn() || carLightController.RightSignalIsOn()))
        {
            signalUsedTimer = requiredSignalTime;
        }
        else if (signalUsedTimer > 0f)
        {
            signalUsedTimer -= Time.deltaTime;
        }

        if (freewayZoneCounter > 0)
        {
            int laneNow = GetLaneIndex(transform.position);
            if (laneNow != currentLane)
            {
                // Only reward if signal was used very recently
                if (signalUsedTimer > 0f)
                {
                    StageScoreManager.Instance.AddPoints(10);
                    stageBaseManager.ShowWade("Good job! You signaled before changing lanes. (+10)");
                }
                else
                {
                    StageScoreManager.Instance.AddPoints(-10);
                    stageBaseManager.ShowWade("You changed lanes without signaling! (-10)");
                }
                currentLane = laneNow;
            }
        }
    }

    int GetLaneIndex(Vector3 position)
    {
        float minDist = float.MaxValue;
        int bestLane = 0;
        for (int i = 0; i < laneCenters.Length; i++)
        {
            float dist = Mathf.Abs(position.x - laneCenters[i]);
            if (dist < minDist)
            {
                minDist = dist;
                bestLane = i;
            }
        }
        return bestLane;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FreewayZone"))
            freewayZoneCounter++;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FreewayZone"))
        {
            freewayZoneCounter--;
            if (freewayZoneCounter < 0) freewayZoneCounter = 0;
        }
    }
}
