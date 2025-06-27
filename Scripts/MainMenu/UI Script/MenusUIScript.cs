using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenusUIScript : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject GeneralUI, MainMenuUI, StartMenuUI, StoryModeUI, SettingsUI, AboutUI, JPXCustomize, JhayluxCustomize, DreivusCustomize;

    // Animations Using Dotween
    [SerializeField] RectTransform MainMenuLowerPanel;
    [SerializeField] float bottomY, baseY;
    [SerializeField] float tweenDuration;

    GameManagerSaveAndLoad gameManager;

    #endregion

    #region AnimationVariables
    [Header("Fade Animations")]
    [SerializeField] private CanvasGroup startMenuCanvasGroup;  // Assign in inspector
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private Ease fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease fadeOutEase = Ease.InQuad;

    [Header("Quiz Mode UI")]
    [SerializeField] private GameObject quizModePanel;
    [SerializeField] private CanvasGroup quizModeCanvasGroup;  // Add this line
    [SerializeField] private GameObject easyPanel, averagePanel, difficultPanel;
    [SerializeField] private Button easyButton, averageButton, difficultButton;

    [Header("Settings UI")]
    [SerializeField] private GameObject settingsPanels;
    [SerializeField] private CanvasGroup settingsPanelsCanvasGroup;
    [SerializeField] private GameObject controlsPanel, musicSfxPanel, qualityPanel;
    [SerializeField] private Button controlsButton, musicSfxButton, qualityButton;

    [Header("Settings Button Animation")]
    [SerializeField] private float settingsButtonScaleDuration = 0.3f;
    [SerializeField] public Vector3 selectedSettingsButtonScale = new Vector3(1.15f, 1.15f, 1.15f);
    [SerializeField] public Vector3 normalSettingsButtonScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private Ease settingsButtonScaleEase = Ease.OutBack;
    public int currentSelectedSettings = 0; // 0 = Controls, 1 = Music/SFX, 2 = Quality
    #endregion

    [Header("Difficulty Button Animation")]
    [SerializeField] private float buttonScaleDuration = 0.3f;
    [SerializeField] private Vector3 selectedButtonScale = new Vector3(1.15f, 1.15f, 1.15f);
    [SerializeField] private Vector3 normalButtonScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private Ease buttonScaleEase = Ease.OutBack;
    public int currentSelectedDifficulty = 0; // 0 = Easy, 1 = Average, 2 = Difficult


    // Start is called before the first frame update

    #region OnStart
    void Start()
    {
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(true);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);
        JPXCustomize.SetActive(false);
        JhayluxCustomize.SetActive(false);
        DreivusCustomize.SetActive(false);
    }

    #endregion


    #region Canvas/Panel Activation

    public void ShowCustomizeMenu()
    {
        Debug.Log("Customize Menu is Visible...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(true);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);

        // Get the current car index from GameManagerSaveAndLoad
        gameManager = FindObjectOfType<GameManagerSaveAndLoad>();
        if (gameManager == null)
        {
            Debug.LogError("GameManagerSaveAndLoad not found!");
            return;
        }

        // Get the current car index using the public method
        int currentCarIndex = gameManager.GetCurrentCarIndex();

        // Disable all customize panels first
        JPXCustomize.SetActive(false);
        JhayluxCustomize.SetActive(false);
        DreivusCustomize.SetActive(false);

        // Set the active customize panel based on the selected car index
        switch (currentCarIndex)
        {
            case 0:
                JPXCustomize.SetActive(true);
                JhayluxCustomize.SetActive(false);
                DreivusCustomize.SetActive(false);
                break;
            case 1:
                JhayluxCustomize.SetActive(true);
                JPXCustomize.SetActive(false);
                DreivusCustomize.SetActive(false);
                break;
            case 2:
                DreivusCustomize.SetActive(true);
                JhayluxCustomize.SetActive(false);
                JPXCustomize.SetActive(false);
                break;
            default:
                Debug.LogError("Invalid car index!");
                break;
        }
    }

    void HideCustomizeMenu()
    {
        Debug.Log("Customize Menu is Hidden...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(true);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);
        JPXCustomize.SetActive(false);
        JhayluxCustomize.SetActive(false);
        DreivusCustomize.SetActive(false);
    }

    void ShowMainMenu()
    {
        Debug.Log("Main Menu is Visible...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(true);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);
    }

    public void ShowStartMenu()
    {
        Debug.Log("Start Menu is fading in...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(false);

        StartMenuUI.SetActive(true);
        startMenuCanvasGroup.alpha = 0f;

        // Fade in the start menu panel
        startMenuCanvasGroup.DOFade(1f, fadeInDuration)
            .SetEase(fadeInEase);

        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);
    }
    void ShowAboutMenu()
    {
        Debug.Log("Main Menu is Visible...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(false);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(true);
    }

    void ShowSettingsMenu()
    {
        Debug.Log("Settings Menu is Visible...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(false);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(true);
        AboutUI.SetActive(false);

        // Initialize settings panels
        controlsPanel.SetActive(true);
        musicSfxPanel.SetActive(false);
        qualityPanel.SetActive(false);
        
        // Set initial button scales
        AnimateSettingsButtonSelection(0);
        currentSelectedSettings = 0;
    }

    // Show the Quiz Mode panel with fade-in animation
    public void ShowQuizModePanel()
    {
        Debug.Log("Quiz Mode Panel is fading in...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(false);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);

        // Activate quiz mode panel but make it transparent
        quizModePanel.SetActive(true);
        quizModeCanvasGroup.alpha = 0f;

        // Fade in the quiz mode panel
        quizModeCanvasGroup.DOFade(1f, fadeInDuration)
            .SetEase(fadeInEase);

        // By default, show the easy panel
        easyPanel.SetActive(true);
        averagePanel.SetActive(false);
        difficultPanel.SetActive(false);
        
        // Add this line to set initial button scales
        AnimateButtonSelection(0);
        currentSelectedDifficulty = 0; // Set default difficulty to Easy
    }

    // Hide the Quiz Mode panel with fade-out animation
    public void HideQuizModePanel()
    {
        Debug.Log("Quiz Mode Panel is fading out...");

        // Fade out
        quizModeCanvasGroup.DOFade(0f, fadeOutDuration)
            .SetEase(fadeOutEase)
            .OnComplete(() => {
                quizModePanel.SetActive(false);
                ShowStartMenu();
            });
    }

    #endregion


    #region DoTween Animations
    // Animations Using Dotween

    // Main Menu UI Animation

    // On Start Menu Button Click
    public void MainMenuLowerPanelOutro4Start()
    {
        MainMenuLowerPanel.DOAnchorPosY(bottomY, tweenDuration);
        StartCoroutine(DelayedStartMainMenu(0.5f));  // Changed to DelayedStartMainMenu
        ShowStartMenu();
    }

    // Add this coroutine if it doesn't exist
    IEnumerator DelayedStartMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        
    }

    public void MainMenuLowerPanelOutro4About()
    {
        MainMenuLowerPanel.DOAnchorPosY(bottomY, tweenDuration);
        StartCoroutine(DelayedAboutMainMenu(0.4f));
    }

    public void MainMenuLowerPanelOutro4Settings()
    {
        MainMenuLowerPanel.DOAnchorPosY(bottomY, tweenDuration);
        StartCoroutine(DelayedSettingsMainMenu(0.4f));
    }

    public void MainMenuLowerPanelIntroFromAbout()
    {
        MainMenuLowerPanel.DOAnchorPosY(baseY, tweenDuration);
        ShowMainMenu();
    }

    public void MainMenuLowerPanelIntroFromSettings()
    {
        MainMenuLowerPanel.DOAnchorPosY(baseY, tweenDuration);
        ShowMainMenu();
    }

    public void MainMenuLowerPanelIntroFromStart()
    {
        MainMenuLowerPanel.DOAnchorPosY(baseY, tweenDuration);
        ShowMainMenu();
    }
    // END

    // On About Menu Button Click
    IEnumerator DelayedAboutMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowAboutMenu();
    }
    IEnumerator DelayedSettingsMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowSettingsMenu();
    }

        // Method to transition from Start Menu to Quiz Mode
    public void TransitionToQuizMode()
    {
        Debug.Log("Transitioning to Quiz Mode...");
        
        // Prepare the quiz panel before starting animations
        quizModePanel.SetActive(true);
        quizModeCanvasGroup.alpha = 0f;
        
        // Setup default panels
        easyPanel.SetActive(true);
        averagePanel.SetActive(false);
        difficultPanel.SetActive(false);
        
        // Create a simultaneous animation sequence
        DOTween.Sequence()
            // Fade out start menu
            .Append(startMenuCanvasGroup.DOFade(0f, fadeOutDuration).SetEase(fadeOutEase))
            // Immediately start fading in the quiz panel (with slight delay for visual appeal)
            .Insert(fadeOutDuration * 0.5f, quizModeCanvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeInEase))
            // When complete, ensure start menu is fully deactivated
            .OnComplete(() => {
                StartMenuUI.SetActive(false);
                // No need to call ShowQuizModePanel() since we're doing it directly here
            });
        
        // Update UI state
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(false);
        SettingsUI.SetActive(false);
        AboutUI.SetActive(false);
    }

    // END Main Menu UI Animation

    // On Start Menu Button Click

    // END OF Animations Using Dotweem

    #endregion


    #region Additional Methods
    
    // Add this to your Additional Methods region
    public void BeginQuiz()
    {
        Debug.Log("Beginning quiz with difficulty: " + currentSelectedDifficulty);
        
        // Find the LevelLoader
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        
        if (levelLoader != null)
        {
            // Fade out the quiz panel first
            quizModeCanvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(fadeOutEase)
                .OnComplete(() => {
                    // Then load the selected quiz
                    levelLoader.LoadSelectedQuiz();
                });
        }
        else
        {
            Debug.LogError("LevelLoader not found!");
        }
    }

    // Enhanced panel switchers with button animations
    public void ShowEasyPanel()
    {
        // Switch panels
        easyPanel.SetActive(true);
        averagePanel.SetActive(false);
        difficultPanel.SetActive(false);

        // Animate buttons
        AnimateButtonSelection(0);

        // Store current difficulty
        currentSelectedDifficulty = 0;
    }

    public void ShowAveragePanel()
    {
        // Switch panels
        easyPanel.SetActive(false);
        averagePanel.SetActive(true);
        difficultPanel.SetActive(false);
        
        // Animate buttons
        AnimateButtonSelection(1);
        
        // Store current difficulty
        currentSelectedDifficulty = 1;
    }

    public void ShowDifficultPanel()
    {
        // Switch panels
        easyPanel.SetActive(false);
        averagePanel.SetActive(false);
        difficultPanel.SetActive(true);
        
        // Animate buttons
        AnimateButtonSelection(2);
        
        // Store current difficulty
        currentSelectedDifficulty = 2;
    }

// Button animation helper
private void AnimateButtonSelection(int selectedIndex)
{
    // Scale all buttons
    if (easyButton != null)
        easyButton.transform.DOScale(selectedIndex == 0 ? selectedButtonScale : normalButtonScale, buttonScaleDuration).SetEase(buttonScaleEase);
        
    if (averageButton != null)
        averageButton.transform.DOScale(selectedIndex == 1 ? selectedButtonScale : normalButtonScale, buttonScaleDuration).SetEase(buttonScaleEase);
        
    if (difficultButton != null)
        difficultButton.transform.DOScale(selectedIndex == 2 ? selectedButtonScale : normalButtonScale, buttonScaleDuration).SetEase(buttonScaleEase);
}
    

    // Method to refresh customize menu when car changes
    public void RefreshCustomizeMenu()
    {
        // Check if any customize panel is active
        bool isCustomizeVisible = JPXCustomize.activeSelf || JhayluxCustomize.activeSelf || DreivusCustomize.activeSelf;

        // Only refresh if customize menu is currently visible
        if (isCustomizeVisible)
        {
            ShowCustomizeMenu();
        }
    }

    public void ToggleCustomizeMenu()
    {
        // Check the current active state of any customize panels
        bool isAnyPanelActive = JPXCustomize.activeSelf || JhayluxCustomize.activeSelf || DreivusCustomize.activeSelf;

        if (isAnyPanelActive)
        {
            // If any panel is active, hide all customize panels
            HideCustomizeMenu();
        }
        else
        {
            // If no panel is active, show the appropriate customize panel
            ShowCustomizeMenu();
        }
    }
    public void ChangeCarColor(int colorIndex)
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManagerSaveAndLoad>();
        }

        if (gameManager != null)
        {
            gameManager.SetCarColorIndex(colorIndex);
        }
        else
        {
            Debug.LogError("GameManagerSaveAndLoad not found when trying to change car color.");
        }
    }
    
    public void ShowControlsPanel()
    {
        Debug.Log("Showing Controls Panel");
        // Switch panels
        controlsPanel.SetActive(true);
        musicSfxPanel.SetActive(false);
        qualityPanel.SetActive(false);
        
        // Animate buttons
        AnimateSettingsButtonSelection(0);
        currentSelectedSettings = 0;
    }

    public void ShowMusicSfxPanel()
    {
        Debug.Log("Showing Music/SFX Panel");
        controlsPanel.SetActive(false);
        musicSfxPanel.SetActive(true);
        qualityPanel.SetActive(false);
        
        AnimateSettingsButtonSelection(1);
        currentSelectedSettings = 1;
    }

    public void ShowQualityPanel()
    {
        Debug.Log("Showing Quality Panel");
        controlsPanel.SetActive(false);
        musicSfxPanel.SetActive(false);
        qualityPanel.SetActive(true);
        
        AnimateSettingsButtonSelection(2);
        currentSelectedSettings = 2;
    }

    private void AnimateSettingsButtonSelection(int selectedIndex)
    {
        if (controlsButton != null)
            controlsButton.transform.DOScale(selectedIndex == 0 ? selectedSettingsButtonScale : normalSettingsButtonScale, settingsButtonScaleDuration).SetEase(settingsButtonScaleEase);
        
        if (musicSfxButton != null)
            musicSfxButton.transform.DOScale(selectedIndex == 1 ? selectedSettingsButtonScale : normalSettingsButtonScale, settingsButtonScaleDuration).SetEase(settingsButtonScaleEase);
        
        if (qualityButton != null)
            qualityButton.transform.DOScale(selectedIndex == 2 ? selectedSettingsButtonScale : normalSettingsButtonScale, settingsButtonScaleDuration).SetEase(settingsButtonScaleEase);
    }

    [Header("Quality Settings Button Animation")]
