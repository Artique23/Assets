using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearShiftController : MonoBehaviour
{
    public enum GearState { Park, Reverse, Neutral, Drive }
    public GearState currentGear = GearState.Park;

    [Header("Assign your Gear Shift Slider here")]
    public Slider gearShiftSlider;

    private void Start()
    {
        if (gearShiftSlider != null)
        {
            gearShiftSlider.minValue = 0;
            gearShiftSlider.maxValue = 3;
            gearShiftSlider.wholeNumbers = true;
            gearShiftSlider.onValueChanged.AddListener(OnGearSliderChanged);
            // Ensure gear matches slider at scene start
            SetGearState(Mathf.RoundToInt(gearShiftSlider.value));
        }
    }

    private void OnGearSliderChanged(float value)
    {
        SetGearState(Mathf.RoundToInt(value));
    }

    private void SetGearState(int gearIndex)
    {
        switch (gearIndex)
        {
            case 0: currentGear = GearState.Park; break;
            case 1: currentGear = GearState.Reverse; break;
            case 2: currentGear = GearState.Neutral; break;
            case 3: currentGear = GearState.Drive; break;
        }
    }

    public GearState GetCurrentGear()
    {
        return currentGear;
    }
}
