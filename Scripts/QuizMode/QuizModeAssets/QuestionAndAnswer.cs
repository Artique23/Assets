using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class QuestionAndAnswer
{
    public string Question;
    public string[] Answers;
    public int CorrectAnswer; // Index of the correct answer in the Answers array
    
    [Header("Optional Image")]
    public bool hasImage = false;
    public Sprite questionImage; // The image to display with this question
    
    [Header("Feedback")]
    [TextArea(2, 4)]
    public string correctExplanation = ""; // Explanation for correct answer
    [TextArea(2, 4)]
    public string incorrectExplanation = ""; // Explanation for incorrect answer
}