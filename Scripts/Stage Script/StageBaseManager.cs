using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageBaseManager : MonoBehaviour
{
    [Header("Wade Instructor")]
    public GameObject wadePanel;
    public TextMeshProUGUI wadeText;

    [Header("Wade Expressions")]
    public InstructorExpressions wadeExpressions;

    private int wadeDialogCounter = 0;

    public virtual int ShowWade(string text)
    {
        if (wadePanel != null)
            wadePanel.SetActive(true);

        if (wadeText != null)
            wadeText.text = text;

        wadeDialogCounter++;
        return wadeDialogCounter; // return current id
    }

    public virtual void HideWade(int dialogId)
    {
        if (dialogId == wadeDialogCounter)
            wadePanel.SetActive(false);
    }

    public virtual void HideWade() // fallback
    {
        wadePanel.SetActive(false);
    }

    // Add expression parameter with default value
    public int ShowWade(string message, Sprite expression = null)
    {
        if (wadePanel != null)
            wadePanel.SetActive(true);

        if (wadeText != null)
            wadeText.text = message;

        // Add this for expressions
        if (wadeExpressions != null && expression != null)
            wadeExpressions.SetExpression(expression);

        return 0; // Or whatever you return
    }

    // Add these shorthand methods for easy expression switching
    public void ShowWadeHappy(string message)
    {
        int dialogId = ShowWade(message);
        if (wadeExpressions != null)
            wadeExpressions.SetHeartEyesExpression();
    }

    public void ShowWadeMad(string message)
    {
        int dialogId = ShowWade(message);
        if (wadeExpressions != null)
            wadeExpressions.SetMadExpression();
    }

    public void ShowWadeTired(string message)
    {
        int dialogId = ShowWade(message);
        if (wadeExpressions != null)
            wadeExpressions.SetTiredExpression();
    }
}
