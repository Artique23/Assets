using UnityEngine;
using UnityEngine.UI;

public class CarUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarController carController;
    
    [Header("Pedals")]
    [SerializeField] private Slider acceleratorPedal;
    [SerializeField] private Slider brakePedal;
    
    [Header("UI Elements")]
    [SerializeField] private RectTransform acceleratorPedalPressPoint;
    [SerializeField] private RectTransform brakePedalPressPoint;
    [SerializeField] private float pedalPressDepth = 50f;
    
    [Header("Auto-Return Settings")]
    [SerializeField] private float acceleratorReturnSpeed = 2.0f;
    [SerializeField] private float brakeReturnSpeed = 1.5f;
    
    // Touch state tracking
    private bool isAcceleratorPressed = false;
    private bool isBrakePressed = false;
    
    private void Update()
    {
        // Handle pedal auto-return when released
        if (!isAcceleratorPressed && acceleratorPedal.value > 0)
        {
            acceleratorPedal.value = Mathf.Max(0, acceleratorPedal.value - acceleratorReturnSpeed * Time.deltaTime);
        }
        
        if (!isBrakePressed && brakePedal.value > 0)
        {
            brakePedal.value = Mathf.Max(0, brakePedal.value - brakeReturnSpeed * Time.deltaTime);
        }
        
        // Animate the pedals based on values
        if (acceleratorPedalPressPoint != null)
        {
            Vector3 acceleratorPos = acceleratorPedalPressPoint.localPosition;
            acceleratorPos.y = -pedalPressDepth * acceleratorPedal.value;
            acceleratorPedalPressPoint.localPosition = acceleratorPos;
        }
        
        if (brakePedalPressPoint != null)
        {
            Vector3 brakePos = brakePedalPressPoint.localPosition;
            brakePos.y = -pedalPressDepth * brakePedal.value;
            brakePedalPressPoint.localPosition = brakePos;
        }
    }
    
    // Accelerator pedal methods for UI buttons
    public void OnAcceleratorDown()
    {
        isAcceleratorPressed = true;
    }
    
    public void OnAcceleratorUp()
    {
        isAcceleratorPressed = false;
    }
    
    public void SetAcceleratorValue(float value)
    {
        acceleratorPedal.value = value;
    }
    
    // Brake pedal methods for UI buttons
    public void OnBrakeDown()
    {
        isBrakePressed = true;
    }
    
    public void OnBrakeUp()
    {
        isBrakePressed = false;
    }
    
    public void SetBrakeValue(float value)
    {
        brakePedal.value = value;
    }
    
    // Methods to call on touch events for pedals
    public void OnAcceleratorTouchDelta(float delta)
    {
        if (isAcceleratorPressed)
        {
            acceleratorPedal.value = Mathf.Clamp01(acceleratorPedal.value + delta * 0.01f);
        }
    }
    
    public void OnBrakeTouchDelta(float delta)
    {
        if (isBrakePressed)
        {
            brakePedal.value = Mathf.Clamp01(brakePedal.value + delta * 0.01f);
        }
    }
    
    // Gear control methods to call from UI buttons
    public void SetGearPark()
    {
        carController.SetGearState(CarController.GearState.Park);
    }
    
    public void SetGearReverse()
    {
        carController.SetGearState(CarController.GearState.Reverse);
    }
    
    public void SetGearNeutral()
    {
        carController.SetGearState(CarController.GearState.Neutral);
    }
    
    public void SetGearDrive()
    {
        carController.SetGearState(CarController.GearState.Drive);
    }
}