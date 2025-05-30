using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CarControls : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Car Engine")]
    public float accelerationForce = 1500f;
    public float brakeForce = 3000f;
    public float presentbrakeForce = 0f;
    public float presentAcceleration = 0f;

    [Header("Car Steering")]
    public float maxSteeringAngle = 30f;
    public float presentSteeringAngle = 0f;

    [Header("UI Pedal Buttons (Assign in Inspector)")]
    public Button acceleratorButton;
    public Button brakeButton;

    [Header("Transmission (Reference)")]
    public GearShiftController gearShiftController;

    [Header("Car Power")]
    public bool carPoweredOn = false; // Default: Off

    [Header("Car cam")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("Car Lights Controller")]
    public CarlightController carlightController; // Assign this in the Inspector

    // Turn signal state
    public bool leftSignalOn { get; private set; } = false;
    public bool rightSignalOn { get; private set; } = false;

    private bool isFirstPerson = false;

    // Pedal state
    private float pedalInput = 0f; // 1 = pressed, 0 = released
    private float brakeInput = 0f; // 1 = pressed, 0 = released

    private void Awake()
    {
        // Connect buttons if assigned
        if (acceleratorButton != null)
        {
            AddButtonEvents(acceleratorButton, OnAcceleratorDown, OnAcceleratorUp);
        }
        if (brakeButton != null)
        {
            AddButtonEvents(brakeButton, OnBrakeDown, OnBrakeUp);
        }
    }

    void Start()
    {
        if (firstPersonCamera != null) firstPersonCamera.gameObject.SetActive(isFirstPerson);
        if (thirdPersonCamera != null) thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
    }

    public void ToggleCarPower()
    {
        carPoweredOn = !carPoweredOn;
        if (carlightController != null)
        {
            carlightController.FlashAllLights(0.3f); // Duration in seconds
            if (!carPoweredOn)
            {
                carlightController.ToggleHeadlights(false);
                carlightController.SetBrakeLights(false);
                carlightController.ToggleLeftSignal(false);
                carlightController.ToggleRightSignal(false);
                carlightController.ToggleHazards(false);
                carlightController.SetReverseLight(false);
            }
        }
    }

    public void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson;
        if (firstPersonCamera != null) firstPersonCamera.gameObject.SetActive(isFirstPerson);
        if (thirdPersonCamera != null) thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
    }

    private void Update()
    {
        if (!carPoweredOn)
            return;
        Movecar();
        CarSteering();
        Applybrake();
        UpdateReverseLight();
    }

    private void Movecar()
    {
        float targetAcceleration = 0f;
        GearShiftController.GearState currentGear = GearShiftController.GearState.Park;
        if (gearShiftController != null)
            currentGear = gearShiftController.GetCurrentGear();

        switch (currentGear)
        {
            case GearShiftController.GearState.Park:
                targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Reverse:
                targetAcceleration = -accelerationForce * pedalInput;
                break;
            case GearShiftController.GearState.Neutral:
                targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Drive:
                targetAcceleration = accelerationForce * pedalInput;
                break;
        }

        presentAcceleration = targetAcceleration;

        frontLeftWheelCollider.motorTorque = presentAcceleration;
        frontRightWheelCollider.motorTorque = presentAcceleration;
        rearLeftWheelCollider.motorTorque = presentAcceleration;
        rearRightWheelCollider.motorTorque = presentAcceleration;
    }

    private void UpdateReverseLight()
    {
        if (carlightController != null && gearShiftController != null)
        {
            bool isReverse = gearShiftController.GetCurrentGear() == GearShiftController.GearState.Reverse;
            carlightController.SetReverseLight(isReverse);
        }
    }

    private void CarSteering()
    {
        presentSteeringAngle = maxSteeringAngle * SimpleInput.GetAxis("Horizontal");
        frontLeftWheelCollider.steerAngle = presentSteeringAngle;
        frontRightWheelCollider.steerAngle = presentSteeringAngle;

        SteeringWheels(frontLeftWheelCollider, frontLeftWheelTransform);
        SteeringWheels(frontRightWheelCollider, frontRightWheelTransform);
        SteeringWheels(rearLeftWheelCollider, rearLeftWheelTransform);
        SteeringWheels(rearRightWheelCollider, rearRightWheelTransform);
    }

    void SteeringWheels(WheelCollider WC, Transform WT)
    {
        Vector3 position;
        Quaternion rotation;
        WC.GetWorldPose(out position, out rotation);
        WT.position = position;
        WT.rotation = rotation;
    }

    private void Applybrake()
    {
        presentbrakeForce = brakeForce * brakeInput;

        frontLeftWheelCollider.brakeTorque = presentbrakeForce;
        frontRightWheelCollider.brakeTorque = presentbrakeForce;
        rearLeftWheelCollider.brakeTorque = presentbrakeForce;
        rearRightWheelCollider.brakeTorque = presentbrakeForce;
    }

    // Methods for UI Button Pedals
    public void OnAcceleratorDown() { pedalInput = 1f; }
    public void OnAcceleratorUp() { pedalInput = 0f; }
    public void OnBrakeDown()
    {
        if (!carPoweredOn) return;
        brakeInput = 1f;
        if (carlightController != null)
            carlightController.SetBrakeLights(true);
    }
    public void OnBrakeUp()
    {
        if (!carPoweredOn) return;
        brakeInput = 0f;
        if (carlightController != null)
            carlightController.SetBrakeLights(false);
    }

    // ----- TURN SIGNAL METHODS -----

    /// <summary>
    /// Turn ON the left signal and turn OFF the right signal.
    /// </summary>
    public void SignalLeft()
    {
        leftSignalOn = true;
        rightSignalOn = false;
        if (carlightController != null)
        {
            carlightController.ToggleLeftSignal(true);
            carlightController.ToggleRightSignal(false);
        }
    }

    /// <summary>
    /// Turn ON the right signal and turn OFF the left signal.
    /// </summary>
    public void SignalRight()
    {
        leftSignalOn = false;
        rightSignalOn = true;
        if (carlightController != null)
        {
            carlightController.ToggleLeftSignal(false);
            carlightController.ToggleRightSignal(true);
        }
    }

    /// <summary>
    /// Turn OFF both signals.
    /// </summary>
    public void SignalOff()
    {
        leftSignalOn = false;
        rightSignalOn = false;
        if (carlightController != null)
        {
            carlightController.ToggleLeftSignal(false);
            carlightController.ToggleRightSignal(false);
        }
    }

    /// <summary>
    /// Toggle left signal (for toggle button UI).
    /// </summary>
    public void ToggleLeftSignal()
    {
        if (!leftSignalOn)
        {
            SignalLeft();
        }
        else
        {
            SignalOff();
        }
    }

    /// <summary>
    /// Toggle right signal (for toggle button UI).
    /// </summary>
    public void ToggleRightSignal()
    {
        if (!rightSignalOn)
        {
            SignalRight();
        }
        else
        {
            SignalOff();
        }
    }

    // --------------------------------------------------

    // Helper to add pointer events to buttons
    private void AddButtonEvents(Button button, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // PointerDown
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((eventData) => { onDown(); });
        trigger.triggers.Add(entryDown);

        // PointerUp
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((eventData) => { onUp(); });
        trigger.triggers.Add(entryUp);
    }

    /// <summary>
    /// Returns the car's current speed (in Unity units/sec).
    /// </summary>
    public float GetCurrentSpeed()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        return rb != null ? rb.velocity.magnitude : 0f;
    }
}
