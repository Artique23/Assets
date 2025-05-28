using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Stage1TutorialManager : MonoBehaviour
{
    [Header("UI & References")]
    public CarControls carControls; // Assign in Inspector
    public Button acceleratorButton;
    public Button brakeButton;
    public Image steeringWheelImage;
    public GameObject[] allOtherUIButtons;

    [Header("Gear Shift UI (Slider Setup)")]
    public Slider gearShiftSlider;             // Assign your gear shift slider here
    public Image gearShiftImage;               // Assign slider's background or handle image here (for highlighting)

    [Header("Wade Dialogue UI")]
    public GameObject wadePopupPanel;
    public TMP_Text wadeText;

    [Header("Tutorial Settings")]
    public float moveDistanceForTutorial = 2.0f;
    public float turnDetectDistance = 3.0f;

    private Vector3 carStartPos;
    private bool brakingComplete = false;
    private bool turningComplete = false;

    void Start()
    {
        // Start the car powered on for the tutorial
        carControls.carPoweredOn = true;
        HideAllControls();

        StartCoroutine(RunTutorialSequence());
    }

    void HideAllControls()
    {
        if (acceleratorButton != null) acceleratorButton.gameObject.SetActive(false);
        if (brakeButton != null) brakeButton.gameObject.SetActive(false);
        if (steeringWheelImage != null) steeringWheelImage.gameObject.SetActive(false);
        if (gearShiftImage != null) gearShiftImage.gameObject.SetActive(false);
        if (gearShiftSlider != null) gearShiftSlider.gameObject.SetActive(false);
        foreach (var btn in allOtherUIButtons)
            btn.SetActive(false);
    }

    public void ShowWade(string text)
    {
        wadePopupPanel.SetActive(true);
        wadeText.text = text;
    }

    public void HideWade()
    {
        wadePopupPanel.SetActive(false);
    }

    void HighlightButton(Selectable btn)
    {
        var outline = btn.GetComponent<Outline>();
        if (!outline) outline = btn.gameObject.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
        outline.effectDistance = new Vector2(6, 6);
    }
    void UnhighlightButton(Selectable btn)
    {
        var outline = btn.GetComponent<Outline>();
        if (outline) Destroy(outline);
    }
    void HighlightImage(Image img)
    {
        var outline = img.GetComponent<Outline>();
        if (!outline) outline = img.gameObject.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
        outline.effectDistance = new Vector2(6, 6);
    }
    void UnhighlightImage(Image img)
    {
        var outline = img.GetComponent<Outline>();
        if (outline) Destroy(outline);
    }
    void MakeImageClickable(Image img, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = img.GetComponent<EventTrigger>();
        if (!trigger) trigger = img.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((e) => action.Invoke());
        trigger.triggers.Add(entry);
    }
    void RemoveAllImageClickables(Image img)
    {
        var trigger = img.GetComponent<EventTrigger>();
        if (trigger) trigger.triggers.Clear();
    }

    IEnumerator RunTutorialSequence()
    {
        // ========== STEP 0: GEAR SHIFT TUTORIAL ==========
        HideAllControls();
        if (gearShiftImage != null) gearShiftImage.gameObject.SetActive(true);
        if (gearShiftSlider != null) gearShiftSlider.gameObject.SetActive(true);

        HighlightImage(gearShiftImage);
        ShowWade(
            "<b>Gear Shift Tutorial</b><br>" +
            "This is your gear shift:<br>" +
            "<b>P</b> = Park (top)<br>" +
            "<b>R</b> = Reverse<br>" +
            "<b>N</b> = Neutral<br>" +
            "<b>D</b> = Drive (bottom)<br>" +
            "<br>Slide to <b>D</b> (bottom) to start driving!"
        );

        // Wait for player to move slider to Drive (value == 3)
        yield return new WaitUntil(() => gearShiftSlider != null && Mathf.Approximately(gearShiftSlider.value, 3));
        UnhighlightImage(gearShiftImage);
        HideWade();

        // ========== STEP 1: ACCELERATION ==========
        carStartPos = carControls.transform.position;
        HideAllControls();
        if (acceleratorButton != null) acceleratorButton.gameObject.SetActive(true);
        ShowWade("Let's start! <b>Press the accelerator</b> to move forward.");
        HighlightButton(acceleratorButton);

        if (brakeButton != null) brakeButton.gameObject.SetActive(false);
        if (steeringWheelImage != null) steeringWheelImage.gameObject.SetActive(false);

        yield return new WaitUntil(() => Vector3.Distance(carControls.transform.position, carStartPos) > moveDistanceForTutorial);
        UnhighlightButton(acceleratorButton);

        // ========== STEP 2: BRAKE ==========
        ShowWade("Good job! Now <b>press the brake</b> to stop.");
        if (brakeButton != null) brakeButton.gameObject.SetActive(true);
        HighlightButton(brakeButton);

        brakingComplete = false;
        brakeButton.onClick.RemoveAllListeners();
        brakeButton.onClick.AddListener(() =>
        {
            StartCoroutine(CheckStopped());
        });

        yield return new WaitUntil(() => brakingComplete);
        UnhighlightButton(brakeButton);

        // ========== STEP 3: FREELY ACCELERATE & BRAKE ==========
        ShowWade("Awesome! Now you can <b>freely accelerate and brake.</b>");
        if (acceleratorButton != null) acceleratorButton.gameObject.SetActive(true);
        if (brakeButton != null) brakeButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        HideWade();

        // ========== STEP 4: TURNING ==========
        ShowWade("Approaching a turn! <b>Tap the steering wheel</b> when ready.");
        if (steeringWheelImage != null)
        {
            steeringWheelImage.gameObject.SetActive(true);
            HighlightImage(steeringWheelImage);

            turningComplete = false;
            MakeImageClickable(steeringWheelImage, () => { turningComplete = true; });
        }

        yield return new WaitUntil(() => IsNearTurnPoint());
        yield return new WaitUntil(() => turningComplete);
        if (steeringWheelImage != null)
        {
            UnhighlightImage(steeringWheelImage);
            RemoveAllImageClickables(steeringWheelImage);
        }

        ShowWade("Great! Now you can control the vehicle fully.");
        yield return new WaitForSeconds(1.5f);
        HideWade();

        // ========== STEP 5: END TUTORIAL ==========
        if (acceleratorButton != null) acceleratorButton.gameObject.SetActive(true);
        if (brakeButton != null) brakeButton.gameObject.SetActive(true);
        if (steeringWheelImage != null) steeringWheelImage.gameObject.SetActive(true);
        if (gearShiftImage != null) gearShiftImage.gameObject.SetActive(true);
        if (gearShiftSlider != null) gearShiftSlider.gameObject.SetActive(true);
        foreach (var btn in allOtherUIButtons)
            btn.SetActive(true);
        // (Tutorial is complete; continue with your stage/advanced scenarios here!)
    }

    IEnumerator CheckStopped()
    {
        Rigidbody rb = carControls.GetComponent<Rigidbody>();
        if (rb == null) yield break;
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.3f);
        brakingComplete = true;
    }

    bool IsNearTurnPoint()
    {
        return Vector3.Distance(carControls.transform.position, carStartPos) > 15.0f;
    }
}
