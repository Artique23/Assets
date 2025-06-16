using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InstructorExpressions : MonoBehaviour
{
    [Header("Instructor Image")]
    public Image instructorImage;  // Drag your existing UI Image here
    
    [Header("Expression Sprites")]
    public Sprite normalFace;      // Default/idle expression
    public Sprite madFace;         // Angry expression
    public Sprite tiredFace;       // Tired/bored expression
    public Sprite heartEyesFace;   // Excited/happy expression
    public Sprite hoorayFace;      // Celebratory/hooray expression
    
    [Header("Expression Durations")]
    [Tooltip("How long each expression stays before returning to normal")]
    public float madFaceDuration = 2.0f;     // Duration for mad face
    public float tiredFaceDuration = 3.0f;   // Duration for tired face
    public float heartEyesDuration = 2.5f;   // Duration for heart eyes
    public float hoorayFaceDuration = 2.0f;  // Duration for hooray face
    
    [Header("Testing Controls")]
    [Tooltip("Enable keyboard shortcuts (Q, W, E, R, T) for testing expressions")]
    public bool enableKeyboardTesting = true;
    
    private Sprite currentExpression;
    private Coroutine resetCoroutine;
    
    void Start()
    {
        // Set default expression when game starts
        if (normalFace != null)
        {
            currentExpression = normalFace;
            instructorImage.sprite = normalFace;
        }
    }
    
    void Update()
    {
        // Only process keyboard input if testing is enabled
        if (enableKeyboardTesting)
        {
            // Q key - Normal face
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Expression: Normal Face");
                SetNormalExpression();
            }
            
            // W key - Mad face
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Expression: Mad Face");
                SetMadExpressionWithTimer();
            }
            
            // E key - Tired face
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Expression: Tired Face");
                SetTiredExpressionWithTimer();
            }
            
            // R key - Heart Eyes face
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Expression: Heart Eyes Face");
                SetHeartEyesExpressionWithTimer();
            }
            
            // T key - Hooray face
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Expression: Hooray Face");
                SetHoorayExpressionWithTimer();
            }
        }
    }
    
    // Basic expression methods (without timers)
    public void SetNormalExpression()
    {
        // Cancel any existing timer
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
        
        SetExpression(normalFace);
    }
    
    public void SetMadExpression()
    {
        // Permanent change without timer
        SetExpression(madFace);
    }
    
    public void SetTiredExpression()
    {
        // Permanent change without timer
        SetExpression(tiredFace);
    }
    
    public void SetHeartEyesExpression()
    {
        // Permanent change without timer
        SetExpression(heartEyesFace);
    }
    
    public void SetHoorayExpression()
    {
        // Permanent change without timer
        SetExpression(hoorayFace);
    }
    
    // Expression methods WITH timers
    public void SetMadExpressionWithTimer(float customDuration = -1)
    {
        float duration = customDuration > 0 ? customDuration : madFaceDuration;
        SetTemporaryExpression(madFace, duration);
    }
    
    public void SetTiredExpressionWithTimer(float customDuration = -1)
    {
        float duration = customDuration > 0 ? customDuration : tiredFaceDuration;
        SetTemporaryExpression(tiredFace, duration);
    }
    
    public void SetHeartEyesExpressionWithTimer(float customDuration = -1)
    {
        float duration = customDuration > 0 ? customDuration : heartEyesDuration;
        SetTemporaryExpression(heartEyesFace, duration);
    }
    
    public void SetHoorayExpressionWithTimer(float customDuration = -1)
    {
        float duration = customDuration > 0 ? customDuration : hoorayFaceDuration;
        SetTemporaryExpression(hoorayFace, duration);
    }
    
    // Set expression with automatic reset
    public void SetTemporaryExpression(Sprite expressionSprite, float duration)
    {
        SetExpression(expressionSprite);
        
        // Cancel any existing reset coroutine
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);
            
        // Start new reset timer
        resetCoroutine = StartCoroutine(ResetExpressionAfterDelay(duration));
    }
    
    // Helper to set any expression
    public void SetExpression(Sprite newExpression)
    {
        if (newExpression != null && instructorImage != null)
        {
            currentExpression = newExpression;
            instructorImage.sprite = newExpression;
        }
    }
    
    // Reset to normal after delay
    private IEnumerator ResetExpressionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetExpression(normalFace);
        resetCoroutine = null;
    }
}