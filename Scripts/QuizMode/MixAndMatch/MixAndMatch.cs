using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Add this for DOTween animations

public class MixAndMatch : MonoBehaviour
{
    [Header("SFX")]
    public QuizModeSFX quizModeSFX;// Reference to the QuizModeSFX script
    private bool timerWarningPlayed = false;
    public MixAndMatchSFX mixAndMatchSFX;
    [Header("References")]
    public QuizManager quizManager; // Reference to the main quiz manager
    public GameObject quizPanel; // Quiz panel to fade out
    public GameObject mixAndMatchPanel; // Mini-game panel to fade in
    public WinningInventory winningInventory; // Reference to the WinningInventory script

    [Header("UI Elements")]
    public Button continueButton; // Button to return to quiz

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f; // How fast the panels fade in/out

    // Components for fading
    private CanvasGroup quizPanelCanvasGroup;
    private CanvasGroup miniGamePanelCanvasGroup;

    [Header("Timer Settings")]
    public float gameTime = 60f; // Time limit in seconds
    public TextMeshProUGUI timerText; // UI for timer display
    private float remainingTime;
    private Coroutine timerCoroutine;
    private bool gameEnded = false;

    [Header("Score Settings")]
    public int pointsPerCompletedRound = 100; // Points per completed round
    private int totalScore = 0;

    [Header("Summary Panel")]
    public GameObject summaryPanel; // Panel to show after game ends
    public TextMeshProUGUI summaryScoreText; // Text to display score in summary
    public TextMeshProUGUI summaryRoundsText; // Text to display completed rounds
    public Button summaryContinueButton; // Button to continue from summary

    [Header("Summary Panel Animation")]
    public Vector2 summaryStartPosition = new Vector2(800, 0); // Start position off-screen to the right
    public Vector2 summaryHiddenPosition = new Vector2(800, 0); // Position to slide to when hiding
    public float slideDuration = 0.7f;         // How long the panel takes to slide in/out
    public float elementFadeDuration = 0.4f;   // How long each element takes to fade in
    public float delayBetweenElements = 0.1f;  // Delay between elements fading in
    public Ease slideEaseType = Ease.OutBack;  // Easing for the panel slide
    public Ease fadeEaseType = Ease.OutQuad;   // Easing for fading elements

    [Header("Pause Controls")]
    public Button pauseButton; // Pause button for Mix and Match mode

    [Header("Instruction Panel")]
    public GameObject instructionPanel; // Panel containing instructions
    public float instructionFadeInDuration = 0.8f; // How long it takes to fade in
    public float instructionDisplayDuration = 3.0f; // How long to display the instruction
    public float instructionFadeOutDuration = 0.8f; // How long it takes to fade out
    public Ease instructionFadeEaseType = Ease.InOutSine; // Easing for the fade animation

    // Add these new fields
    private bool isTimerPaused = false;
    private float pausedTimeRemaining = 0f;

    void Start()
    {
        // Make sure panels have CanvasGroup components for fading
        SetupCanvasGroups();

        // Hide mini-game panel initially
        if (mixAndMatchPanel != null)
        {
            mixAndMatchPanel.SetActive(false);
        }

        // Hide summary panel initially
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
        }

