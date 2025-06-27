using System.Collections;
using UnityEngine;
using TMPro;

public class SlipperyRoadZone : MonoBehaviour
{
    [Header("Slippery Zone Settings")]
    public float maxAllowedSpeedKmh = 18f; // UI speed threshold
    public float gracePeriod = 3f;
    public int rewardPoints = 100;
    public int penaltyPoints = -50;

    [Header("UI References")]
    public TMP_Text speedometerText; // Drag your UI text here (e.g., "12 km/h")

    [Header("Dialogue")]
    public string enterMessage = "Maintain a safe speed through the slippery road!";

    private float overSpeedTimer = 0f;
    private bool playerInside = false;
    private bool completed = false;
    private CarControls carControls;

    void OnTriggerEnter(Collider other)
    {
        if (completed) return;

        if (other.TryGetComponent(out CarControls controls))
        {
            carControls = controls;
            playerInside = true;
            overSpeedTimer = 0f;

            if (!string.IsNullOrEmpty(enterMessage))
            {
                carControls.tutorialManager?.ShowWade(enterMessage);
            }

            Debug.Log("Player entered slippery zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (completed || carControls == null) return;

        if (other.TryGetComponent(out CarControls controls) && controls == carControls)
        {
            playerInside = false;

            if (overSpeedTimer < gracePeriod)
            {
                StageScoreManager.Instance.AddPoints(rewardPoints);
                carControls.tutorialManager?.ShowWade("Great control! +100 points.");
                Debug.Log("Player rewarded.");
            }
            else
            {
                Debug.Log("Player already punished.");
            }

            completed = true;
        }
    }

    void Update()
    {
        if (!playerInside || completed || speedometerText == null) return;

        float displayedSpeed = ParseSpeedFromText();

        if (displayedSpeed > maxAllowedSpeedKmh)
        {
            overSpeedTimer += Time.deltaTime;

            if (overSpeedTimer >= gracePeriod)
            {
                StageScoreManager.Instance.AddPoints(penaltyPoints);
                carControls.tutorialManager?.ShowWade("Too fast! -50 points.");
                Debug.Log("Player punished for speeding.");
                completed = true;
            }
        }
        else
        {
            overSpeedTimer = 0f; // Reset if speed is back under control
        }
    }

    float ParseSpeedFromText()
    {
        string rawText = speedometerText.text;
        string digitsOnly = "";

        foreach (char c in rawText)
        {
            if (char.IsDigit(c))
                digitsOnly += c;
            else if (c == '.' && !digitsOnly.Contains("."))
                digitsOnly += c;
        }

        if (float.TryParse(digitsOnly, out float speed))
            return speed;

        return 0f;
    }
}
