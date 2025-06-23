using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ParkingZone : MonoBehaviour
{
    // With Ending sumarry
    public StageBaseManager stageBaseManager;
    public GameObject highlightVisual; // Assign in Inspector
    public float minParkTime = 2.0f;
    public float maxParkSpeed = 0.5f;
    private float parkedTimer = 0f;
    private bool playerInZone = false;
    private Rigidbody playerRb;

    public LevelLoader levelLoader; // Reference to LevelLoader for scene management

    private bool highlightActive = false;

    public GameObject finalGameMarker; // Assign in Inspector: the marker GameObject
    private bool markerGone = false;

    // For pulsing effect
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;
    private Vector3 baseScale;

    // For emission (optional)
    public Renderer highlightRenderer; // assign in Inspector if using emission
    public Color glowColor = Color.yellow;
    public float emissionPulseSpeed = 2f;
    public float emissionMin = 0.5f;
    public float emissionMax = 2f;

    public bool canCheckParking = false;

    [Header("Win Panel")]

    public Canvas winPanelCanvas; // Assign in Inspector
    public GameObject winPanel;       // Win UI panel
    public TextMeshProUGUI scoreText; // Score text
    public TextMeshProUGUI label;     // Feedback label
    public GameObject starContainersShadow; // Star containers shadow
    public GameObject star1;          // First star
    public GameObject star2;          // Second star
    public GameObject star3;          // Third star
    public GameObject retryButton;    // Retry button
    public GameObject homeButton;     // Home button
    

    [Header("Win Panel Animation Settings")]
    public Vector2 winPanelStartPosition = new Vector2(0, 500); // Start position off-screen
    public Vector2 winPanelFinalPosition = new Vector2(0, 0);   // Final centered position
    public float slideDuration = 0.7f;                          // How long the panel takes to slide in
    public float elementFadeDuration = 0.5f;                    // How long each element takes to fade in
    public float delayBetweenElements = 0.2f;                   // Delay between each element fading in
    public Ease slideEaseType = Ease.OutBack;                   // Easing for the panel slide
    public Ease fadeEaseType = Ease.OutQuad;                    // Easing for fading elements

    [Header("Star Thresholds")]
    public int star1Threshold = 100; // Score needed for first star
    public int star2Threshold = 200; // Score needed for second star
    public int star3Threshold = 300; // Score needed for third star

    [Header("Testing Controls")]
    [Tooltip("Enable the Z key to test the win panel")]
    public bool enableTestKey = true;

    [Header("Background Darkening")]
    public GameObject darkeningPanel;         // Assign in inspector - this is the panel that darkens the background
    public float darkeningAlpha = 0.7f;       // How dark the background gets (0-1)
    public float darkeningFadeDuration = 0.5f; // How quickly the darkening fades in

    [Header("Canvas Sorting")]
    public Canvas driversUICanvas; // Assign the drivers UI canvas in Inspector
    public int normalWinPanelSortOrder = 5; // Default sort order when showing win panel
    public int normalDriversUISortOrder = 1; // Default sort order for drivers UI
    private int originalDriversUISortOrder; // To store the original order

    // Add this field to track if Z has been pressed
    private bool zKeyWasPressed = false;

    void Start()
    {
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        if (highlightVisual != null)
        {
            highlightVisual.SetActive(false);
            baseScale = highlightVisual.transform.localScale;
        }

        // Initialize the win panel components
        InitializeWinPanel();

        // Hide win panel at start
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    // Initialize win panel components
    private void InitializeWinPanel()
    {
        if (winPanel != null)
        {
            // Make sure all elements have CanvasGroups
            if (scoreText != null && scoreText.gameObject.GetComponent<CanvasGroup>() == null)
                scoreText.gameObject.AddComponent<CanvasGroup>();

            if (label != null && label.gameObject.GetComponent<CanvasGroup>() == null)
                label.gameObject.AddComponent<CanvasGroup>();

            if (starContainersShadow != null && starContainersShadow.GetComponent<CanvasGroup>() == null)
                starContainersShadow.AddComponent<CanvasGroup>();

            if (star1 != null && star1.GetComponent<CanvasGroup>() == null)
                star1.AddComponent<CanvasGroup>();

            if (star2 != null && star2.GetComponent<CanvasGroup>() == null)
                star2.AddComponent<CanvasGroup>();

            if (star3 != null && star3.GetComponent<CanvasGroup>() == null)
                star3.AddComponent<CanvasGroup>();

            if (retryButton != null && retryButton.GetComponent<CanvasGroup>() == null)
                retryButton.AddComponent<CanvasGroup>();

            if (homeButton != null && homeButton.GetComponent<CanvasGroup>() == null)
                homeButton.AddComponent<CanvasGroup>();
        }
    }

    // For testing - call this from inspector
    public void TestWinPanel()
    {
        ShowWinPanel();
    }

    public void ActivateParkingHighlight()
    {
        if (highlightVisual != null)
        {
            highlightVisual.SetActive(true);
            highlightActive = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerRb = other.attachedRigidbody;
            parkedTimer = 0f;

            // Show highlight if not already active
            if (highlightVisual != null && !highlightActive)
            {
                highlightVisual.SetActive(true);
                highlightActive = true;
            }

            ShowDialogAutoHide("Park your car inside the highlighted space and stop.", 1f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            parkedTimer = 0f;
            playerRb = null;
            ShowDialogAutoHide("Try again! Park fully in the space and come to a stop.", 1f);
        }
    }

    void Update()
    {
        finalGameMarker = markerGone ? null : finalGameMarker;
        // Show highlight when marker is destroyed/inactive, even if player hasn't entered yet
        if (!markerGone && (finalGameMarker == null || !finalGameMarker.activeSelf))
        {
            markerGone = true;
            if (highlightVisual != null)
            {
                highlightVisual.SetActive(true);
                highlightActive = true;
            }
            ShowDialogAutoHide("Proceed to the highlighted parking spot!", 1f);
        }

        // Pulsing effect (scale)
        if (highlightVisual != null && highlightVisual.activeSelf)
        {
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            highlightVisual.transform.localScale = baseScale * scale;

            // Optional: Emission pulse for glowing
            if (highlightRenderer != null)
            {
                float emission = Mathf.Lerp(emissionMin, emissionMax, (Mathf.Sin(Time.time * emissionPulseSpeed) + 1f) / 2f);
                highlightRenderer.material.SetColor("_EmissionColor", glowColor * emission);
            }
        }

        // Parking logic
        if (playerInZone && playerRb != null)
        {
            if (playerRb.velocity.magnitude < maxParkSpeed)
            {
                parkedTimer += Time.deltaTime;
                if (parkedTimer >= minParkTime)
                {
                    // Success!
                    StageScoreManager.Instance.AddPoints(1000);

                    // Show win panel!
                    ShowWinPanel();

                    if (highlightVisual != null)
                        highlightVisual.SetActive(false);
                    enabled = false;
                }
            }
            else
            {
                parkedTimer = 0f;
            }
        }

        // Test key to show win panel (press Z)
        // Simple Z key handling without complex conditions
        if (Input.GetKeyDown(KeyCode.Z) && !zKeyWasPressed)
        {
            Debug.Log("Z KEY PRESSED!");
            zKeyWasPressed = true;
            
            // Force StageScoreManager to use test score
            Stage1TutorialManager tutorialManager = FindObjectOfType<Stage1TutorialManager>();
            if (tutorialManager != null && tutorialManager.testMode)
            {
                Debug.Log("Using test score: " + tutorialManager.testScore);
                StageScoreManager.Instance.SetPointsForTesting(tutorialManager.testScore);
            }
            
            // Directly show win panel without additional conditions
            ShowWinPanel();
        }

        // Allow R key to reset the test
        if (Input.GetKeyDown(KeyCode.R) && zKeyWasPressed)
        {
            Debug.Log("R key pressed - resetting test");
            zKeyWasPressed = false;
            
            // Hide panels if they're active
            if (winPanel != null) winPanel.SetActive(false);
            if (darkeningPanel != null) darkeningPanel.SetActive(false);
            
            // Restore canvas order
            if (driversUICanvas != null)
                driversUICanvas.sortingOrder = originalDriversUISortOrder;
        }
    }

    // Show dialog and auto-hide after duration
    void ShowDialogAutoHide(string msg, float delay)
    {
        if (stageBaseManager != null)
        {
            stageBaseManager.ShowWade(msg);
            StopAllCoroutines(); // Prevent multiple overlapping hides
            StartCoroutine(HideDialogAfter(delay));
        }
    }

    IEnumerator HideDialogAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        stageBaseManager.HideWade();
    }

    // WIN UI logic with animation
    void ShowWinPanel()
    {
        if (winPanel == null) return;

        // Activate panel but hide elements
        winPanel.SetActive(true);
        
        // Store original drivers UI sort order and bring win panel to front
        if (driversUICanvas != null && winPanelCanvas != null)
        {
            originalDriversUISortOrder = driversUICanvas.sortingOrder;
            
            // Make win panel canvas higher than drivers UI
            winPanelCanvas.sortingOrder = normalWinPanelSortOrder;
            driversUICanvas.sortingOrder = normalDriversUISortOrder;
            
            Debug.Log($"Canvas sorting orders - Win Panel: {winPanelCanvas.sortingOrder}, Drivers UI: {driversUICanvas.sortingOrder}");
        }
        
        // Activate and fade in the darkening panel first
        if (darkeningPanel != null)
        {
            darkeningPanel.SetActive(true);
            
            // Make sure it has a CanvasGroup
            CanvasGroup darkeningGroup = darkeningPanel.GetComponent<CanvasGroup>();
            if (darkeningGroup == null)
                darkeningGroup = darkeningPanel.AddComponent<CanvasGroup>();
            
            // Start it invisible
            darkeningGroup.alpha = 0;
            
            // Fade it in
            darkeningGroup.DOFade(darkeningAlpha, darkeningFadeDuration)
                .SetEase(fadeEaseType);
        }

        // Get score
        int score = StageScoreManager.Instance.GetPoints();

        // Update text elements
        if (scoreText != null)
            scoreText.text = "Total Score: " + score;

        // Set label text based on score
        if (label != null)
        {
            if (score >= star3Threshold)
                label.text = "Excellent!" + "\nPerfect Parking!";
            else if (score >= star2Threshold)
                label.text = "Great Job!" + "\nWell Done!";
            else if (score >= star1Threshold)
                label.text = "Good Work!" + "\nYou did it!";
            else
                label.text = "Not Bad!" + "\nKeep Practicing!";
        }

        // Hide individual elements initially
        SetElementAlpha(scoreText?.gameObject, 0);
        SetElementAlpha(label?.gameObject, 0);
        SetElementAlpha(starContainersShadow, 0);
        SetElementAlpha(star1, 0);
        SetElementAlpha(star2, 0);
        SetElementAlpha(star3, 0);
        SetElementAlpha(retryButton, 0);
        SetElementAlpha(homeButton, 0);

        // Position the panel at the start position
        RectTransform panelRect = winPanel.GetComponent<RectTransform>();
        if (panelRect != null)
            panelRect.anchoredPosition = winPanelStartPosition;

        // Start the animation sequence
        StartCoroutine(AnimateWinPanel(score));
    }

    // Helper method to set alpha for UI elements
    private void SetElementAlpha(GameObject element, float alpha)
    {
        if (element == null) return;

        // Try to get CanvasGroup, add one if not present
        CanvasGroup group = element.GetComponent<CanvasGroup>();
        if (group == null)
            group = element.AddComponent<CanvasGroup>();

        group.alpha = alpha;
    }

    private IEnumerator AnimateWinPanel(int score)
    {
        // Ensure darkening panel is behind win panel in hierarchy
        if (darkeningPanel != null && winPanel != null)
        {
            // Lower sibling index means further back in hierarchy
            darkeningPanel.transform.SetSiblingIndex(winPanel.transform.GetSiblingIndex() - 1);
        }

        // Get the panel's RectTransform
        RectTransform panelRect = winPanel.GetComponent<RectTransform>();

        // 1. Slide panel from top to final position
        if (panelRect != null)
        {
            panelRect.DOAnchorPos(winPanelFinalPosition, slideDuration)
                .SetEase(slideEaseType);
        }

        // Wait for slide to complete
        yield return new WaitForSeconds(slideDuration);

        // 2. Fade in the label AND shadow container SIMULTANEOUSLY
        if (label != null)
        {
            label.gameObject.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
                .SetEase(fadeEaseType);
        }
            
        // Fade in star container shadow at the same time as the label
        if (starContainersShadow != null)
        {
            starContainersShadow.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
                .SetEase(fadeEaseType);
        }

        yield return new WaitForSeconds(delayBetweenElements * 1.5f);
        
        // 3. Fade in score text
        if (scoreText != null)
        {
            scoreText.gameObject.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
                .SetEase(fadeEaseType);
        }

        yield return new WaitForSeconds(delayBetweenElements * 2f);

        // 4. Fade in stars based on score thresholds - SIMPLIFIED, NO SCALING
        // First star
        if (score >= star1Threshold && star1 != null)
        {
            CanvasGroup star1Group = star1.GetComponent<CanvasGroup>();
            if (star1Group != null)
            {
                star1Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                yield return new WaitForSeconds(delayBetweenElements);
            }
        }

        // Second star
        if (score >= star2Threshold && star2 != null)
        {
            CanvasGroup star2Group = star2.GetComponent<CanvasGroup>();
            if (star2Group != null)
            {
                star2Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                yield return new WaitForSeconds(delayBetweenElements);
            }
        }

        // Third star
        if (score >= star3Threshold && star3 != null)
        {
            CanvasGroup star3Group = star3.GetComponent<CanvasGroup>();
            if (star3Group != null)
            {
                star3Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                yield return new WaitForSeconds(delayBetweenElements);
            }
        }

        // Wait a bit longer before showing buttons
        yield return new WaitForSeconds(delayBetweenElements * 2);

        // 5. Fade in buttons - SIMPLIFIED, NO SCALING
        if (retryButton != null)
        {
            retryButton.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
                .SetEase(fadeEaseType);
        }

        yield return new WaitForSeconds(delayBetweenElements * 0.5f);

        if (homeButton != null)
        {
            homeButton.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
                .SetEase(fadeEaseType);
        }

        // Set expressions based on score
        if (stageBaseManager != null && stageBaseManager.wadeExpressions != null)
        {
            if (score >= star3Threshold)
                stageBaseManager.wadeExpressions.SetHeartEyesExpressionWithTimer(5f);
            else if (score >= star2Threshold)
                stageBaseManager.wadeExpressions.SetHoorayExpressionWithTimer(5f);
            else if (score >= star1Threshold)
                stageBaseManager.wadeExpressions.SetHeartEyesExpressionWithTimer(3f);
            else
                stageBaseManager.wadeExpressions.SetTiredExpressionWithTimer(4f);
        }
    }

    // Button handler methods
    public void RetryLevel()
    {
        // Restore original sorting order
        if (driversUICanvas != null)
        {
            driversUICanvas.sortingOrder = originalDriversUISortOrder;
        }
        
        // Hide darkening panel before scene change
        if (darkeningPanel != null)
            darkeningPanel.SetActive(false);

        StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().name));
    }

    public void GoToMainMenu()
    {
        // Restore original sorting order
        if (driversUICanvas != null)
        {
            driversUICanvas.sortingOrder = originalDriversUISortOrder;
        }

        // Hide darkening panel before scene change
        if (darkeningPanel != null)
            darkeningPanel.SetActive(false);

        levelLoader.LoadMainMenu(); 
    }
}
