using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlightController : MonoBehaviour
{
    [Header("Lights")]
    public Light headlightLeft;
    public Light headlightRight;
    public Light tailLightLeft;
    public Light tailLightRight;
    public Light brakeLightLeft;
    public Light brakeLightRight;
    public Light turnSignalLeft;
    public Light turnSignalRight;

    public Light reverseLight;

    private Coroutine hazardCoroutine;
    private Coroutine leftSignalCoroutine;
    private Coroutine rightSignalCoroutine;

    // State tracking for toggles
    private bool headlightsOn = false;
    private bool leftSignalOn = false;
    private bool rightSignalOn = false;
    private bool hazardsOn = false;



    // ---- UI BUTTONS SHOULD CALL THESE ----

    void Start()
    {
        // Ensure all booleans are false
        headlightsOn = false;
        leftSignalOn = false;
        rightSignalOn = false;
        hazardsOn = false;

        // Make sure all lights are off at start
        if (headlightLeft) headlightLeft.enabled = false;
        if (headlightRight) headlightRight.enabled = false;
        if (tailLightLeft) tailLightLeft.enabled = false;
        if (tailLightRight) tailLightRight.enabled = false;
        if (brakeLightLeft) brakeLightLeft.enabled = false;
        if (brakeLightRight) brakeLightRight.enabled = false;
        if (turnSignalLeft) turnSignalLeft.enabled = false;
        if (turnSignalRight) turnSignalRight.enabled = false;
    }

    public void FlashAllLights(float duration = 0.3f)
    {
        StartCoroutine(FlashAllLightsCoroutine(duration));
    }

    private IEnumerator FlashAllLightsCoroutine(float duration)
    {
        // Save the original state of each light
        bool headlightLeftState = headlightLeft && headlightLeft.enabled;
        bool headlightRightState = headlightRight && headlightRight.enabled;
        bool tailLightLeftState = tailLightLeft && tailLightLeft.enabled;
        bool tailLightRightState = tailLightRight && tailLightRight.enabled;
        bool brakeLightLeftState = brakeLightLeft && brakeLightLeft.enabled;
        bool brakeLightRightState = brakeLightRight && brakeLightRight.enabled;
        bool turnSignalLeftState = turnSignalLeft && turnSignalLeft.enabled;
        bool turnSignalRightState = turnSignalRight && turnSignalRight.enabled;
        bool reverseLightState = reverseLight && reverseLight.enabled;

        // Turn all on (check for null to avoid errors)
        if (headlightLeft) headlightLeft.enabled = true;
        if (headlightRight) headlightRight.enabled = true;
        if (tailLightLeft) tailLightLeft.enabled = true;
        if (tailLightRight) tailLightRight.enabled = true;
        if (brakeLightLeft) brakeLightLeft.enabled = true;
        if (brakeLightRight) brakeLightRight.enabled = true;
        if (turnSignalLeft) turnSignalLeft.enabled = true;
        if (turnSignalRight) turnSignalRight.enabled = true;
        if (reverseLight) reverseLight.enabled = true;

        yield return new WaitForSeconds(duration);

        // Restore previous state
        if (headlightLeft) headlightLeft.enabled = headlightLeftState;
        if (headlightRight) headlightRight.enabled = headlightRightState;
        if (tailLightLeft) tailLightLeft.enabled = tailLightLeftState;
        if (tailLightRight) tailLightRight.enabled = tailLightRightState;
        if (brakeLightLeft) brakeLightLeft.enabled = brakeLightLeftState;
        if (brakeLightRight) brakeLightRight.enabled = brakeLightRightState;
        if (turnSignalLeft) turnSignalLeft.enabled = turnSignalLeftState;
        if (turnSignalRight) turnSignalRight.enabled = turnSignalRightState;
        if (reverseLight) reverseLight.enabled = reverseLightState;
    }


    public void SetReverseLight(bool on)
    {
        if (reverseLight != null)
            reverseLight.enabled = on;
    }


    public void ToggleHeadlights()
    {
        headlightsOn = !headlightsOn;
        ToggleHeadlights(headlightsOn);
    }

    public void ToggleLeftSignal()
    {
        leftSignalOn = !leftSignalOn;
        ToggleLeftSignal(leftSignalOn);
    }

    public void ToggleRightSignal()
    {
        rightSignalOn = !rightSignalOn;
        ToggleRightSignal(rightSignalOn);
    }

    public void ToggleHazards()
    {
        hazardsOn = !hazardsOn;
        ToggleHazards(hazardsOn);
    }

    // ---- INTERNAL CONTROL FUNCTIONS ----

    // Headlights
    public void ToggleHeadlights(bool on)
    {
        headlightLeft.enabled = on;
        headlightRight.enabled = on;
        tailLightLeft.enabled = on;
        tailLightRight.enabled = on;
    }

    // Brake Lights
    public void SetBrakeLights(bool on)
    {
        brakeLightLeft.enabled = on;
        brakeLightRight.enabled = on;
    }

    // Turn Signals
    public void ToggleLeftSignal(bool on)
    {
        if (on)
        {
            if (leftSignalCoroutine == null)
                leftSignalCoroutine = StartCoroutine(BlinkLight(turnSignalLeft));
        }
        else
        {
            if (leftSignalCoroutine != null)
            {
                StopCoroutine(leftSignalCoroutine);
                leftSignalCoroutine = null;
            }
            turnSignalLeft.enabled = false;
        }
    }

    // Add these public getters to expose the signal states:
    public bool LeftSignalIsOn()
    {
        return leftSignalOn;
    }

    public bool RightSignalIsOn()
    {
        return rightSignalOn;
    }


    public void ToggleRightSignal(bool on)
    {
        if (on)
        {
            if (rightSignalCoroutine == null)
                rightSignalCoroutine = StartCoroutine(BlinkLight(turnSignalRight));
        }
        else
        {
            if (rightSignalCoroutine != null)
            {
                StopCoroutine(rightSignalCoroutine);
                rightSignalCoroutine = null;
            }
            turnSignalRight.enabled = false;
        }
    }

    // Hazards
    public void ToggleHazards(bool on)
    {
        if (on)
        {
            if (hazardCoroutine == null)
                hazardCoroutine = StartCoroutine(BlinkHazards());
        }
        else
        {
            if (hazardCoroutine != null)
            {
                StopCoroutine(hazardCoroutine);
                hazardCoroutine = null;
            }
            turnSignalLeft.enabled = false;
            turnSignalRight.enabled = false;
        }
    }

    private IEnumerator BlinkLight(Light signalLight)
    {
        while (true)
        {
            signalLight.enabled = !signalLight.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator BlinkHazards()
    {
        while (true)
        {
            bool state = !turnSignalLeft.enabled;
            turnSignalLeft.enabled = state;
            turnSignalRight.enabled = state;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
