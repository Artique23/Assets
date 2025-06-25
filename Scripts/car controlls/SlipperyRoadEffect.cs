using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyRoadEffect : MonoBehaviour
{
    [Header("Slippery Settings")]
    public float reducedForwardFriction = 0.3f;
    public float reducedSidewaysFriction = 0.2f;

    [Header("Wheel Colliders (Assign)")]
    public WheelCollider[] wheelColliders;

    void Start()
    {
        ApplySlipperyPhysics();
    }

    void ApplySlipperyPhysics()
    {
        foreach (WheelCollider wheel in wheelColliders)
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;

            forwardFriction.stiffness = reducedForwardFriction;
            sidewaysFriction.stiffness = reducedSidewaysFriction;

            wheel.forwardFriction = forwardFriction;
            wheel.sidewaysFriction = sidewaysFriction;
        }
    }
}