        // Set up continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnGameButtonClicked);
        }

        // Set up summary continue button
        if (summaryContinueButton != null)
        {
            summaryContinueButton.onClick.AddListener(OnSummaryContinueClicked);
        }

        // Find WinningInventory if not assigned
        if (winningInventory == null)
        {
            winningInventory = FindObjectOfType<WinningInventory>();
        }

        // Fix for line 458 - Check if PauseManager exists before referencing it
        if (pauseButton != null)
        {
            PauseManager pauseManager = FindObjectOfType<PauseManager>();
            if (pauseManager != null)
            {
                pauseManager.RegisterPauseButton(pauseButton);
            }
            else
            {
                Debug.LogWarning("PauseManager not found in scene. Pause functionality may not work.");
            }
        }
    }

    private void SetupCanvasGroups()
    {
        // Get or add CanvasGroup to quiz panel
        if (quizPanel != null)
        {
            quizPanelCanvasGroup = quizPanel.GetComponent<CanvasGroup>();
            if (quizPanelCanvasGroup == null)
            {
                quizPanelCanvasGroup = quizPanel.AddComponent<CanvasGroup>();
            }
        }

        // Get or add CanvasGroup to mini-game panel
        if (mixAndMatchPanel != null)
        {
            miniGamePanelCanvasGroup = mixAndMatchPanel.GetComponent<CanvasGroup>();
            if (miniGamePanelCanvasGroup == null)
            {
                miniGamePanelCanvasGroup = mixAndMatchPanel.AddComponent<CanvasGroup>();
            }
        }
    }

    // Called from QuizManager when half the questions are done
    public void StartMiniGame()
    {
        // Reset game state
        gameEnded = false;
        totalScore = 0;
        
        // Explicitly make sure summary panel is hidden
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
        }
        
        // Setup mini-game with initial shuffle
        SetupMiniGame();
        
        // Start transition
        StartCoroutine(TransitionToMiniGame());
    }

    // Replace the SetupMiniGame method
    private void SetupMiniGame()
    {
        // If you have a WinningInventory reference, tell it to do initial shuffle
        if (winningInventory != null)
        {
            // Check if quiz panel is active
            bool quizPanelActive = quizPanel != null && quizPanel.activeSelf;
            
            // Just call ShuffleRoadSigns without parameters
            winningInventory.ShuffleRoadSigns();
            
            // Tell WinningInventory about the quiz panel state
            winningInventory.SetQuizPanelActive(quizPanelActive);
        }
    }

    // Update the TransitionToMiniGame method
    private IEnumerator TransitionToMiniGame()
    {
        // Make sure summary panel is hidden before starting
        if (summaryPanel != null && summaryPanel.activeSelf)
        {
            summaryPanel.SetActive(false);
        }
        
        // Fade out quiz panel
        if (quizPanelCanvasGroup != null)
        {
            yield return FadeCanvasGroup(quizPanelCanvasGroup, 1f, 0f, fadeDuration);
        }

        // Deactivate quiz panel after fading
        quizPanel.SetActive(false);

        // Show instruction panel with fade animation BEFORE showing the mini-game panel
        if (instructionPanel != null)
        {
            yield return StartCoroutine(ShowInstructionPanelWithFade());
        }

        // Now activate mini-game panel
        mixAndMatchPanel.SetActive(true);

        // Fade in mini-game panel
        if (miniGamePanelCanvasGroup != null)
        {
            miniGamePanelCanvasGroup.alpha = 0f;
            yield return FadeCanvasGroup(miniGamePanelCanvasGroup, 0f, 1f, fadeDuration);
        }

        // At this point, the mini-game is fully visible
        Debug.Log("Mini-game panel is now visible!");
        
        // Start the timer after everything is visible
        StartTimer();
    }

    // Start the timer
    private void StartTimer()
    {
        // Initialize time
        remainingTime = gameTime;
        
        // Stop any existing timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        // Start the countdown
        timerCoroutine = StartCoroutine(CountdownTimer());
    }

    // Timer countdown coroutine
    private IEnumerator CountdownTimer()
    {
        while (remainingTime > 0 && !gameEnded)
        {
            // Update timer display
            UpdateTimerDisplay();
            int secondsLeft = Mathf.CeilToInt(remainingTime);
            if (secondsLeft == 15 && !timerWarningPlayed && quizModeSFX != null)
            {
                quizModeSFX.PlayTimerWarning();
                timerWarningPlayed = true;
            }
            // Wait for a frame
            yield return null;
            
            // Decrease time
            remainingTime -= Time.deltaTime;
        }
        
        // Make sure timer shows zero at the end
        remainingTime = 0;
        UpdateTimerDisplay();
        
        if (quizModeSFX != null)
        quizModeSFX.StopWarning();
        // Time's up!
        if (!gameEnded)
        {
            EndGame();
        }
    }

    // Update the timer text
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Display whole seconds
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = seconds.ToString();
            
            // Change color when time is running out
            if (seconds <= 10)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }

    // Called when continue button is clicked during game
    public void OnGameButtonClicked()
    {
        // If game is already ended, do nothing
        if (gameEnded)
            return;
            
        // Calculate final score before ending
        if (winningInventory != null)
        {
            int completedRounds = Mathf.Max(0, winningInventory.CurrentRound - 1);
            totalScore = completedRounds * pointsPerCompletedRound;
            Debug.Log($"Game ended early with {completedRounds} completed rounds for {totalScore} points");
        }
        
        // Set game ended flag
        gameEnded = true;
        
        // Stop timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        // Disable input for all draggable items
        DisableDragging();
        
        // Add score to quiz manager directly
        if (quizManager != null)
        {
            quizManager.AddMiniGameScore(totalScore);
            Debug.Log($"Added {totalScore} points to quiz score");
        }
        
        // Return to quiz without showing summary panel
        StartCoroutine(ReturnToQuiz());
    }

    // End the game and show summary
    private void EndGame()
    {
        // Set game ended flag
        gameEnded = true;
        
        // Stop timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        // Calculate final score based on completed rounds
        if (winningInventory != null)
        {
            int completedRounds = Mathf.Max(0, winningInventory.CurrentRound - 1);
            totalScore = completedRounds * pointsPerCompletedRound;
            Debug.Log($"Game ended with {completedRounds} completed rounds for {totalScore} points");
        }
        
        // Disable input for all draggable items
        DisableDragging();
        
        // Only show summary panel if the timer has run out (or is very close to 0)
        if (remainingTime <= 0.1f)
        {
            ShowSummaryPanel();
        }
        else
        {
            // If the game was ended early (by button click), just return to quiz
            StartCoroutine(ReturnToQuiz());
        }
    }

    // Disable dragging for all items
    private void DisableDragging()
    {
        DraggableItem[] draggableItems = FindObjectsOfType<DraggableItem>();
        foreach (DraggableItem item in draggableItems)
        {
            CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    // Show the summary panel with score
    private void ShowSummaryPanel()
    {
        if (summaryPanel != null)
        {
            // Make panel active but position it off-screen first
            summaryPanel.SetActive(true);
            
                if (mixAndMatchSFX != null)
                mixAndMatchSFX.PlaySummaryPanelSFX();
            // Get the panel's RectTransform
            RectTransform panelRect = summaryPanel.GetComponent<RectTransform>();
            
            // Set initial position (off-screen to the right)
            panelRect.anchoredPosition = summaryStartPosition;
            
            // Make sure panel elements have CanvasGroups
            EnsureSummaryElementsHaveCanvasGroups();
            
            // Hide all elements initially (they'll fade in during animation)
            SetSummaryElementsAlpha(0);
            
            // Update text values
            if (summaryScoreText != null)
            {
                summaryScoreText.text = $"Total Score: {totalScore}";
            }
            
            if (summaryRoundsText != null && winningInventory != null)
            {
                int completedRounds = Mathf.Max(0, winningInventory.CurrentRound - 1);
                summaryRoundsText.text = $"Completed Rounds: {completedRounds}";
            }
            
            // Hide the game continue button
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }
            
            // Make sure summary continue button is active
            if (summaryContinueButton != null)
            {
                summaryContinueButton.gameObject.SetActive(true);
                summaryContinueButton.interactable = true;
            }
            
            // Start animation sequence
            StartCoroutine(AnimateSummaryPanelIn());
        }
    }

    // Add these new methods for handling the animations
    private IEnumerator AnimateSummaryPanelIn()
    {
        // Get the panel's RectTransform
        RectTransform panelRect = summaryPanel.GetComponent<RectTransform>();
        
        // 1. Slide panel from right to center
        panelRect.DOAnchorPos(Vector2.zero, slideDuration)
            .SetEase(slideEaseType);
        
        // Wait for slide to complete
        yield return new WaitForSeconds(slideDuration);
        
        // 2. Fade in each element with slight delays between them
        
        // Fade in score text
        if (summaryScoreText != null)
        {
            CanvasGroup scoreGroup = summaryScoreText.gameObject.GetComponent<CanvasGroup>();
            if (scoreGroup != null)
            {
                scoreGroup.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
            }
        }
        
        yield return new WaitForSeconds(delayBetweenElements);
        
        // Fade in rounds text
        if (summaryRoundsText != null)
        {
            CanvasGroup roundsGroup = summaryRoundsText.gameObject.GetComponent<CanvasGroup>();
            if (roundsGroup != null)
            {
                roundsGroup.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
            }
        }
        
        yield return new WaitForSeconds(delayBetweenElements * 2);
        
        // Fade in continue button without changing its size
        if (summaryContinueButton != null)
        {
            CanvasGroup buttonGroup = summaryContinueButton.gameObject.GetComponent<CanvasGroup>();
            if (buttonGroup != null)
            {
                // Just fade in the button without scaling
                buttonGroup.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                
                // Optional: Add a subtle highlight effect instead of scaling
                Image buttonImage = summaryContinueButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    // Store original color
                    Color originalColor = buttonImage.color;
                    Color highlightColor = new Color(
                        Mathf.Min(originalColor.r + 0.2f, 1f),
                        Mathf.Min(originalColor.g + 0.2f, 1f),
                        Mathf.Min(originalColor.b + 0.2f, 1f),
                        originalColor.a
                    );
                    
                    // Create a subtle flash effect
                    buttonImage.DOColor(highlightColor, elementFadeDuration * 0.5f)
                        .SetLoops(2, LoopType.Yoyo);
                }
            }
        }
    }

    private void EnsureSummaryElementsHaveCanvasGroups()
    {
        // Add CanvasGroups to all summary elements if they don't have them
        if (summaryScoreText != null && summaryScoreText.gameObject.GetComponent<CanvasGroup>() == null)
            summaryScoreText.gameObject.AddComponent<CanvasGroup>();
            
        if (summaryRoundsText != null && summaryRoundsText.gameObject.GetComponent<CanvasGroup>() == null)
            summaryRoundsText.gameObject.AddComponent<CanvasGroup>();
            
        if (summaryContinueButton != null && summaryContinueButton.gameObject.GetComponent<CanvasGroup>() == null)
            summaryContinueButton.gameObject.AddComponent<CanvasGroup>();
    }

    private void SetSummaryElementsAlpha(float alpha)
    {
        // Set alpha for all summary elements
        SetElementAlpha(summaryScoreText?.gameObject, alpha);
        SetElementAlpha(summaryRoundsText?.gameObject, alpha);
        SetElementAlpha(summaryContinueButton?.gameObject, alpha);
    }

    private void SetElementAlpha(GameObject element, float alpha)
    {
        if (element == null) return;
        
        CanvasGroup group = element.GetComponent<CanvasGroup>();
        if (group == null)
            group = element.AddComponent<CanvasGroup>();
            
        group.alpha = alpha;
    }

    // Called when summary continue button is clicked
    public void OnSummaryContinueClicked()
    {
        // Add score to quiz manager
        if (quizManager != null)
        {
            quizManager.AddMiniGameScore(totalScore);
            Debug.Log($"Added {totalScore} points to quiz score");
        }
        
        // Start animation to slide panel out
        StartCoroutine(AnimateSummaryPanelOut());
    }

    private IEnumerator AnimateSummaryPanelOut()
    {
        // Get the panel's RectTransform
        RectTransform panelRect = summaryPanel.GetComponent<RectTransform>();
        
        // Fade out all elements quickly
        SetSummaryElementsAlpha(0);
        
        // Slide panel back to the right
        panelRect.DOAnchorPos(summaryHiddenPosition, slideDuration * 0.7f)
            .SetEase(Ease.InBack);
        
        // Wait for slide to complete
        yield return new WaitForSeconds(slideDuration * 0.7f);
        
        // Hide the panel
        summaryPanel.SetActive(false);
        
        // Return to quiz
        StartCoroutine(ReturnToQuiz());
    }

    private IEnumerator ReturnToQuiz()
    {
        // Fade out mini-game panel
        if (miniGamePanelCanvasGroup != null)
        {
            yield return FadeCanvasGroup(miniGamePanelCanvasGroup, 1f, 0f, fadeDuration);
        }

        // Deactivate mini-game panel
        mixAndMatchPanel.SetActive(false);

        // Activate quiz panel
        quizPanel.SetActive(true);

        // Fade in quiz panel
        if (quizPanelCanvasGroup != null)
        {
            quizPanelCanvasGroup.alpha = 0f;
            yield return FadeCanvasGroup(quizPanelCanvasGroup, 0f, 1f, fadeDuration);
        }

        // Tell QuizManager to resume the quiz
        if (quizManager != null)
        {
            quizManager.ResumeQuiz();
        }
    }

    // Helper method for fading
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        group.alpha = startAlpha;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }

        group.alpha = endAlpha;
    }

    // Add this new method to pause the timer
    public void PauseTimer()
    {
        // Store the current time remaining
        pausedTimeRemaining = remainingTime;
        
        // Set pause flag
        isTimerPaused = true;
        
        // Stop the coroutine
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        
        Debug.Log($"MixAndMatch timer paused with {pausedTimeRemaining} seconds remaining");
    }

    // Modify the ResumeTiming method
    public void ResumeTiming()
    {
        // Only resume if game is not ended
        if (!gameEnded)
        {
            Debug.Log($"Resuming MixAndMatch timer with {pausedTimeRemaining} seconds remaining");
            
            // Start a new timer from the stored time (not resetting to gameTime)
            timerCoroutine = StartCoroutine(ResumeCountdownTimer());
        }
    }

    // Add this new coroutine method for resuming the timer
    private IEnumerator ResumeCountdownTimer()
    {
        // Use the stored time if we're resuming from pause
        if (isTimerPaused)
        {
            remainingTime = pausedTimeRemaining;
            isTimerPaused = false;
        }
        else
        {
            // Initialize timer if not resuming from pause
            remainingTime = gameTime;
        }
        
        // Continue the timer loop as normal
        while (remainingTime > 0 && !gameEnded)
        {
            UpdateTimerDisplay();
            int secondsLeft = Mathf.CeilToInt(remainingTime);
            if (secondsLeft == 15 && !timerWarningPlayed && quizModeSFX != null)
            {
                quizModeSFX.PlayTimerWarning();
                timerWarningPlayed = true;
            }
            yield return null;
            remainingTime -= Time.deltaTime;
        }
        
        remainingTime = 0;
        UpdateTimerDisplay();
        if (quizModeSFX != null)
        quizModeSFX.StopWarning();

        if (!gameEnded)
        {
            EndGame();
        }
    }

    // Add this new method for fading the instruction panel
    private IEnumerator ShowInstructionPanelWithFade()
    {
          if (mixAndMatchSFX != null)
        {
            mixAndMatchSFX.PlayInstructionAudio();
        }
        // Make sure the panel is active
        instructionPanel.SetActive(true);
        
        // Make sure the panel has a CanvasGroup component
        CanvasGroup instructionCanvasGroup = instructionPanel.GetComponent<CanvasGroup>();
        if (instructionCanvasGroup == null)
        {
            instructionCanvasGroup = instructionPanel.AddComponent<CanvasGroup>();
        }
        
        // Start with fully transparent
        instructionCanvasGroup.alpha = 0f;
        
        // Fade in
        instructionCanvasGroup.DOFade(1f, instructionFadeInDuration)
            .SetEase(instructionFadeEaseType);
        
        // Wait for fade in to complete
        yield return new WaitForSeconds(instructionFadeInDuration);
        
        // Wait for display duration
        yield return new WaitForSeconds(instructionDisplayDuration);
        
        // Fade out
        instructionCanvasGroup.DOFade(0f, instructionFadeOutDuration)
            .SetEase(instructionFadeEaseType);
        
        // Wait for fade out to complete
        yield return new WaitForSeconds(instructionFadeOutDuration);
        
        // Hide the panel
        instructionPanel.SetActive(false);
        
        Debug.Log("Instruction panel fade sequence completed!");
    }
}