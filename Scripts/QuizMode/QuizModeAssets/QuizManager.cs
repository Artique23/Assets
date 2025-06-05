using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
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
    private Coroutine timerCoroutine;

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

    private void Start()
    {
        totalQuestionsCount = QnA.Count;
        summaryPanel.SetActive(false);
        ScoreText.text = "Score: " + scoreCount;
        generateQuestion();
    }

    // Call this method to start the timer for a question
    private void StartTimer()
    {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void gameOver()
    {
        quizPanel.SetActive(false); // Hide the quiz panel
        summaryPanel.SetActive(true);
        
        // Update summary panel texts
        FinalScore.text = "Final Score: " + scoreCount.ToString(); // Display the final score (including mini-game)
        Percentage.text = ((float)correctAnswersCount / answeredQuestionsCount * 100).ToString("F1") + "%"; // Display the percentage
        TotalAnsweredQuestions.text = correctAnswersCount.ToString() + " / " + answeredQuestionsCount; // Display correct/total
        
        // Optionally add text showing mini-game contribution
        // Create a new TextMeshProUGUI component for this if needed
    }

    public void correct()
    {
        correctAnswersCount++;
        scoreCount += 100;

        // Update score display
        if (ScoreText != null)
            ScoreText.text = "Score: " + scoreCount;

        // Show feedback
        ShowFeedback(true);
    }



    public void wrong()
    {
        // Just show feedback - don't remove the question or generate new one yet
        ShowFeedback(false);

        // We'll handle question removal and generation in ProceedToNextQuestion
    }


    private void ShowFeedback(bool isCorrect)
    {
        isShowingFeedback = true;

        // Disable all answer buttons to prevent multiple answers
        foreach (var option in options)
        {
            option.GetComponent<Button>().interactable = false;
        }

        // Get the appropriate feedback message from the current question
        string feedback;
        if (isCorrect)
        {
            feedback = QnA[currentQuestionIndex].correctExplanation;
        }
        else
        {
            // For wrong answers, show the incorrect explanation and the correct answer
            int correctIndex = QnA[currentQuestionIndex].CorrectAnswer - 1;
            feedback = QnA[currentQuestionIndex].incorrectExplanation;
            feedback += "\n\nThe correct answer was: " + QnA[currentQuestionIndex].Answers[correctIndex];
        }

        // Display feedback in the QuestionTxt instead of a separate feedback text
        QuestionTxt.text = feedback;

        // Show the next button
        nextButton.SetActive(true);

        // Since we're using QuestionTxt, we don't need the feedbackText object anymore
        // Remove or comment out the following lines:
        // feedbackText.gameObject.SetActive(true);
        // feedbackText.text = feedback;
    }

    public void ProceedToNextQuestion()
    {
        // Hide the next button
        nextButton.SetActive(false);

        // Re-enable answer buttons for the next question
        foreach (var option in options)
        {
            option.GetComponent<Button>().interactable = true;
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

        // We don't need to hide feedbackText anymore since we're using QuestionTxt
        // Remove or comment out the following line:
        // feedbackText.gameObject.SetActive(false);
    }



    void SetAnswer()
    {
        // First set the answers as usual (without changing button positions)
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestionIndex].Answers[i];

            if (QnA[currentQuestionIndex].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true; // Set the correct answer
            }
        }
        
        // Now shuffle the positions of the buttons
        RandomizeButtonPositions();
    }

        private void RandomizeButtonPositions()
    {
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
        
        // Log the shuffled order for debugging
        string debugOrder = "Button position order: ";
        foreach (int idx in shuffledIndices)
        {
            debugOrder += idx + ", ";
        }
        Debug.Log(debugOrder);
        
        // Apply shuffled positions to buttons
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<RectTransform>().anchoredPosition = originalPositions[shuffledIndices[i]];
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
        if (answeredQuestionsCount >= maximumQuestions)
        {
            // End the quiz if we've answered enough questions
            gameOver();
            return;
        }

        // NEW CODE: Check if we've reached halfway point
        if (!miniGamePlayed && answeredQuestionsCount >= (maximumQuestions / 2))
        {
            // We've reached half of the questions, trigger mini-game
            StartMiniGame();
            return;
        }

        if (QnA.Count > 0)
        {
            currentQuestionIndex = Random.Range(0, QnA.Count);
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

        // Continue with the next question
        generateQuestion();
    }

    public void AddMiniGameScore(int points)
    {
        // Add mini-game points to the quiz score
        scoreCount += points;
        
        // Update score display
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + scoreCount;
            StartCoroutine(ShowScoreAddedEffect(points));
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
    
    
}

