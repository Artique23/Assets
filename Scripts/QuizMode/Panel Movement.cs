using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelMovement : MonoBehaviour
{
    [Header("Continuous Movement Settings")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float movementAmount = 10f; // How much the panel moves
    [SerializeField] private float movementDuration = 2f; // Time for one complete movement cycle
    [SerializeField] private Ease movementEase = Ease.InOutSine; // Smooth sine wave motion
    
    [Header("Optional Rotation")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private float rotationAmount = 0.5f; // Very subtle rotation
    [SerializeField] private float rotationDuration = 3f; // Slightly different timing creates a more organic feel
    
    [Header("Optional Scale")]
    [SerializeField] private bool enableScale = true;
    [SerializeField] private float scaleAmount = 0.03f; // Just 3% size change
    [SerializeField] private float scaleDuration = 2.5f;
    
    private Vector2 startPosition;
    private Sequence movementSequence;
    
    void Start()
    {
        // If panelRect not assigned, use this object's RectTransform
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();
            
        // Store initial position
        startPosition = panelRect.anchoredPosition;
        
        // Start the continuous animation
        StartContinuousMovement();
    }
    
    public void StartContinuousMovement()
    {
        // Kill any existing animations
        if (movementSequence != null)
            movementSequence.Kill();
            
        // Create a new animation sequence
        movementSequence = DOTween.Sequence();
        
        // Add horizontal movement (left to right and back)
        movementSequence.Append(
            panelRect.DOAnchorPosX(startPosition.x + movementAmount, movementDuration / 2)
            .SetEase(movementEase)
        );
        movementSequence.Append(
            panelRect.DOAnchorPosX(startPosition.x - movementAmount, movementDuration)
            .SetEase(movementEase)
        );
        movementSequence.Append(
            panelRect.DOAnchorPosX(startPosition.x, movementDuration / 2)
            .SetEase(movementEase)
        );
        
        // Add vertical movement (up and down) with slight offset timing
        movementSequence.Join(
            panelRect.DOAnchorPosY(startPosition.y + movementAmount * 0.7f, movementDuration * 0.6f)
            .SetEase(movementEase)
            .SetDelay(0.3f) // Offset to create more organic movement
        );
        movementSequence.Append(
            panelRect.DOAnchorPosY(startPosition.y - movementAmount * 0.7f, movementDuration * 1.2f)
            .SetEase(movementEase)
        );
        movementSequence.Append(
            panelRect.DOAnchorPosY(startPosition.y, movementDuration * 0.6f)
            .SetEase(movementEase)
        );
        
        // Add subtle rotation if enabled
        if (enableRotation) {
            Sequence rotationSequence = DOTween.Sequence();
            rotationSequence.Append(
                panelRect.DORotate(new Vector3(0, 0, rotationAmount), rotationDuration / 2)
                .SetEase(movementEase)
            );
            rotationSequence.Append(
                panelRect.DORotate(new Vector3(0, 0, -rotationAmount), rotationDuration)
                .SetEase(movementEase)
            );
            rotationSequence.Append(
                panelRect.DORotate(Vector3.zero, rotationDuration / 2)
                .SetEase(movementEase)
            );
            
            // Add rotation to main sequence
            movementSequence.Join(rotationSequence);
        }
        
        // Add subtle scaling if enabled
        if (enableScale) {
            Vector3 baseScale = panelRect.localScale;
            Sequence scaleSequence = DOTween.Sequence();
            scaleSequence.Append(
                panelRect.DOScale(baseScale * (1 + scaleAmount), scaleDuration / 2)
                .SetEase(movementEase)
            );
            scaleSequence.Append(
                panelRect.DOScale(baseScale * (1 - scaleAmount * 0.5f), scaleDuration)
                .SetEase(movementEase)
            );
            scaleSequence.Append(
                panelRect.DOScale(baseScale, scaleDuration / 2)
                .SetEase(movementEase)
            );
            
            // Add scaling to main sequence
            movementSequence.Join(scaleSequence);
        }
        
        // Set the sequence to loop infinitely
        movementSequence.SetLoops(-1, LoopType.Restart);
    }
    
    public void StopMovement()
    {
        // Stop the animation and return to original position
        if (movementSequence != null) {
            movementSequence.Kill();
            
            // Return to original position
            panelRect.DOAnchorPos(startPosition, 0.5f).SetEase(Ease.OutQuad);
            panelRect.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuad);
            panelRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
        }
    }
    
    void OnDisable()
    {
        // Clean up animations when object is disabled
        if (movementSequence != null)
            movementSequence.Kill();
    }
}