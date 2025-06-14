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
    
    [Header("Animation Settings")]
    public float transitionSpeed = 0.2f;  // How fast expressions change
    public float defaultDuration = 2.0f;  // How long temporary expressions last
    
    [Header("Testing Controls")]
    [Tooltip("Enable keyboard shortcuts (Q, W, E, R) for testing expressions")]
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
                SetMadExpression();
            }
            
            // E key - Tired face
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Expression: Tired Face");
                SetTiredExpression();
            }
            
            // R key - Heart Eyes face
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Expression: Heart Eyes Face");
                SetHeartEyesExpression();
            }
        }
    }
    
    // Basic expression methods
    public void SetNormalExpression()
    {
        SetExpression(normalFace);
    }
    
    public void SetMadExpression()
    {
        SetExpression(madFace);
    }
    
    public void SetTiredExpression()
    {
        SetExpression(tiredFace);
    }
    
    public void SetHeartEyesExpression()
    {
        SetExpression(heartEyesFace);
    }
    
    // Set expression with automatic reset
    public void SetTemporaryExpression(Sprite expressionSprite, float duration = -1)
    {
        if (duration < 0) duration = defaultDuration;
        
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
        SetNormalExpression();
        resetCoroutine = null;
    }
    
    // Shorthand methods for temporary expressions
    public void ShowMadExpression(float duration = -1)
    {
        SetTemporaryExpression(madFace, duration);
    }
    
    public void ShowTiredExpression(float duration = -1)
    {
        SetTemporaryExpression(tiredFace, duration);
    }
    
    public void ShowHeartEyesExpression(float duration = -1)
    {
        SetTemporaryExpression(heartEyesFace, duration);
    }
}