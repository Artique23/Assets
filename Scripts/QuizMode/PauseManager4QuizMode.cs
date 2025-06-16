using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class PauseManager : MonoBehaviour
{

    LevelLoader levelLoader;
    [Header("References")]
    public GameObject pausePanel;
    public Button[] pauseButtons; // Change to array for multiple pause buttons
    public Button resumeButton;
    public Button retryButton;
    public Button mainMenuButton;
    public TextMeshProUGUI pauseTitle;

    [Header("Animation Settings")]
    public float fadeDuration = 0.3f;
    public float elementFadeDuration = 0.2f;
    public float delayBetweenElements = 0.05f;
    public Ease fadeEaseType = Ease.OutQuad;
    
    // References to game managers
    private QuizManager quizManager;
    private MixAndMatch mixAndMatchManager;

    // Track current game state
    private bool isPaused = false;
    private CanvasGroup pausePanelCanvasGroup;
    
    private void Awake()
    {
        // Find the LevelLoader if we need it
        levelLoader = FindObjectOfType<LevelLoader>();
        
        // Find references if not set
        quizManager = FindObjectOfType<QuizManager>();
        mixAndMatchManager = FindObjectOfType<MixAndMatch>();
        
        // Get canvas group (or add one)
        if (pausePanel != null)
        {
            pausePanelCanvasGroup = pausePanel.GetComponent<CanvasGroup>();
            if (pausePanelCanvasGroup == null)
                pausePanelCanvasGroup = pausePanel.AddComponent<CanvasGroup>();
                
            pausePanel.SetActive(false);
        }
        
        // Set up multiple pause buttons
        if (pauseButtons != null && pauseButtons.Length > 0)
        {
            foreach (Button btn in pauseButtons)
            {
                if (btn != null)
                    btn.onClick.AddListener(TogglePause);
            }
        }
        
        // Set up other buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }
    
    // Call this from the pause button
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
    
    public void PauseGame()
    {
        // Already paused
        if (isPaused) return;
        
        // Set paused state
        isPaused = true;
        
        // Pause game time
        Time.timeScale = 0f;
        
        // Pause timers in quiz/mini-game without resetting them
        if (quizManager != null)
        {
            quizManager.PauseTimer(); // We'll add this new method to QuizManager
        }

        if (mixAndMatchManager != null && mixAndMatchManager.gameObject.activeInHierarchy)
        {
            mixAndMatchManager.PauseTimer(); // We'll add this new method to MixAndMatch
        }
        
        // Show pause panel with animation
        ShowPausePanel();
    }
    
    public void ResumeGame()
    {
        // Not paused
        if (!isPaused) return;
        
        // Hide pause panel with animation
        HidePausePanel(() => {
            // Set game state after animation completes
            isPaused = false;
            
            // Resume game time
            Time.timeScale = 1f;
            
            // Resume timers in quiz/mini-game
            if (quizManager != null)
            {
                quizManager.ResumeTiming();
            }
            
            if (mixAndMatchManager != null && mixAndMatchManager.gameObject.activeInHierarchy)
            {
                mixAndMatchManager.ResumeTiming();
            }
        });
    }
    
    public void RetryGame()
    {
        // Resume time scale before scene reload
        Time.timeScale = 1f;
        
        // Use LevelLoader for animated transition if available
        if (levelLoader != null)
        {
            StartCoroutine(RetryCurrentLevel());
        }
        else
        {
            // Fallback to direct loading if LevelLoader isn't found
            Debug.LogWarning("LevelLoader not found. Using direct scene loading instead.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    public void GoToMainMenu()
    {
        // Resume time scale before scene change
        Time.timeScale = 1f;
        
        // Use LevelLoader if available, otherwise direct load
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader != null)
        {
            levelLoader.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
    
    // Simplify ShowPausePanel method to avoid size changes
    private void ShowPausePanel()
    {
        if (pausePanel == null) return;
        
        // Make panel active
        pausePanel.SetActive(true);
        
        // Reset alpha
        pausePanelCanvasGroup.alpha = 0f;
        
        // Get all UI elements in panel for sequential fade-in
        CanvasGroup[] elementGroups = pausePanel.GetComponentsInChildren<CanvasGroup>();
        
        // Reset alpha for all elements
        foreach (CanvasGroup group in elementGroups)
        {
            if (group != pausePanelCanvasGroup)
                group.alpha = 0f;
        }
        
        // Fade in panel background
        pausePanelCanvasGroup.DOFade(1f, fadeDuration)
            .SetEase(fadeEaseType)
            .SetUpdate(true);
        
        // Fade in other elements sequentially
        float delay = fadeDuration * 0.6f;
        foreach (CanvasGroup group in elementGroups)
        {
            if (group != pausePanelCanvasGroup)
            {
                group.DOFade(1f, elementFadeDuration)
                    .SetDelay(delay)
                    .SetEase(fadeEaseType)
                    .SetUpdate(true);
                
                // No scaling animations - maintain original size
                delay += delayBetweenElements;
            }
        }
    }
    
    // Simplify HidePausePanel method to avoid size changes
    private void HidePausePanel(TweenCallback onComplete = null)
    {
        if (pausePanel == null) return;
        
        // Get all UI elements for simultaneous fade-out
        CanvasGroup[] elementGroups = pausePanel.GetComponentsInChildren<CanvasGroup>();
        
        // Fade out all elements except the panel itself
        foreach (CanvasGroup group in elementGroups)
        {
            if (group != pausePanelCanvasGroup)
            {
                group.DOFade(0f, fadeDuration * 0.7f)
                    .SetEase(fadeEaseType)
                    .SetUpdate(true);
            }
        }
        
        // Fade out panel background last
        pausePanelCanvasGroup.DOFade(0f, fadeDuration)
            .SetEase(fadeEaseType)
            .SetUpdate(true)
            .OnComplete(() => {
                pausePanel.SetActive(false);
                onComplete?.Invoke();
            });
    }
    
    private void OnDestroy()
    {
        // Kill all tweens when destroyed
        DOTween.Kill(pausePanelCanvasGroup);
        
        if (pauseTitle != null)
            DOTween.Kill(pauseTitle.transform);
            
        // Make sure time scale is reset
        Time.timeScale = 1f;
    }

    // Add this public method for registering new pause buttons
    public void RegisterPauseButton(Button button)
    {
        if (button == null) 
        {
            Debug.LogWarning("Attempted to register a null pause button");
            return;
        }
        
        // Remove any existing listeners to prevent duplicates
        button.onClick.RemoveListener(TogglePause);
        
        // Add listener
        button.onClick.AddListener(TogglePause);
        
        Debug.Log($"Successfully registered pause button: {button.gameObject.name}");
    }

    // Helper method to handle LevelLoader coroutine
    private IEnumerator RetryCurrentLevel()
    {
        // Get current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Retrying current level: {currentSceneName} with transition");
        
        // Use the LevelLoader to reload with transition
        yield return StartCoroutine(levelLoader.LoadLevel(currentSceneName));
    }
}