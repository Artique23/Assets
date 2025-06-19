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
    private Coroutine leftSignalAutoOffCoroutine;
    private Coroutine rightSignalAutoOffCoroutine;

    private bool headlightsOn = false;
    private bool leftSignalOn = false;
    private bool rightSignalOn = false;
    private bool hazardsOn = false;

    void Start()
    {
        headlightsOn = false;
        leftSignalOn = false;
        rightSignalOn = false;
        hazardsOn = false;

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
        bool headlightLeftState = headlightLeft && headlightLeft.enabled;
        bool headlightRightState = headlightRight && headlightRight.enabled;
        bool tailLightLeftState = tailLightLeft && tailLightLeft.enabled;
        bool tailLightRightState = tailLightRight && tailLightRight.enabled;
        bool brakeLightLeftState = brakeLightLeft && brakeLightLeft.enabled;
        bool brakeLightRightState = brakeLightRight && brakeLightRight.enabled;
        bool turnSignalLeftState = turnSignalLeft && turnSignalLeft.enabled;
        bool turnSignalRightState = turnSignalRight && turnSignalRight.enabled;
        bool reverseLightState = reverseLight && reverseLight.enabled;

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

    public void ToggleHeadlights(bool on)
    {
        headlightLeft.enabled = on;
        headlightRight.enabled = on;
        tailLightLeft.enabled = on;
        tailLightRight.enabled = on;
    }

    public void SetBrakeLights(bool on)
    {
        brakeLightLeft.enabled = on;
        brakeLightRight.enabled = on;
    }

    public void ToggleLeftSignal(bool on)
    {
        if (on)
        {
            if (rightSignalOn)
            {
                rightSignalOn = false;
                ToggleRightSignal(false);
            }

            if (leftSignalCoroutine == null)
                leftSignalCoroutine = StartCoroutine(BlinkLight(turnSignalLeft));

            if (leftSignalAutoOffCoroutine != null)
                StopCoroutine(leftSignalAutoOffCoroutine);
            leftSignalAutoOffCoroutine = StartCoroutine(AutoTurnOffLeftSignal());
        }
        else
        {
            if (leftSignalCoroutine != null)
            {
                StopCoroutine(leftSignalCoroutine);
                leftSignalCoroutine = null;
            }
            turnSignalLeft.enabled = false;

            if (leftSignalAutoOffCoroutine != null)
            {
                StopCoroutine(leftSignalAutoOffCoroutine);
                leftSignalAutoOffCoroutine = null;
            }
        }

        leftSignalOn = on;
    }

    public void ToggleRightSignal(bool on)
    {
        if (on)
        {
            if (leftSignalOn)
            {
                leftSignalOn = false;
                ToggleLeftSignal(false);
            }

            if (rightSignalCoroutine == null)
                rightSignalCoroutine = StartCoroutine(BlinkLight(turnSignalRight));

            if (rightSignalAutoOffCoroutine != null)
                StopCoroutine(rightSignalAutoOffCoroutine);
            rightSignalAutoOffCoroutine = StartCoroutine(AutoTurnOffRightSignal());
        }
        else
        {
            if (rightSignalCoroutine != null)
            {
                StopCoroutine(rightSignalCoroutine);
                rightSignalCoroutine = null;
            }
            turnSignalRight.enabled = false;

            if (rightSignalAutoOffCoroutine != null)
            {
                StopCoroutine(rightSignalAutoOffCoroutine);
                rightSignalAutoOffCoroutine = null;
            }
        }

        rightSignalOn = on;
    }

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

    private IEnumerator AutoTurnOffLeftSignal()
    {
        yield return new WaitForSeconds(5f);
        ToggleLeftSignal(false);
    }

    private IEnumerator AutoTurnOffRightSignal()
    {
        yield return new WaitForSeconds(5f);
        ToggleRightSignal(false);
    }

    public bool LeftSignalIsOn()
    {
        return leftSignalOn;
    }

    public bool RightSignalIsOn()
    {
        return rightSignalOn;
    }
}
