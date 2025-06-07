using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage2Manager : StageBaseManager
{
    [Header("UI & References")]
    public CarControls carControls; // Assign in Inspector
    public Button acceleratorButton;
    public Button brakeButton;
    public Image steeringWheelImage;
    public GameObject[] allOtherUIButtons;

    [Header("Gear Shift UI (Slider Setup)")]
    public Slider gearShiftSlider;
    public Image gearShiftImage;
    public TMP_Text scoreText; // Assign in Inspector

    void Start()
    {
        carControls.carPoweredOn = true;
        ShowAllControls();
        ShowWade("It's the next day. Ready for your next lessons?");
        StartCoroutine(HideWadeAfterDelay(2f));
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

    IEnumerator HideWadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideWade();
    }
}
