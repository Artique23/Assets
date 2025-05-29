using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    
    public QuizManager quizManager; // Reference to the QuizManager to access the current question

    public void Answer()
    {
        if (isCorrect)
        {
            // Handle correct answer logic here
            Debug.Log("Correct Answer!");
            // You can add more logic like updating score, moving to next question, etc.
            quizManager.correct();
        }
        else
        {
            // Handle incorrect answer logic here
            Debug.Log("Incorrect Answer!");
            // You can add more logic like showing feedback, retrying, etc.
        }
    }
}
