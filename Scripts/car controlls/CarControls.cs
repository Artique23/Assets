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
    public float speedLimit = 20f; // Speed limit in Unity units/second
    public float accelerationSmoothing = 5f; // Smoothing for acceleration

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
    public Text speedometerText; // Assign in inspector (or TMP_Text if you use TextMeshPro)

    // Turn signal state
    public bool leftSignalOn { get; private set; } = false;
    public bool rightSignalOn { get; private set; } = false;

    private bool isFirstPerson = false;

    // Pedal state
    private float pedalInput = 0f; // 1 = pressed, 0 = released
    private float brakeInput = 0f; // 1 = pressed, 0 = released

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
        UpdateSpeedometer(); // Speedometer update
    }

    // --- NEW: SPEED LIMIT & SMOOTH ACCELERATION ---
    private void Movecar()
    {
        float targetAcceleration = 0f;
        GearShiftController.GearState currentGear = GearShiftController.GearState.Park;
        if (gearShiftController != null)
            currentGear = gearShiftController.GetCurrentGear();

        Rigidbody rb = GetComponent<Rigidbody>();
        float currentSpeed = rb != null ? rb.velocity.magnitude : 0f;

        switch (currentGear)
        {
            case GearShiftController.GearState.Park:
                targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Reverse:
                // Cap reverse speed too
                if (Mathf.Abs(currentSpeed) < speedLimit)
                    targetAcceleration = -accelerationForce * pedalInput;
                else
                    targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Neutral:
                targetAcceleration = 0f;
                break;
            case GearShiftController.GearState.Drive:
                if (currentSpeed < speedLimit)
                    targetAcceleration = accelerationForce * pedalInput;
                else
                    targetAcceleration = 0f;
                break;
        }

        // Smoother acceleration
        currentAcceleration = Mathf.Lerp(currentAcceleration, targetAcceleration, Time.deltaTime * accelerationSmoothing);
        presentAcceleration = currentAcceleration;

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

    // --- IMPROVED: REALISTIC BRAKING ---
    private void Applybrake()
    {
        presentbrakeForce = brakeForce * brakeInput;

        // More realistic braking: add drag while braking
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (brakeInput > 0 && rb.velocity.magnitude > 1f)
            {
                rb.drag = 2f; // You can tweak this value for more/less drag
            }
            else
            {
                rb.drag = 0f; // Reset drag when not braking
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

    // --- NEW: SPEEDOMETER DISPLAY ---
    private void UpdateSpeedometer()
    {
        if (speedometerText != null)
        {
            float speed = GetCurrentSpeed() * 3.6f; // Converts Unity units/sec to km/h (approx if 1 unit = 1 meter)
            speedometerText.text = Mathf.RoundToInt(speed).ToString() + " km/h";
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AutonomousVehicle"))
        {
            // Subtract 50 points when player hits an AI car
            StageScoreManager.Instance.AddPoints(-50);
            tutorialManager.ShowWade("Don't crash into other cars! -50 points");
            

            // Optional: Show a message if you have a tutorial manager reference

            Debug.Log("Player hit an AI car! -50 points");
        }
    }
}
