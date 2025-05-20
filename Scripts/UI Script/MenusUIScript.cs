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

    #endregion
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
        Debug.Log("Main Menu is Visible...");
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(false);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(true);
        AboutUI.SetActive(false);
    }

    #endregion



    #region Loading Levels

    // LEVELS Loading Scenes Code
    public void LoadLevel1()
    {
        Debug.Log("Loaded Level 1...");
        SceneManager.LoadScene("Level1Scene");
    }
    public void LoadLevel2()
    {
        Debug.Log("Loaded Level 2...");
        SceneManager.LoadScene("Level2Scene");
    }
    public void LoadLevel3()
    {
        Debug.Log("Loaded Level 3...");
        SceneManager.LoadScene("Level3Scene");
    }
    public void LoadLevel4()
    {
        Debug.Log("Loaded Level 4...");
        SceneManager.LoadScene("Level4Scene");
    }
    public void LoadLevel5()
    {
        Debug.Log("Loaded Level 5...");
        SceneManager.LoadScene("Level5Scene");
    }
    // End for LEVELS Loading Scenes Code

    // QUIZ Loading Scenes Code
    public void LoadMCQEasy()
    {
        Debug.Log("Loaded MCQEasy...");
        SceneManager.LoadScene("1QuizEasy");
    }
    public void LoadMCQAverage()
    {
        Debug.Log("Loaded MCQAverage...");
        SceneManager.LoadScene("2QuizAverage");
    }
    public void LoadMCQDifficult()
    {
        Debug.Log("Loaded MCQAverage...");
        SceneManager.LoadScene("3QuizDifficult");
    }
    // End for QUIZ Loading Scenes Code

    #endregion

    #region DoTween Animations
    // Animations Using Dotween

    // Main Menu UI Animation

    // On Start Menu Button Click
    public void MainMenuLowerPanelOutro4Start()
    {
        MainMenuLowerPanel.DOAnchorPosY(bottomY, tweenDuration);
        StartCoroutine(DelayedStartMainMenu(0.4f));
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



    IEnumerator DelayedStartMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowStartMenu();
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

    // END Main Menu UI Animation

    // On Start Menu Button Click

    // END OF Animations Using Dotweem

    #endregion


    #region Additional Methods

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
    
    #endregion
}