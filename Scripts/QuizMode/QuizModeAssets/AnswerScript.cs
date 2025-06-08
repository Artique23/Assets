using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    public bool wasSelected = false; // Add this field to AnswerScript.cs
    
    public QuizManager quizManager; // Reference to the QuizManager to access the current question

    public void Answer()
    {
        if (!quizManager.isShowingFeedback)
        {
            if (isCorrect)
            {
                quizManager.correct();
            }
            else
            {
                quizManager.wrong();
            }
        }
    }
}
