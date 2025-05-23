using UnityEngine;
using UnityEngine.UI;

public class CarUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarController carController;

    [Header("Pedals (Optional Buttons, for Button UI)")]
    [SerializeField] private Button acceleratorButton;
    [SerializeField] private Button brakeButton;

    [Header("Gear Shift")]
    [SerializeField] private Slider gearShiftSlider;

    private float acceleratorValue = 0f;
    private float brakeValue = 0f;

    private void Start()
    {
        if (gearShiftSlider != null)
            gearShiftSlider.onValueChanged.AddListener(OnGearSliderChanged);

        // Button hooks are optional: only if using UI Buttons, set via inspector or code!
        if (acceleratorButton != null)
        {
            // Setup buttons via EventTrigger or add listeners in inspector.
        }
        if (brakeButton != null)
        {
            // Setup buttons via EventTrigger or add listeners in inspector.
        }
    }

    // ----- PEDAL INTERFACE -----
    // These work for BUTTONS or Touch
    public void OnAcceleratorDown() { acceleratorValue = 1f; }
    public void OnAcceleratorUp() { acceleratorValue = 0f; }
    public void OnBrakeDown() { brakeValue = 1f; }
    public void OnBrakeUp() { brakeValue = 0f; }

    // These allow analog/touch pressure
    public void SetAcceleratorValue(float value) { acceleratorValue = Mathf.Clamp01(value); }
    public void SetBrakeValue(float value) { brakeValue = Mathf.Clamp01(value); }

    // Touch delta events from TouchPedalController
    public void OnAcceleratorTouchDelta(float delta)
    {
        SetAcceleratorValue(acceleratorValue + delta * 0.01f);
    }
    public void OnBrakeTouchDelta(float delta)
    {
        SetBrakeValue(brakeValue + delta * 0.01f);
    }

    // --- CarController reads these ---
    public float GetAcceleratorValue() => acceleratorValue;
    public float GetBrakeValue() => brakeValue;

    // ----- GEAR SHIFT -----
    private void OnGearSliderChanged(float value)
    {
        int gearIndex = Mathf.RoundToInt(value);
        switch (gearIndex)
        {
            case 0: carController.SetGearState(CarController.GearState.Park); break;
            case 1: carController.SetGearState(CarController.GearState.Reverse); break;
            case 2: carController.SetGearState(CarController.GearState.Neutral); break;
            case 3: carController.SetGearState(CarController.GearState.Drive); break;
        }
    }

    // --- For keyboard testing (optional) ---
    public void SetGearPark()    { carController.SetGearState(CarController.GearState.Park); }
    public void SetGearReverse() { carController.SetGearState(CarController.GearState.Reverse); }
    public void SetGearNeutral() { carController.SetGearState(CarController.GearState.Neutral); }
    public void SetGearDrive()   { carController.SetGearState(CarController.GearState.Drive); }
}
