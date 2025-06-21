using UnityEngine;
using TrafficSimulation;
using System.Collections.Generic;

public class WrongWayFromSegment : MonoBehaviour
{
    public List<Segment> allSegments; // Populate this manually or via manager
    public float minSpeedToTrigger = 2f;
    public StageBaseManager stageManager;

    private Rigidbody rb;
    private bool warningShown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb.velocity.magnitude < minSpeedToTrigger)
        {
            warningShown = false;
            return;
        }

        Segment currentSegment = GetCurrentSegment();
        if (currentSegment == null || currentSegment.waypoints.Count < 2)
            return;

        Vector3 carForward = transform.forward;
        Vector3 segmentDirection = (currentSegment.waypoints[1].transform.position - currentSegment.waypoints[0].transform.position).normalized;

        float dot = Vector3.Dot(carForward.normalized, segmentDirection);
        bool wrongWay = dot < 0f;

        if (wrongWay && !warningShown)
        {
            warningShown = true;
            Debug.LogWarning("Wrong Way on Segment!");
            stageManager?.ShowWadeMad("You're going the wrong way! -20 points");
            StageScoreManager.Instance.AddPoints(-20);
        }

        if (!wrongWay)
        {
            warningShown = false;
        }
    }

    Segment GetCurrentSegment()
    {
        foreach (var segment in allSegments)
        {
            if (segment.IsOnSegment(transform.position))
                return segment;
        }
        return null;
    }
}
