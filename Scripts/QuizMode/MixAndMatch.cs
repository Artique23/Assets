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

    [Header("UI Elements")]
    public Button continueButton; // Button to return to quiz

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f; // How fast the panels fade in/out

    // Components for fading
    private CanvasGroup quizPanelCanvasGroup;
    private CanvasGroup miniGamePanelCanvasGroup;

    [Header("Road Sign Game")]
    public Transform roadsignContainer;  // Container where road signs spawn
    public Transform boxesContainer;     // Container for target boxes
    public GameObject roadSignPrefab;    // Prefab for draggable road signs
    public GameObject targetBoxPrefab;   // Prefab for target boxes
    public float gameTime = 60f;         // Time limit in seconds
    public int boxesPerRound = 3;        // Number of boxes to show each round
    public TextMeshProUGUI timerText;    // UI for timer display

    void Start()
    {
        // Make sure panels have CanvasGroup components for fading
        SetupCanvasGroups();

        // Hide mini-game panel initially
        if (mixAndMatchPanel != null)
        {
            mixAndMatchPanel.SetActive(false);
        }

        // Set up continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
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
        // Start transition
        StartCoroutine(TransitionToMiniGame());
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
    }

    // Called when continue button is clicked
    public void OnContinueButtonClicked()
    {

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

    // Add these methods to your MixAndMatch class


    
    
}