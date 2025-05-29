using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswer> QnA; // List of questions and answers
    public GameObject[] options; // UI elements for answer options
    public int currentQuestionIndex; // Track the current question index

    public TextMeshProUGUI QuestionTxt; // UI Text element to display the question; // UI Text element to display the question

    private void Start()
    {
        // Initialize the quiz by displaying the first question
        generateQuestion();
    }

    public void correct()
    {
        QnA.RemoveAt(currentQuestionIndex); // Remove the current question from the list
        generateQuestion(); // Generate a new question
    }
        

    void SetAnswer()
    {
        // Loop through all options and set their text
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false; 
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestionIndex].Answers[i];
            
            if(QnA[currentQuestionIndex].CorrectAnswer == i+1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true; // Set the correct answer
            }
        }
    }

    void generateQuestion()
    {
        currentQuestionIndex = Random.Range(0, QnA.Count);

        QuestionTxt.text = QnA[currentQuestionIndex].Question;

        SetAnswer(); 

    }

}

