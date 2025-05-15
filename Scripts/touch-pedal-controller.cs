using UnityEngine;

public class TouchPedalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarUIController carUIController;
    [SerializeField] private RectTransform acceleratorPedalArea;
    [SerializeField] private RectTransform brakePedalArea;
    
    [Header("Pedal Behavior")]
    [SerializeField] private float sensitivityMultiplier = 1.0f;
    [SerializeField] private float pressHoldIncrement = 0.05f;
    [SerializeField] private float pressHoldDelay = 0.1f;
    
    // Touch tracking
    private int acceleratorTouchId = -1;
    private int brakeTouchId = -1;
    private Vector2 acceleratorTouchStartPos;
    private Vector2 brakeTouchStartPos;
    private float lastAcceleratorHoldTime = 0f;
    private float lastBrakeHoldTime = 0f;
    
    private void Update()
    {
        HandleTouchInput();
        HandleKeyboardInput(); // For debugging in editor
    }
    
    private void HandleTouchInput()
    {
        // Process all touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];
            Vector2 touchPos = touch.position;
            
            // Handle touch began
            if (touch.phase == TouchPhase.Began)
            {
                // Check if touch is on accelerator
                if (RectTransformUtility.RectangleContainsScreenPoint(acceleratorPedalArea, touchPos) && acceleratorTouchId == -1)
                {
                    acceleratorTouchId = touch.fingerId;
                    acceleratorTouchStartPos = touchPos;
                    carUIController.OnAcceleratorDown();
                }
                // Check if touch is on brake
                else if (RectTransformUtility.RectangleContainsScreenPoint(brakePedalArea, touchPos) && brakeTouchId == -1)
                {
                    brakeTouchId = touch.fingerId;
                    brakeTouchStartPos = touchPos;
                    carUIController.OnBrakeDown();
                }
            }
            // Handle touch moved
            else if (touch.phase == TouchPhase.Moved)
            {
                // Process accelerator movement
                if (touch.fingerId == acceleratorTouchId)
                {
                    float deltaY = touchPos.y - acceleratorTouchStartPos.y;
                    carUIController.OnAcceleratorTouchDelta(-deltaY * sensitivityMultiplier);
                    acceleratorTouchStartPos = touchPos;
                }
                // Process brake movement
                else if (touch.fingerId == brakeTouchId)
                {
                    float deltaY = touchPos.y - brakeTouchStartPos.y;
                    carUIController.OnBrakeTouchDelta(-deltaY * sensitivityMultiplier);
                    brakeTouchStartPos = touchPos;
                }
            }
            // Handle touch stationary
            else if (touch.phase == TouchPhase.Stationary)
            {
                // For pedals, we can implement a gradual "press harder" mechanism
                if (touch.fingerId == acceleratorTouchId)
                {
                    if (Time.time > lastAcceleratorHoldTime + pressHoldDelay)
                    {
                        carUIController.OnAcceleratorTouchDelta(-pressHoldIncrement * 100);
                        lastAcceleratorHoldTime = Time.time;
                    }
                }
                else if (touch.fingerId == brakeTouchId)
                {
                    if (Time.time > lastBrakeHoldTime + pressHoldDelay)
                    {
                        carUIController.OnBrakeTouchDelta(-pressHoldIncrement * 100);
                        lastBrakeHoldTime = Time.time;
                    }
                }
            }
            // Handle touch ended or cancelled
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == acceleratorTouchId)
                {
                    acceleratorTouchId = -1;
                    carUIController.OnAcceleratorUp();
                }
                else if (touch.fingerId == brakeTouchId)
                {
                    brakeTouchId = -1;
                    carUIController.OnBrakeUp();
                }
            }
        }
    }
    
    private void HandleKeyboardInput()
    {
        // Debug inputs for testing in Unity Editor
        if (Input.GetKey(KeyCode.UpArrow))
        {
            carUIController.SetAcceleratorValue(Mathf.Min(1f, carUIController.GetComponent<Slider>().value + 0.01f));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            carUIController.SetBrakeValue(Mathf.Min(1f, carUIController.GetComponent<Slider>().value + 0.01f));
        }
        
        // For gear shifting with keyboard
        if (Input.GetKeyDown(KeyCode.P)) carUIController.SetGearPark();
        if (Input.GetKeyDown(KeyCode.R)) carUIController.SetGearReverse();
        if (Input.GetKeyDown(KeyCode.N)) carUIController.SetGearNeutral();
        if (Input.GetKeyDown(KeyCode.D)) carUIController.SetGearDrive();
    }
}