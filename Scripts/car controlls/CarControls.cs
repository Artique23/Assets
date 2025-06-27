using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // For TMP_Text speedometer

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
    public float brakeForce = 8000f;
    public float speedLimit = 10; // 60 km/h
    public float accelerationSmoothing = 5f;

    public float presentbrakeForce = 0f;
    public float presentAcceleration = 0f;

    private float currentAcceleration = 0f; // For smoothing

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

    [Header("UI")]
    public TMP_Text speedometerText; // Assign in inspector

    // Turn signal state
    public bool leftSignalOn { get; private set; } = false;
    public bool rightSignalOn { get; private set; } = false;

    private bool isFirstPerson = false;

    // Pedal state
    public float pedalInput = 0f; // 1 = pressed, 0 = released
    public float brakeInput = 0f; // 1 = pressed, 0 = released

    public float currentUISpeedKmh { get; private set; } = 0f;

    public StageBaseManager stageBaseManager; 

    public Stage1TutorialManager tutorialManager; // Drag Wade's manager in the inspector

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
        UpdateSpeedometer();
        ClampSpeed(); // Hard speed cap
    }

    // SPEED LIMIT & SMOOTH ACCELERATION
    private void Movecar()
    {
        float targetAcceleration = 0f;
        GearShiftController.GearState currentGear = GearShiftController.GearState.Park;
        if (gearShiftController != null)
            currentGear = gearShiftController.GetCurrentGear();

        Rigidbody rb = GetComponent<Rigidbody>();
        float currentSpeed = rb != null ? rb.velocity.magnitude : 0f;

        bool atSpeedLimit = currentSpeed >= speedLimit;

        switch (currentGear)
        {
            case GearShiftController.GearState.Park:
            case GearShiftController.GearState.Neutral:
                targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Reverse:
                if (Mathf.Abs(currentSpeed) < speedLimit)
                    targetAcceleration = -accelerationForce * pedalInput;
                else
                    targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Drive:
                // Only allow positive torque if under the limit
                if (currentSpeed < speedLimit || pedalInput < 0.01f)
                    targetAcceleration = accelerationForce * pedalInput;
                else
                    targetAcceleration = 0f;
                break;
        }

        // Smoother acceleration
        // In Movecar()
        currentAcceleration = Mathf.Lerp(currentAcceleration, targetAcceleration, Time.deltaTime * (accelerationSmoothing * 2f));

        presentAcceleration = currentAcceleration;

        // Apply only to front wheels for FWD
        frontLeftWheelCollider.motorTorque = presentAcceleration;
        frontRightWheelCollider.motorTorque = presentAcceleration;

        // Remove from rear
        rearLeftWheelCollider.motorTorque = 0f;
        rearRightWheelCollider.motorTorque = 0f;

    }


    private void ClampSpeed()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            float maxSpeed = speedLimit;
            float currentSpeed = rb.velocity.magnitude;

            // Hard clamp the velocity
            if (currentSpeed > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;

                // Optionally, apply a little extra drag at the limit
                rb.drag = 2f; // or higher if you want a stronger slowdown
            }
            else
            {
                // Reset to normal drag (0 is default unless you want a little rolling resistance)
                rb.drag = 0f;
            }
        }
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

    // IMPROVED: REALISTIC BRAKING
    private void Applybrake()
    {
        presentbrakeForce = brakeForce * brakeInput;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (brakeInput > 0 && rb.velocity.magnitude > 0.5f)
            {
                rb.drag = 5f; // Tweak as needed for instant response
            }
            else
            {
                rb.drag = 0f;
            }
        }

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

    // SPEEDOMETER DISPLAY
    private void UpdateSpeedometer()
    {
        if (speedometerText != null)
        {
            float speed = GetCurrentSpeed() * 3.6f; // Convert to km/h
            currentUISpeedKmh = speed;              // <== STORE this value for others to read
            speedometerText.text = Mathf.RoundToInt(speed).ToString() + " <size=12>km/h</size>";
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AutonomousVehicle"))
        {
            StageScoreManager.Instance.AddPoints(-50);
            if (stageBaseManager != null)
                stageBaseManager.ShowWade("Don't crash into other cars! -50 points");
        }
        else if (collision.gameObject.CompareTag("Environment"))
        {
            StageScoreManager.Instance.AddPoints(-20);
                stageBaseManager.ShowWade("Careful! You hit the environment! -20 points");
        }
    }
}
