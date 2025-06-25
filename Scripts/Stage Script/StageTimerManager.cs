using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageTimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 300f;              // Starting time (e.g. 5 minutes)
    public float timePerObjective = 10f;        // Time added when objective is reached
    private float currentTime;
    private bool isTimerActive = true;

    [Header("UI")]
    public TMP_Text timerText;                  // Reference to the timer UI Text
    public GameObject losePanel;                // UI panel to show when time runs out

    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    void Update()
    {
        if (!isTimerActive) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerActive = false;
            TriggerLose();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString() + "s";

            // Color indicator based on urgency
            float percentage = currentTime / startTime;

            if (percentage <= 0.2f)
            {
                timerText.color = Color.red; // Low time = red
            }
            else if (percentage <= 0.5f)
            {
                timerText.color = new Color(1f, 0.6f, 0f); // Medium time = orange
            }
            else
            {
                timerText.color = Color.green; // Plenty of time = green
            }
        }
    }

    public void AddTime()
    {
        currentTime += timePerObjective;
        UpdateTimerUI();
    }

    void TriggerLose()
    {
        Debug.Log("Time is up! Showing lose panel.");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // Optional: freeze car control or trigger fail expression
        CarControls car = FindObjectOfType<CarControls>();
        if (car != null)
            car.carPoweredOn = false;

        StageBaseManager baseManager = FindObjectOfType<StageBaseManager>();
        if (baseManager != null)
            baseManager.ShowWade("Time's up! You didn't make it in time.");
    }
}
