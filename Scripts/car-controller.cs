using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarUIController carUIController;

    [Header("Car Specifications")]
    [SerializeField] private float maxMotorTorque = 2000f;
    [SerializeField] private float maxBrakeTorque = 3000f;
    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private float motorForceReverse = 1000f;
    [SerializeField] private Transform centerOfMass;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [Header("Driving UI")]
    [SerializeField] private TMP_Text gearDisplayText;

    [Header("Sound")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private float minEnginePitch = 0.5f;
    [SerializeField] private float maxEnginePitch = 2.0f;

    // Vehicle state
    private Rigidbody carRigidbody;
    private float currentSteeringAngle;
    private float currentAccelerationInput;
    private float currentBrakeInput;

    // Gear system
    public enum GearState { Park, Reverse, Neutral, Drive }
    private GearState currentGear = GearState.Park;
    private bool isParkingBrakeEngaged = true;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        if (centerOfMass != null)
        {
            carRigidbody.centerOfMass = centerOfMass.localPosition;
        }

        UpdateGearDisplay();
    }

    private void Update()
    {
        // Get pedal input from CarUIController (button or analog/touch)
        if (carUIController != null)
        {
            currentAccelerationInput = carUIController.GetAcceleratorValue();
            currentBrakeInput = carUIController.GetBrakeValue();
        }

        UpdateWheelPoses();
        UpdateEngineSound();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
    }

    private void HandleMotor()
    {
        float motorTorque = 0f;
        float brakeTorque = 0f;
        switch (currentGear)
        {
            case GearState.Park:
                brakeTorque = maxBrakeTorque;
                motorTorque = 0f;
                break;
            case GearState.Reverse:
                motorTorque = -motorForceReverse * currentAccelerationInput;
                brakeTorque = maxBrakeTorque * currentBrakeInput;
                break;
            case GearState.Neutral:
                motorTorque = 0f;
                brakeTorque = maxBrakeTorque * currentBrakeInput;
                break;
            case GearState.Drive:
                motorTorque = maxMotorTorque * currentAccelerationInput;
                brakeTorque = maxBrakeTorque * currentBrakeInput;
                break;
        }

        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;
    }

    private void HandleSteering()
    {
        float steeringInput = Input.GetAxis("Horizontal");
        currentSteeringAngle = maxSteeringAngle * steeringInput;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPose(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPose(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPose(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelCollider == null || wheelTransform == null) return;
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    public void SetGearState(GearState gear)
    {
        float currentSpeed = carRigidbody.velocity.magnitude;
        if ((gear == GearState.Park || gear == GearState.Reverse || currentGear == GearState.Park || currentGear == GearState.Reverse)
            && currentSpeed > 5f)
        {
            Debug.Log("Cannot change gear: Vehicle is moving too fast!");
            return;
        }
        currentGear = gear;
        isParkingBrakeEngaged = (gear == GearState.Park);
        UpdateGearDisplay();
        Debug.Log("Gear changed to: " + gear.ToString());
    }

    private void UpdateGearDisplay()
    {
        if (gearDisplayText != null)
            gearDisplayText.text = currentGear.ToString();
    }

    private void UpdateEngineSound()
    {
        if (engineAudioSource != null)
        {
            float wheelRPM = Mathf.Abs((frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2);
            float normalizedRPM = Mathf.Clamp01(wheelRPM / 200);
            float accelerationFactor = currentGear != GearState.Park ? currentAccelerationInput : 0;
            float targetPitch = minEnginePitch + (maxEnginePitch - minEnginePitch) * (normalizedRPM * 0.7f + accelerationFactor * 0.3f);
            engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 2);
            engineAudioSource.volume = 0.4f + accelerationFactor * 0.6f;
        }
    }
}