[SerializeField] private Button lowQualityButton, mediumQualityButton, highQualityButton;
[SerializeField] private float qualityButtonScaleDuration = 0.3f;
[SerializeField] private Vector3 selectedQualityButtonScale = new Vector3(1.15f, 1.15f, 1.15f);
[SerializeField] private Vector3 normalQualityButtonScale = new Vector3(0.9f, 0.9f, 0.9f);
[SerializeField] private Ease qualityButtonScaleEase = Ease.OutBack;
[SerializeField] private SetQuality qualitySettings; // Reference to your SetQuality script
public int currentSelectedQuality = 0; // 0 = Low, 1 = Medium, 2 = High

public void SetLowQuality()
{
    Debug.Log("Setting Low Quality");
    if (qualitySettings != null)
    {
        qualitySettings.LowQ();
        AnimateQualityButtonSelection(0);
        currentSelectedQuality = 0;
    }
}

public void SetMediumQuality()
{
    Debug.Log("Setting Medium Quality");
    if (qualitySettings != null)
    {
        qualitySettings.MediumQ();
        AnimateQualityButtonSelection(1);
        currentSelectedQuality = 1;
    }
}

public void SetHighQuality()
{
    Debug.Log("Setting High Quality");
    if (qualitySettings != null)
    {
        qualitySettings.HighQ();
        AnimateQualityButtonSelection(2);
        currentSelectedQuality = 2;
    }
}

private void AnimateQualityButtonSelection(int selectedIndex)
{
    if (lowQualityButton != null)
        lowQualityButton.transform.DOScale(selectedIndex == 0 ? selectedQualityButtonScale : normalQualityButtonScale, qualityButtonScaleDuration).SetEase(qualityButtonScaleEase);
    
    if (mediumQualityButton != null)
        mediumQualityButton.transform.DOScale(selectedIndex == 1 ? selectedQualityButtonScale : normalQualityButtonScale, qualityButtonScaleDuration).SetEase(qualityButtonScaleEase);
    
    if (highQualityButton != null)
        highQualityButton.transform.DOScale(selectedIndex == 2 ? selectedQualityButtonScale : normalQualityButtonScale, qualityButtonScaleDuration).SetEase(qualityButtonScaleEase);
}
    #endregion
}