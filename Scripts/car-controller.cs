using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarController : MonoBehaviour
{
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
    [SerializeField] private Slider acceleratorPedal;
    [SerializeField] private Slider brakePedal;
    [SerializeField] private TMP_Text gearDisplayText;
    [SerializeField] private Button parkButton;
    [SerializeField] private Button reverseButton;
    [SerializeField] private Button neutralButton;
    [SerializeField] private Button driveButton;

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

        // Set up UI button listeners
        if (parkButton) parkButton.onClick.AddListener(() => SetGearState(GearState.Park));
        if (reverseButton) reverseButton.onClick.AddListener(() => SetGearState(GearState.Reverse));
        if (neutralButton) neutralButton.onClick.AddListener(() => SetGearState(GearState.Neutral));
        if (driveButton) driveButton.onClick.AddListener(() => SetGearState(GearState.Drive));
        
        UpdateGearDisplay();
    }

    private void Update()
    {
        // Get pedal inputs from UI sliders
        if (acceleratorPedal != null)
        {
            currentAccelerationInput = acceleratorPedal.value;
        }
        
        if (brakePedal != null)
        {
            currentBrakeInput = brakePedal.value;
        }

        // Update wheel visuals
        UpdateWheelPoses();
        
        // Update engine sound
        UpdateEngineSound();
    }

    private void FixedUpdate()
    {
        // Apply vehicle physics
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
                // In Park, apply full brakes regardless of pedal input
                brakeTorque = maxBrakeTorque;
                motorTorque = 0f;
                break;
                
            case GearState.Reverse:
                // In Reverse, apply reverse torque based on accelerator
                motorTorque = -motorForceReverse * currentAccelerationInput;
                // Apply brakes if brake pedal is pressed
                brakeTorque = maxBrakeTorque * currentBrakeInput;
                break;
                
            case GearState.Neutral:
                // In Neutral, no motor torque, only brakes
                motorTorque = 0f;
                brakeTorque = maxBrakeTorque * currentBrakeInput;
                break;
                
            case GearState.Drive:
                // In Drive, apply forward torque based on accelerator
                motorTorque = maxMotorTorque * currentAccelerationInput;
                // Apply brakes if brake pedal is pressed
                brakeTorque = maxBrakeTorque * currentBrakeInput;
                break;
        }

        // Apply calculated forces to wheel colliders
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
        
        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;
    }

    private void HandleSteering()
    {
        // Get steering input from horizontal axis (for keyboard/controller input)
        float steeringInput = Input.GetAxis("Horizontal");
        
        // Calculate steering angle
        currentSteeringAngle = maxSteeringAngle * steeringInput;
        
        // Apply steering to front wheels
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
        // Check if the car is moving too fast to change gears
        float currentSpeed = carRigidbody.velocity.magnitude;
        
        // Prevent shifting from/to Park or Reverse if moving too fast (5 units/sec ≈ 18 km/h)
        if ((gear == GearState.Park || gear == GearState.Reverse || currentGear == GearState.Park || currentGear == GearState.Reverse) 
            && currentSpeed > 5f)
        {
            Debug.Log("Cannot change gear: Vehicle is moving too fast!");
            return;
        }

        currentGear = gear;
        
        // Set the parking brake state
        isParkingBrakeEngaged = (gear == GearState.Park);
        
        // Update UI
        UpdateGearDisplay();
        
        Debug.Log("Gear changed to: " + gear.ToString());
    }
    
    private void UpdateGearDisplay()
    {
        if (gearDisplayText != null)
        {
            gearDisplayText.text = currentGear.ToString();
            
            // Highlight the current gear button
            Color activeColor = new Color(0.8f, 1f, 0.8f);
            Color normalColor = Color.white;
            
            if (parkButton) parkButton.GetComponent<Image>().color = (currentGear == GearState.Park) ? activeColor : normalColor;
            if (reverseButton) reverseButton.GetComponent<Image>().color = (currentGear == GearState.Reverse) ? activeColor : normalColor;
            if (neutralButton) neutralButton.GetComponent<Image>().color = (currentGear == GearState.Neutral) ? activeColor : normalColor;
            if (driveButton) driveButton.GetComponent<Image>().color = (currentGear == GearState.Drive) ? activeColor : normalColor;
        }
    }
    
    private void UpdateEngineSound()
    {
        if (engineAudioSource != null)
        {
            // Calculate engine RPM based on wheel rotation speed and acceleration input
            float wheelRPM = Mathf.Abs((frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2);
            float normalizedRPM = Mathf.Clamp01(wheelRPM / 200); // Normalize RPM
            
            // Add acceleration effect to pitch
            float accelerationFactor = currentGear != GearState.Park ? currentAccelerationInput : 0;
            float targetPitch = minEnginePitch + (maxEnginePitch - minEnginePitch) * (normalizedRPM * 0.7f + accelerationFactor * 0.3f);
            
            // Smoothly adjust pitch
            engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 2);
            
            // Adjust volume based on acceleration
            engineAudioSource.volume = 0.4f + accelerationFactor * 0.6f;
        }
    }
}