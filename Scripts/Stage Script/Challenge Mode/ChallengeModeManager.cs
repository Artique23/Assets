using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengeModeManager : MonoBehaviour
{
    [Header("UI & References")]
    public CarControls carControls;
    public Button acceleratorButton;
    public Button brakeButton;
    public Image steeringWheelImage;
    public GameObject[] allOtherUIButtons;

    [Header("Gear Shift UI")]
    public Slider gearShiftSlider;
    public Image gearShiftImage;
    public TMP_Text scoreText;

    [Header("Timer & Lives")]
    public StageTimerManager timerManager;
    public float timeRewardPerObjective = 10f;
    public int maxLives = 3;
    private int currentLives;
    public Image[] lifeIcons; // NEW: array of images for lives

    [Header("Objectives & Parking")]
    public GameObject[] objectiveMarkers;
    public ParkingZone parkingZone;
    private int currentObjectiveIndex = 0;

    void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();

        carControls.carPoweredOn = true;
        ShowAllControls();
        ActivateCurrentObjective();
    }

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + StageScoreManager.Instance.GetPoints();
    }

    void ShowAllControls()
    {
        if (acceleratorButton != null) acceleratorButton.gameObject.SetActive(true);
        if (brakeButton != null) brakeButton.gameObject.SetActive(true);
        if (steeringWheelImage != null) steeringWheelImage.gameObject.SetActive(true);
        if (gearShiftImage != null) gearShiftImage.gameObject.SetActive(true);
        if (gearShiftSlider != null) gearShiftSlider.gameObject.SetActive(true);

        foreach (var btn in allOtherUIButtons)
            btn.SetActive(true);
    }

    void ActivateCurrentObjective()
    {
        if (currentObjectiveIndex < objectiveMarkers.Length)
        {
            objectiveMarkers[currentObjectiveIndex].SetActive(true);
        }
        else
        {
            parkingZone.canCheckParking = true;
        }
    }

    public void OnObjectiveReached()
    {
        if (currentObjectiveIndex < objectiveMarkers.Length)
        {
            objectiveMarkers[currentObjectiveIndex].SetActive(false);
        }

        if (timerManager != null)
            timerManager.AddTime();

        currentObjectiveIndex++;
        ActivateCurrentObjective();
    }

    public void ApplyPunishment()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            // Optional: Reset car position or give player another try
        }
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = i < currentLives;
        }
    }

    void GameOver()
    {
        carControls.carPoweredOn = false;
        Debug.Log("Game Over - All lives lost");
        // Optional: Show Game Over UI, return to menu, etc.
    }
}
