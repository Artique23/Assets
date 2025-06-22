using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage3Manager : StageBaseManager
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

    [Header("Weather Settings")]
    public ParticleSystem rainEffect; // Assign in Inspector
    public AudioSource rainSound;     // Optional

    void Start()
    {
        carControls.carPoweredOn = true;
        ShowAllControls();
        ActivateRain();

        // Inherited from StageBaseManager
        ShowWade("It's raining today. Driving will be a bit more challenging—be careful out there!");
        StartCoroutine(HideWadeAfterDelay(3f));
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

    void ActivateRain()
    {
        if (rainEffect != null) rainEffect.Play();
        if (rainSound != null) rainSound.Play();
    }

    IEnumerator HideWadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Inherited from StageBaseManager
        HideWade();
    }
}
