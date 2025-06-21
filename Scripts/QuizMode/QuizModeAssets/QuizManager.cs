using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class QuizManager : MonoBehaviour
{
    public LevelLoader levelLoader; // Reference to LevelLoader
    [System.Serializable]
    public class ButtonInfo
    {
        public GameObject buttonObject;
        public Vector2 originalPosition;
    }

    public List<QuestionAndAnswer> QnA;
    public GameObject[] options; // UI elements for answer options
    public int currentQuestionIndex; // Track the current question index
    public TextMeshProUGUI QuestionTxt; // UI Text element to display the question; // UI Text element to display the question

    [Header("SFX")]
    public QuizModeSFX quizModeSFX;// Reference to the QuizModeSFX script
    private bool timerWarningPlayed = false;

    // total questions count
    public int totalQuestionsCount = 0;
    public int scoreCount = 0;
    public int correctAnswersCount = 0;
    public int percentage = 0;

    private int answeredQuestionsCount = 0;

    public int maximumQuestions = 0; // Maximum number of questions to be answered in the quiz

    // Add these fields to your QuizManager class
    [Header("Question Image")]
    public GameObject imageContainer; // GameObject containing the Image component
    public Image questionImageDisplay; // Image component to display the question image


    [Header("Quiz Panel")]
    public GameObject quizPanel;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI ScoreText;

    [Header("Timer Settings")]
    public float questionTime = 15f;
    private float currentTime;
    public Coroutine timerCoroutine; // Add this field to your class so it can be accessed by PauseManager

    [Header("Summary Panel")]
    public GameObject summaryPanel;
    public TextMeshProUGUI FinalScore;
    public TextMeshProUGUI TotalAnsweredQuestions;
    public TextMeshProUGUI Percentage;
    public TextMeshProUGUI Label;
    public GameObject starContainersShadow;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject retryButton;
    public GameObject homeButton;

    [Header("Feedback")]
    public GameObject nextButton;
    public bool isShowingFeedback = false;

    [Header("MixAndMatch Mini-Game")]
    public MixAndMatch mixAndMatchManager; // Reference to the MixAndMatch script
    private bool miniGamePlayed = false; // Track if mini-game has been played

    [Header("Summary Animation Settings")]
    public Vector2 summaryStartPosition = new Vector2(0, 500); // Start position off-screen
    public Vector2 summaryFinalPosition = new Vector2(0, 0); // Final centered position
    public float slideDuration = 0.7f; // How long the panel takes to slide in
    public float elementFadeDuration = 0.5f; // How long each element takes to fade in
    public float delayBetweenElements = 0.2f; // Delay between each element fading in
    public Ease slideEaseType = Ease.OutBack; // Easing for the panel slide
    public Ease fadeEaseType = Ease.OutQuad; // Easing for fading elements

    [Header("Star Thresholds")]
    public int star1Threshold = 300; // Score needed for first star
    public int star2Threshold = 600; // Score needed for second star
    public int star3Threshold = 900; // Score needed for third star
    public int star4Threshold = 1200; // Score needed for fourth star (optional)
    public GameObject star4; // Reference to the fourth star (optional)

    [Header("Button Animation Settings")]
    public float correctButtonScaleMultiplier = 1.2f; // Button will scale to 120% of original size
    public float buttonAnimationDuration = 0.3f;      // How long the animation takes
    public Ease buttonAnimationEase = Ease.OutBack;   // The easing function for smooth animation

    // Add this field at the top of your class to store original button scales
    private Dictionary<GameObject, Vector3> originalButtonScales = new Dictionary<GameObject, Vector3>();

    // Add these new fields
    private bool isTimerPaused = false;
    private float pausedTimeRemaining = 0f;

    [Header("Pause Controls")]
public Button pauseButton; // Pause button for Quiz mode

    // Add these fields to the QuizManager class
    [Header("Instruction Panel")]
    public GameObject instructionPanel;
    public GameObject instructionLabelContainer; // Container GameObject for the label
    public GameObject instructionTextContainer; // Container GameObject for instructions
    public Button startQuizButton;
    public float instructionFadeDuration = 0.5f;
    public float instructionDelayBetweenElements = 0.3f;

    // Track if the quiz has started
    private bool quizStarted = false;

    // Add this field to your QuizManager class
    [Header("Panel Movement")]
public PanelMovement panelMovementScript;

    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        totalQuestionsCount = QnA.Count;

        // Store original button scales
        foreach (var option in options)
        {
            originalButtonScales[option] = option.transform.localScale;
        }

        // Initialize the summary panel components
        InitializeSummaryPanel();

        // Make sure summary panel is hidden
        summaryPanel.SetActive(false);

        // Set initial score text
        ScoreText.text = "Score: " + scoreCount;

        // Show the instruction panel instead of starting the quiz immediately
        ShowInstructionPanel();

        // Register pause button with PauseManager
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null && pauseButton != null)
        {
            pauseManager.RegisterPauseButton(pauseButton);
        }
    }

    // Add this method to show the instruction panel
    private void ShowInstructionPanel()
    {
        // Hide the quiz panel initially
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }

        // Show instruction panel
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
            
            // Set up button click event
            if (startQuizButton != null)
            {
                startQuizButton.onClick.RemoveAllListeners();
                startQuizButton.onClick.AddListener(StartQuiz);
                
                // Hide button initially (will be faded in)
                CanvasGroup buttonGroup = startQuizButton.GetComponent<CanvasGroup>();
                if (buttonGroup == null)
                    buttonGroup = startQuizButton.gameObject.AddComponent<CanvasGroup>();
                buttonGroup.alpha = 0;
                buttonGroup.interactable = false;
            }
            
            // Ensure all instruction elements have CanvasGroups
            EnsureInstructionElementsHaveCanvasGroups();
            
            // Hide all elements initially
            SetInstructionElementsAlpha(0);
            
            // Start animation sequence
            StartCoroutine(AnimateInstructionPanel());
        }
    }

    // Add this method to start the quiz when the start button is clicked
    public void StartQuiz()
    {
        // Set quiz started flag
        quizStarted = true;
        
        // Hide instruction panel
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
        
        // Show quiz panel
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
        }
        
        // Generate first question
        generateQuestion();
    }

    // Add this method to animate the instruction panel elements
    private IEnumerator AnimateInstructionPanel()
    {
        // Wait a moment before starting animations
        yield return new WaitForSeconds(0.5f);
        
        // 1. Fade in the label container first
        if (instructionLabelContainer != null)
        {
            CanvasGroup labelGroup = instructionLabelContainer.GetComponent<CanvasGroup>();
            if (labelGroup == null)
                labelGroup = instructionLabelContainer.AddComponent<CanvasGroup>();
            
            // Make sure alpha is 0 to start
            labelGroup.alpha = 0;
            labelGroup.DOFade(1, instructionFadeDuration).SetEase(fadeEaseType);
            yield return new WaitForSeconds(instructionFadeDuration + instructionDelayBetweenElements);
        }
        
        // 2. Fade in the instruction text container
        if (instructionTextContainer != null)
        {
            CanvasGroup textGroup = instructionTextContainer.GetComponent<CanvasGroup>();
            if (textGroup == null)
                textGroup = instructionTextContainer.AddComponent<CanvasGroup>();
            
            // Make sure alpha is 0 to start
            textGroup.alpha = 0;
            textGroup.DOFade(1, instructionFadeDuration).SetEase(fadeEaseType);
            yield return new WaitForSeconds(instructionFadeDuration + instructionDelayBetweenElements);
        }
        
        // 3. Fade in the start button - Fixed approach
        if (startQuizButton != null)
        {
            // Get or add the CanvasGroup component
            CanvasGroup buttonGroup = startQuizButton.GetComponent<CanvasGroup>();
            if (buttonGroup == null)
                buttonGroup = startQuizButton.gameObject.AddComponent<CanvasGroup>();
            
            // Ensure button starts completely invisible and non-interactive
            buttonGroup.alpha = 0;
            buttonGroup.interactable = false;
            buttonGroup.blocksRaycasts = false;
            
            // Wait a frame to ensure UI updates
            yield return null;
            
            // Use DOTween to fade in, set this to a variable so we can track when it completes
            Tween fadeTween = buttonGroup.DOFade(1, instructionFadeDuration)
                .SetEase(fadeEaseType);
            
            // Wait for the animation to complete
            yield return fadeTween.WaitForCompletion();
            
            // Make button interactive after animation completes
            buttonGroup.interactable = true;
            buttonGroup.blocksRaycasts = true;
            
            Debug.Log("Start button fade animation completed");
        }
    }

    // Helper methods for instruction panel
    private void EnsureInstructionElementsHaveCanvasGroups()
    {
        if (instructionLabelContainer != null && instructionLabelContainer.GetComponent<CanvasGroup>() == null)
            instructionLabelContainer.AddComponent<CanvasGroup>();
            
        if (instructionTextContainer != null && instructionTextContainer.GetComponent<CanvasGroup>() == null)
            instructionTextContainer.AddComponent<CanvasGroup>();
            
        if (startQuizButton != null && startQuizButton.GetComponent<CanvasGroup>() == null)
            startQuizButton.gameObject.AddComponent<CanvasGroup>();
    }

    private void SetInstructionElementsAlpha(float alpha)
    {
        SetElementAlpha(instructionLabelContainer, alpha);
        SetElementAlpha(instructionTextContainer, alpha);
        SetElementAlpha(startQuizButton?.gameObject, alpha);
    }

    // Call this method to start the timer for a question
    private void StartTimer()
    {
         timerWarningPlayed = false;
        // Stop any existing timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Start a new timer
        timerCoroutine = StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        // Initialize timer
        currentTime = questionTime;

        // Update timer UI
        if (TimerText != null)
        {
            TimerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        // Count down until time runs out
        while (currentTime > 0 && !isShowingFeedback)
        {
            // Wait for a frame
            yield return null;

            // Decrease time
            currentTime -= Time.deltaTime;

            // Update UI
            if (TimerText != null)
            {
                // Only show whole seconds
                TimerText.text = Mathf.CeilToInt(currentTime).ToString();

                // Optional: change color when time is running out
                if (currentTime <= 5)
                {
                    if (!timerWarningPlayed && quizModeSFX != null)
                {
                    quizModeSFX.PlayTimerWarning();
                    timerWarningPlayed = true;
                }
                    TimerText.color = Color.red;
                }
                else
                {

                    TimerText.color = Color.white;
                }
            }
        }

        // Time's up if we didn't already answer
        if (!isShowingFeedback)
        {
            quizModeSFX.StopWarning();
            TimeUp();
        }
    }

    // Implement the TimeUp method
    public void TimeUp()
    {
        // Display timeout message using QuestionTxt
        int correctIndex = QnA[currentQuestionIndex].CorrectAnswer - 1;
        QuestionTxt.text = "Time's up! The correct answer was: " +
                        QnA[currentQuestionIndex].Answers[correctIndex];

        // Disable all buttons
        foreach (var option in options)
        {
            option.GetComponent<Button>().interactable = false;
        }

        // Show the next button
        nextButton.SetActive(true);

        // Set feedback state
        isShowingFeedback = true;
    }


        public void retryQuiz()
        {
        // Make sure we have a reference to the LevelLoader
        if (levelLoader == null)
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            
            if (levelLoader == null)
            {
                Debug.LogWarning("LevelLoader not found, using direct scene loading instead");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }
        }
        
        // Use LevelLoader's coroutine to reload the current scene with transition
        StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().name));
    }

    public void goToMainMenu()
    {
        if (levelLoader == null)
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            
            if (levelLoader == null)
            {
                Debug.LogWarning("LevelLoader not found, using direct scene loading instead");
                SceneManager.LoadScene("MainMenuScene");
                return;
            }
        }
        
        // Fix: Don't use StartCoroutine with levelLoader's method
        // Instead, call the LoadMainMenu method directly
        levelLoader.LoadMainMenu();
    }
    public void gameOver()
    {
        
        // Stop any running timers
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Keep the quiz panel active but disable interaction with its buttons
        foreach (var option in options)
        {
            if (option != null && option.GetComponent<Button>() != null)
            {
                option.GetComponent<Button>().interactable = false;
            }
        }

        // Hide the next button if it's visible
        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        // Activate summary panel but make elements invisible
        summaryPanel.SetActive(true);

        // Update summary panel texts (they'll be invisible initially)
        FinalScore.text = "Final Score: " + scoreCount.ToString();
        TotalAnsweredQuestions.text = correctAnswersCount.ToString() + " / " + answeredQuestionsCount;

        // Calculate percentage with one decimal place
        float percentValue = (answeredQuestionsCount > 0) ?
            ((float)correctAnswersCount / answeredQuestionsCount * 100) : 0;
        Percentage.text = percentValue.ToString("F1") + "%";

        // Set label text based on score
        if (percentValue >= 90)
            Label.text = "Excellent!" + "\nNice Work!";
        else if (percentValue >= 75)
            Label.text = "Great Job! \nYou did well!";
        else if (percentValue >= 60)
            Label.text = "Good Work!" + "\nWell Done!";
        else if (percentValue >= 40)
            Label.text = "Not Bad!" + "\nKeep Trying!";
        else
            Label.text = "Keep Practicing!";

        // Initialize the elements as invisible
        CanvasGroup panelGroup = summaryPanel.GetComponent<CanvasGroup>();
        if (panelGroup == null)
            panelGroup = summaryPanel.AddComponent<CanvasGroup>();

        // Make sure panel is fully visible but elements are hidden
        panelGroup.alpha = 1f;

        // Hide individual elements
        SetElementAlpha(FinalScore.gameObject, 0);
        SetElementAlpha(TotalAnsweredQuestions.gameObject, 0);
        SetElementAlpha(Percentage.gameObject, 0);
        SetElementAlpha(Label.gameObject, 0);
        SetElementAlpha(starContainersShadow, 0);
        SetElementAlpha(star1, 0);
        SetElementAlpha(star2, 0);
        SetElementAlpha(star3, 0);
        if (star4 != null)
            SetElementAlpha(star4, 0);
            
        SetElementAlpha(retryButton, 0);
        SetElementAlpha(homeButton, 0);

        // Position the panel at the start position
        RectTransform panelRect = summaryPanel.GetComponent<RectTransform>();
        panelRect.anchoredPosition = summaryStartPosition;

        // Start the animation sequence
        StartCoroutine(AnimateSummaryPanel());
        
        // Award currency based on quiz performance - add this line
        AwardCurrencyForQuizCompletion();
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

    private IEnumerator AnimateSummaryPanel()
    {
        // Get the panel's RectTransform
        RectTransform panelRect = summaryPanel.GetComponent<RectTransform>();

        // 1. Slide panel from top to final position
        panelRect.DOAnchorPos(summaryFinalPosition, slideDuration)
            .SetEase(slideEaseType);

        // Wait for slide to complete
        yield return new WaitForSeconds(slideDuration);

        // 2. Fade in the label AND shadow container SIMULTANEOUSLY
        Label.gameObject.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);
            
        // Fade in star container shadow at the same time as the label
        starContainersShadow.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);

        yield return new WaitForSeconds(delayBetweenElements * 1.5f);
        
        // 3. Fade in score text
        FinalScore.gameObject.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);

        yield return new WaitForSeconds(delayBetweenElements);

        // 4. Fade in total answered questions
        TotalAnsweredQuestions.gameObject.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);

        yield return new WaitForSeconds(delayBetweenElements);

        // 5. Fade in percentage
        Percentage.gameObject.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);

        yield return new WaitForSeconds(delayBetweenElements);

        // Debug the star thresholds and current score
        Debug.Log($"Star thresholds: {star1Threshold}, {star2Threshold}, {star3Threshold}, {star4Threshold}");
        Debug.Log($"Current score: {scoreCount}");
        
        // 7. Fade in stars based on score thresholds - FIXED VERSION
        // First star
        if (scoreCount >= star1Threshold)
        {
            CanvasGroup star1Group = star1.GetComponent<CanvasGroup>();
            if (star1Group != null)
            {
                Debug.Log("Fading in star 1");
                star1Group.alpha = 0; // Ensure it starts invisible
                star1Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                yield return new WaitForSeconds(delayBetweenElements);
            }
            else
            {
                Debug.LogError("Star 1 is missing CanvasGroup component!");
            }
        }

        // Second star
        if (scoreCount >= star2Threshold)
        {
            CanvasGroup star2Group = star2.GetComponent<CanvasGroup>();
            if (star2Group != null)
            {
                Debug.Log("Fading in star 2");
                star2Group.alpha = 0; // Ensure it starts invisible
                star2Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                yield return new WaitForSeconds(delayBetweenElements);
            }
            else
            {
                Debug.LogError("Star 2 is missing CanvasGroup component!");
            }
        }

        // Third star
        if (scoreCount >= star3Threshold)
        {
            CanvasGroup star3Group = star3.GetComponent<CanvasGroup>();
            if (star3Group != null)
            {
                Debug.Log("Fading in star 3");
                star3Group.alpha = 0; // Ensure it starts invisible
                star3Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                yield return new WaitForSeconds(delayBetweenElements);
            }
            else
            {
                Debug.LogError("Star 3 is missing CanvasGroup component!");
            }
        }

        // Fourth star - only if it exists
        if (star4 != null && scoreCount >= star4Threshold)
        {
            CanvasGroup star4Group = star4.GetComponent<CanvasGroup>();
            if (star4Group != null)
            {
                Debug.Log("Fading in star 4");
                star4Group.alpha = 0; // Ensure it starts invisible
                star4Group.DOFade(1, elementFadeDuration).SetEase(fadeEaseType);
                
                // Optional special effect for highest star
                star4.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 5, 0.5f);
                
                yield return new WaitForSeconds(delayBetweenElements);
            }
            else
            {
                Debug.LogError("Star 4 is missing CanvasGroup component!");
            }
        }

        // Wait a bit longer before showing buttons
        yield return new WaitForSeconds(delayBetweenElements * 2);

        // 8. Fade in buttons (no scaling)
        retryButton.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);

        yield return new WaitForSeconds(delayBetweenElements * 0.5f);

        homeButton.GetComponent<CanvasGroup>().DOFade(1, elementFadeDuration)
            .SetEase(fadeEaseType);

        // Animation sequence complete!
    }

    public void correct()
    {
        // Clear previous selection state
        foreach (var option in options)
        {
            option.GetComponent<AnswerScript>().wasSelected = false;
        }

        // Set the current option as selected
        GameObject selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (selectedButton != null)
        {
            AnswerScript answerScript = selectedButton.GetComponent<AnswerScript>();
            if (answerScript != null)
            {
                answerScript.wasSelected = true;
            }
        }

        // Track old score for animation
        int oldScore = scoreCount;
        
        // Update internal score values
        correctAnswersCount++;
        scoreCount += 100;

        // Animate the score text counting up
        if (ScoreText != null)
            AnimateScoreIncrease(oldScore, scoreCount);

        // Show feedback
        ShowFeedback(true);
    }

    // New method to animate score counting up
    private void AnimateScoreIncrease(int fromValue, int toValue)
    {
        // First create a visual "popup" effect for the score text
        ScoreText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 5, 0.5f);
        
        // Then animate the score counting up
        DOTween.To(() => fromValue, x => {
            // Update UI on each step of the animation
            ScoreText.text = "Score: " + x.ToString();
        }, toValue, 0.75f).SetEase(Ease.OutCubic);
        
        // Create a floating "+100" text that moves upward and fades out
        CreateScorePopup("+100", ScoreText.transform.position, Color.green);
    }

    // Creates a floating score popup
    private void CreateScorePopup(string text, Vector3 position, Color color)
    {
        // Create temporary text game object
        GameObject tempTextObj = new GameObject("ScorePopup");
        tempTextObj.transform.SetParent(ScoreText.transform.parent);
        tempTextObj.transform.position = position;
        
        // Add text component
        TextMeshProUGUI popupText = tempTextObj.AddComponent<TextMeshProUGUI>();
        popupText.text = text;
        popupText.fontSize = ScoreText.fontSize * 1.2f;
        popupText.fontStyle = FontStyles.Bold;
        popupText.color = color;
        popupText.alignment = TextAlignmentOptions.Center;
        
        // Set its position slightly above the score text
        RectTransform rectTransform = tempTextObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = ScoreText.rectTransform.anchoredPosition + new Vector2(0, -30);
        
        // Create animation sequence
        Sequence popupSequence = DOTween.Sequence();
        
        // Scale up slightly
        popupSequence.Append(tempTextObj.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
        
        // Move upward
        popupSequence.Join(rectTransform.DOAnchorPos(
            rectTransform.anchoredPosition + new Vector2(0, 60), 1.0f)
            .SetEase(Ease.OutCubic));
        
        // Fade out
        popupSequence.Join(popupText.DOFade(0, 0.8f).SetDelay(0.2f));
        
        // Destroy when complete
        popupSequence.OnComplete(() => {
            Destroy(tempTextObj);
        });
    }



    public void wrong()
    {
        // Clear previous selection state
        foreach (var option in options)
        {
            option.GetComponent<AnswerScript>().wasSelected = false;
        }

        // Set the current option as selected
        GameObject selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (selectedButton != null)
        {
            AnswerScript answerScript = selectedButton.GetComponent<AnswerScript>();
            if (answerScript != null)
            {
                answerScript.wasSelected = true;
            }
        }

        // Show feedback
        ShowFeedback(false);
    }


    private void ShowFeedback(bool isCorrect)
    {
        isShowingFeedback = true;

        if(quizModeSFX != null)
        {
            quizModeSFX.StopWarning();
        }

        // Disable all answer buttons to prevent multiple answers
        foreach (var option in options)
        {
            option.GetComponent<Button>().interactable = false;
        }

        // Hide the question image during feedback
        if (imageContainer != null)
        {
            imageContainer.SetActive(false);
        }

        // Get the appropriate feedback message from the current question
        string feedback;
        int correctIndex = QnA[currentQuestionIndex].CorrectAnswer - 1;

        // Display feedback in the QuestionTxt immediately
        if (isCorrect)
        {
            feedback = QnA[currentQuestionIndex].correctExplanation;
            // Set text color to green for correct answers
            QuestionTxt.color = new Color(0.2f, 0.8f, 0.2f); // Green color
        }
        else
        {
            // For wrong answers, show the incorrect explanation and the correct answer
            feedback = QnA[currentQuestionIndex].incorrectExplanation;
            feedback += "\n\nThe correct answer was: " + QnA[currentQuestionIndex].Answers[correctIndex];

            // Set text color to red for incorrect answers
            QuestionTxt.color = new Color(0.9f, 0.2f, 0.2f); // Red color
        }

        // Update the question text with the feedback
        QuestionTxt.text = feedback;

        // Set text alignment to center during feedback (since image is hidden)
        QuestionTxt.alignment = TextAlignmentOptions.Center;

        // Ensure next button is initially hidden but active
        nextButton.SetActive(true);
        CanvasGroup nextButtonCanvasGroup = nextButton.GetComponent<CanvasGroup>();
        if (nextButtonCanvasGroup == null)
            nextButtonCanvasGroup = nextButton.AddComponent<CanvasGroup>();
        
        nextButtonCanvasGroup.alpha = 0;
        nextButtonCanvasGroup.interactable = false;
        nextButtonCanvasGroup.blocksRaycasts = false;

        // Create a main sequence for all animations
        Sequence mainSequence = DOTween.Sequence();

        if (isCorrect)
        {
            // Create a separate sequence for the button animation
            Sequence buttonSequence = DOTween.Sequence();
            AnimateCorrectButton(options[correctIndex], buttonSequence);
            
            // Add the button animation to the main sequence
            mainSequence.Append(buttonSequence);
        }
        else
        {
            // Find which button was selected
            int selectedButtonIndex = -1;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].GetComponent<AnswerScript>().wasSelected)
                {
                    selectedButtonIndex = i;
                    break;
                }
            }

            // First shake the wrong button that was selected
            if (selectedButtonIndex >= 0)
            {
                Sequence wrongButtonSequence = DOTween.Sequence();
                ShakeWrongButton(options[selectedButtonIndex], wrongButtonSequence);
                mainSequence.Append(wrongButtonSequence);
            }

            // Then animate the correct button (with a slight delay)
            mainSequence.AppendInterval(0.3f);
            Sequence correctButtonSequence = DOTween.Sequence();
            AnimateCorrectButton(options[correctIndex], correctButtonSequence);
            mainSequence.Append(correctButtonSequence);
        }

        // After all animations are complete, fade in the next button
        mainSequence.OnComplete(() => {
            // Fade in the next button
            nextButtonCanvasGroup.DOFade(1, 0.3f).OnComplete(() => {
                // Enable interaction after fade completes
                nextButtonCanvasGroup.interactable = true;
                nextButtonCanvasGroup.blocksRaycasts = true;
            });
        });

        // Play the sequence
        mainSequence.Play();
    }

    public void ProceedToNextQuestion()
    {
        // Reset text color back to normal
        QuestionTxt.color = Color.white;

        // Hide the next button
        nextButton.SetActive(false);

        // Re-enable and reset all buttons (even those deactivated)
        foreach (var option in options)
        {
            // Make all buttons active first (we'll deactivate unused ones in SetAnswer)
            option.SetActive(true);
            
            // Re-enable button
            option.GetComponent<Button>().interactable = true;

            // Reset selection state
            option.GetComponent<AnswerScript>().wasSelected = false;

            // Reset button color
            Image buttonImage = option.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = Color.white; // Or your default button color
            }

            // Reset to original scale
            if (originalButtonScales.ContainsKey(option))
            {
                option.transform.localScale = originalButtonScales[option];
            }
            else
            {
                // Fallback to Vector3.one
                option.transform.localScale = Vector3.one;
            }
        }

        // If we just showed feedback, generate a new question
        if (isShowingFeedback)
        {
            // Remove current question
            QnA.RemoveAt(currentQuestionIndex);

            // Increment answered questions counter
            answeredQuestionsCount++;

            // Reset feedback state
            isShowingFeedback = false;

            // Move to next question - this will update QuestionTxt with the new question text
            generateQuestion();
        }
    }



    void SetAnswer()
    {
        // Get how many answers this question has
        int answerCount = QnA[currentQuestionIndex].Answers.Length;
        
        // First make all buttons active
        foreach (var option in options)
        {
            option.SetActive(true);
        }
        
        // Set up the buttons we need
        for (int i = 0; i < options.Length; i++)
        {
            // Reset button state
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            
            if (i < answerCount)
            {
                // This button is needed for this question
                options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestionIndex].Answers[i];
                
                if (QnA[currentQuestionIndex].CorrectAnswer == i + 1)
                {
                    options[i].GetComponent<AnswerScript>().isCorrect = true;
                }
            }
            else
            {
                // Deactivate unused buttons
                options[i].SetActive(false);
            }
        }

        // Now shuffle the positions of the active buttons
        RandomizeButtonPositions();
    }

    private void RandomizeButtonPositions()
    {
        // First, ensure all buttons are active (we'll deactivate unused ones after)
        foreach (var option in options)
        {
            option.SetActive(true);
        }

        // Store original positions of all buttons
        Vector3[] originalPositions = new Vector3[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            originalPositions[i] = options[i].GetComponent<RectTransform>().anchoredPosition;
        }

        // Create a shuffled order of indices
        List<int> shuffledIndices = new List<int>();
        for (int i = 0; i < options.Length; i++)
        {
            shuffledIndices.Add(i);
        }
        ShuffleList(shuffledIndices);

        // Apply shuffled positions to buttons
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<RectTransform>().anchoredPosition = originalPositions[shuffledIndices[i]];
        }

        // Check if current question has fewer answers than max buttons
        int answerCount = QnA[currentQuestionIndex].Answers.Length;
        if (answerCount < options.Length)
        {
            // Deactivate unused buttons
            for (int i = answerCount; i < options.Length; i++)
            {
                options[i].SetActive(false);
            }
            
            Debug.Log($"Question has only {answerCount} answers. Deactivated {options.Length - answerCount} buttons.");
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void generateQuestion()
    {
        // Don't start timer if quiz hasn't officially started
        if (!quizStarted)
            return;
    
        if (answeredQuestionsCount >= maximumQuestions)
        {
            // End the quiz if we've answered enough questions
            gameOver();
            return;
        }

        // Check if we've reached halfway point
        if (!miniGamePlayed && answeredQuestionsCount >= (maximumQuestions / 2))
        {
            // We've reached half of the questions, trigger mini-game
            StartMiniGame();
            return;
        }

        if (QnA.Count > 0)
        {
            currentQuestionIndex = Random.Range(0, QnA.Count);
            
            // Debug the question and its answer count
            Debug.Log($"Generating question: {QnA[currentQuestionIndex].Question}");
            Debug.Log($"This question has {QnA[currentQuestionIndex].Answers.Length} answers");
            
            QuestionTxt.text = QnA[currentQuestionIndex].Question;

            // Handle image display
            if (imageContainer != null && questionImageDisplay != null)
            {
                if (QnA[currentQuestionIndex].hasImage && QnA[currentQuestionIndex].questionImage != null)
                {
                    // Show image container and set the sprite
                    imageContainer.SetActive(true);
                    questionImageDisplay.sprite = QnA[currentQuestionIndex].questionImage;
                    QuestionTxt.alignment = TextAlignmentOptions.Left;
                }
                else
                {
                    // Hide image container if no image for this question
                    imageContainer.SetActive(false);
                    QuestionTxt.alignment = TextAlignmentOptions.Center;
                }
            }

            SetAnswer();

            // Start the timer for this question
            StartTimer();
        }
        else
        {
            // End the quiz if we've run out of questions
            gameOver();
        }
    }

    private void StartMiniGame()
    {
        // Set flag so we only play the mini-game once
        miniGamePlayed = true;

        // Pause the timer if it's running
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // Start the mini-game transition
        if (mixAndMatchManager != null)
        {
            mixAndMatchManager.StartMiniGame();
        }
        else
        {
            Debug.LogError("MixAndMatch reference not set in QuizManager!");
        }
    }

    public void ResumeQuiz()
    {
        // Activate quiz panel (the MixAndMatch script will handle this)

        // Restart panel movement animation if it exists
        if (panelMovementScript != null)
        {
            panelMovementScript.StartContinuousMovement();
        }

        // Continue with the next question
        generateQuestion();
    }

    public void AddMiniGameScore(int points)
    {
        // Track old score for animation
        int oldScore = scoreCount;
        
        // Add mini-game points to the quiz score
        scoreCount += points;

        // Update score display with animation
        if (ScoreText != null)
        {
            AnimateScoreIncrease(oldScore, scoreCount);
        }

        Debug.Log($"Mini-game score added: {points} points. New total score: {scoreCount}");
    }
    private IEnumerator ShowScoreAddedEffect(int points)
    {
        // Create temporary text to show points being added
        GameObject tempTextObj = new GameObject("TempScoreText");
        tempTextObj.transform.SetParent(ScoreText.transform.parent);

        TextMeshProUGUI tempText = tempTextObj.AddComponent<TextMeshProUGUI>();
        tempText.text = "+" + points;
        tempText.fontSize = ScoreText.fontSize;
        tempText.color = Color.green;

        // Position next to score text
        RectTransform tempRect = tempTextObj.GetComponent<RectTransform>();
        tempRect.anchoredPosition = ScoreText.rectTransform.anchoredPosition + new Vector2(0, -30);

        // Animate the text
        float duration = 1.0f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, time / duration);
            tempText.color = new Color(0, 1, 0, alpha);
            tempRect.anchoredPosition += new Vector2(0, Time.deltaTime * 30); // Drift upward

            yield return null;
        }

        // Destroy the temporary text
        Destroy(tempTextObj);
    }

    private void InitializeSummaryPanel()
    {
        // Make sure the summary panel is initially inactive
        if (summaryPanel != null)
        {
            summaryPanel.SetActive(false);

            // Make sure all elements have CanvasGroups
            if (FinalScore != null && FinalScore.gameObject.GetComponent<CanvasGroup>() == null)
                FinalScore.gameObject.AddComponent<CanvasGroup>();

            if (TotalAnsweredQuestions != null && TotalAnsweredQuestions.gameObject.GetComponent<CanvasGroup>() == null)
                TotalAnsweredQuestions.gameObject.AddComponent<CanvasGroup>();

            if (Percentage != null && Percentage.gameObject.GetComponent<CanvasGroup>() == null)
                Percentage.gameObject.AddComponent<CanvasGroup>();

            if (Label != null && Label.gameObject.GetComponent<CanvasGroup>() == null)
                Label.gameObject.AddComponent<CanvasGroup>();

            if (starContainersShadow != null && starContainersShadow.GetComponent<CanvasGroup>() == null)
                starContainersShadow.AddComponent<CanvasGroup>();

            if (star1 != null && star1.GetComponent<CanvasGroup>() == null)
                star1.AddComponent<CanvasGroup>();

            if (star2 != null && star2.GetComponent<CanvasGroup>() == null)
                star2.AddComponent<CanvasGroup>();

            if (star3 != null && star3.GetComponent<CanvasGroup>() == null)
                star3.AddComponent<CanvasGroup>();
            
            // Add this for the 4th star - check if it exists first
            if (star4 != null && star4.GetComponent<CanvasGroup>() == null)
                star4.AddComponent<CanvasGroup>();

            if (retryButton != null && retryButton.GetComponent<CanvasGroup>() == null)
                retryButton.AddComponent<CanvasGroup>();

            if (homeButton != null && homeButton.GetComponent<CanvasGroup>() == null)
                homeButton.AddComponent<CanvasGroup>();
        }
    }

    private void AnimateCorrectButton(GameObject button, Sequence parentSequence)
    {
        // Store original scale
        Vector3 originalScale = button.transform.localScale;

        // Create a new sequence for this animation
        Sequence popSequence = DOTween.Sequence();

        // Change to green
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            popSequence.Append(buttonImage.DOColor(Color.green, 0.2f));

            // Return to original color
            popSequence.Append(buttonImage.DOColor(originalColor, 0.5f));
        }

        // Pop forward (scale up and back)
        popSequence.Join(button.transform.DOScale(originalScale * correctButtonScaleMultiplier, 0.3f)
            .SetEase(Ease.OutBack));

        // Return to original scale
        popSequence.Append(button.transform.DOScale(originalScale, 0.2f)
            .SetEase(Ease.InOutQuad));

        // Add this animation to the parent sequence
        parentSequence.Append(popSequence);
    }

    private void ShakeWrongButton(GameObject button, Sequence parentSequence)
    {
        // Create a new sequence for this animation
        Sequence shakeSequence = DOTween.Sequence();

        // Change to red
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            shakeSequence.Append(buttonImage.DOColor(Color.red, 0.2f));

            // Return to original color
            shakeSequence.Append(buttonImage.DOColor(originalColor, 0.5f));
        }

        // Simple left-right shake
        shakeSequence.Join(button.transform.DOShakePosition(0.5f, new Vector3(15, 0, 0), 10, 90, false, false));

        // Add this animation to the parent sequence
        parentSequence.Append(shakeSequence);
    }

    // Add this method to resume timing after pause
    public void ResumeTiming()
    {
        // Only resume if we were showing a question (not feedback)
        if (!isShowingFeedback)
        {
            Debug.Log($"Resuming quiz timer with {pausedTimeRemaining} seconds remaining");
            
            // Start a new timer from the stored time (not resetting to questionTime)
            timerCoroutine = StartCoroutine(ResumeCountdownTimer());
        }
    }

    // Add this new coroutine method for resuming the timer
    private IEnumerator ResumeCountdownTimer()
    {
        // Use the stored time if we're resuming from pause
        if (isTimerPaused)
        {
            currentTime = pausedTimeRemaining;
            isTimerPaused = false;
        }
        else
        {
            // Initialize timer if not resuming from pause
            currentTime = questionTime;
        }
        timerWarningPlayed = false;

        // Update timer UI
        if (TimerText != null)
        {
            TimerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        // Continue the rest of your timer code as normal
        while (currentTime > 0 && !isShowingFeedback)
        {
            yield return null;
            currentTime -= Time.deltaTime;
            
            if (TimerText != null)
            {
                TimerText.text = Mathf.CeilToInt(currentTime).ToString();
                
                if (currentTime <= 5)
                    TimerText.color = Color.red;
                    if (!timerWarningPlayed && quizModeSFX != null)
                        {
                            quizModeSFX.PlayTimerWarning();
                            timerWarningPlayed = true;
                        }
                else
                    TimerText.color = Color.white;
            }
        }

        if (!isShowingFeedback)
        {
            quizModeSFX.StopWarning();
            TimeUp();
        }
    }

    // Modify your LoadMainMenu method or add if missing
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // Add the new PauseTimer method
    public void PauseTimer()
    {
        // Store the current time remaining
        pausedTimeRemaining = currentTime;
        
        // Set pause flag
        isTimerPaused = true;
        
        // Stop the coroutine
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        
        Debug.Log($"Quiz timer paused with {pausedTimeRemaining} seconds remaining");
    }

    // Add this method to your QuizManager class at the end of the gameOver() method
private void AwardCurrencyForQuizCompletion()
{
    // Calculate stars earned based on score thresholds
    int starsEarned = 0;
    
    if (scoreCount >= star1Threshold)
        starsEarned++;
    
    if (scoreCount >= star2Threshold)
        starsEarned++;
    
    if (scoreCount >= star3Threshold)
        starsEarned++;
    
    if (star4 != null && scoreCount >= star4Threshold)
        starsEarned++;
    
    // Log the stars earned for debugging
    Debug.Log($"Quiz completed! Player earned {starsEarned} stars that will be added to currency.");
    
    // Add stars to player currency
    if (PlayerManager.Instance != null)
    {
        // Get current currency before adding
        int previousCurrency = PlayerManager.Instance.GetCurrency();
        
        // Add the stars to player's currency
        PlayerManager.Instance.AddCurrency(starsEarned);
        
        // Get new currency for display
        int newCurrency = PlayerManager.Instance.GetCurrency();
        
        Debug.Log($"Stars added to player currency: {starsEarned}. Previous: {previousCurrency}, New: {newCurrency}");
    }
    else
    {
        Debug.LogWarning("PlayerManager not found! Stars earned were not added to currency.");
    }
}
}

