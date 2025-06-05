using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixAndMatch : MonoBehaviour
{
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

    private IEnumerator TransitionToMiniGame()
    {
        // Fade out quiz panel
        if (quizPanelCanvasGroup != null)
        {
            yield return FadeCanvasGroup(quizPanelCanvasGroup, 1f, 0f, fadeDuration);
        }

        // Deactivate quiz panel after fading
        quizPanel.SetActive(false);

        // Activate mini-game panel
        mixAndMatchPanel.SetActive(true);

        // Fade in mini-game panel
        if (miniGamePanelCanvasGroup != null)
        {
            miniGamePanelCanvasGroup.alpha = 0f;
            yield return FadeCanvasGroup(miniGamePanelCanvasGroup, 0f, 1f, fadeDuration);
        }

        // At this point, the mini-game is fully visible
        Debug.Log("Mini-game is now active!");
        
        // Start the timer
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
            
            // Wait for a frame
            yield return null;
            
            // Decrease time
            remainingTime -= Time.deltaTime;
        }
        
        // Make sure timer shows zero at the end
        remainingTime = 0;
        UpdateTimerDisplay();
        
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
            
        // End the game early
        EndGame();
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
        
        // Show summary panel
        ShowSummaryPanel();
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
        // Show panel
        summaryPanel.SetActive(true);
        
        // Update score text
        if (summaryScoreText != null)
        {
            summaryScoreText.text = $"Total Score: {totalScore}";
        }
        
        // Update rounds text
        if (summaryRoundsText != null && winningInventory != null)
        {
            int completedRounds = Mathf.Max(0, winningInventory.CurrentRound - 1);
            summaryRoundsText.text = $"Completed Rounds: {completedRounds}";
        }
        
        // Hide the game continue button (if it exists)
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
        
        // Make sure the summary continue button is active and visible
        if (summaryContinueButton != null)
        {
            summaryContinueButton.gameObject.SetActive(true);
            
            // Ensure the button is interactable
            summaryContinueButton.interactable = true;
            
            // Log for debugging
            Debug.Log("Summary continue button is active: " + summaryContinueButton.gameObject.activeInHierarchy);
        }
        else
        {
            Debug.LogError("Summary continue button is not assigned in the inspector!");
        }
    }
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
        
        // Hide summary panel
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);
        }
        
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
}