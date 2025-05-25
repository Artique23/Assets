// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class RedLightStatus : MonoBehaviour
{

    public int lightGroupId;  // The ID of this light's group (e.g., which traffic light it belongs to)
    public Intersection intersection; // Reference to the intersection this light is part of
    
    Light pointLight; // The Light component representing the visual light

    void Start(){
        // Get the Light component from the first child of this GameObject
        pointLight = this.transform.GetChild(0).GetComponent<Light>();
        // Set the initial color of the traffic light
        SetTrafficLightColor();
    }

    // Update is called once per frame
    void Update(){
        // Continuously update the light color based on the intersection's state
        SetTrafficLightColor();
    }

    void SetTrafficLightColor(){
        // If this light's group is currently red at the intersection, set color to red
        if(lightGroupId == intersection.currentRedLightsGroup)
            pointLight.color = new Color(1, 0, 0); // Red
        else
            pointLight.color = new Color(0, 1, 0); // Green
    }
}