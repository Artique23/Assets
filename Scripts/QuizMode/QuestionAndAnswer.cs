using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionAndAnswer
{
    public string Question;
    public string[] Answers;
    public int CorrectAnswer; // Index of the correct answer in the Answers array
}
