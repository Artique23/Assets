using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage4Manager : StageBaseManager
{
    [Header("UI & References")]
    public CarControls carControls; // Assign in Inspector
    public Button acceleratorButton;
    public Button brakeButton;
    public Image steeringWheelImage;
    public GameObject[] allOtherUIButtons;

    [Header("Gear Shift UI")]
    public Slider gearShiftSlider;
    public Image gearShiftImage;
    public TMP_Text scoreText; // Assign in Inspector

    [Header("Timer Mode")]
    public StageTimerManager timerManager; // Assign your timer script
    public float timeRewardPerObjective = 10f;

    [Header("Objectives & Parking")]
    public GameObject[] objectiveMarkers; // Assign all in order
    public ParkingZone parkingZone;       // Assign your parking zone
    private int currentObjectiveIndex = 0;

    void Start()
    {
        carControls.carPoweredOn = true;
        ShowAllControls();

        ShowWade("Beat the clock! Reach all checkpoints to earn more time!");
        StartCoroutine(HideWadeAfterDelay(3f));

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
            ShowWade("Nice job! Now head to the parking zone to finish.");
            StartCoroutine(HideWadeAfterDelay(3f));
        }
    }

    public void OnObjectiveReached()
    {
        if (currentObjectiveIndex < objectiveMarkers.Length)
        {
            objectiveMarkers[currentObjectiveIndex].SetActive(false);
        }

        // Reward time if timer manager is available
        if (timerManager != null)
            timerManager.AddTime();

        currentObjectiveIndex++;
        ActivateCurrentObjective();
    }

    IEnumerator HideWadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideWade();
    }
}
