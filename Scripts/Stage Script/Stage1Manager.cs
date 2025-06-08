using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Stage1TutorialManager : StageBaseManager
{
    [Header("Wade Dialogue UI (Image and Text only)")]
    public GameObject wadeImage;      // Assign Wade's image GameObject
    public TMP_Text wadeText;         // Assign Wade's TMP_Text

    [Header("UI & References")]
    public CarControls carControls; // Assign in Inspector
    public Button acceleratorButton;
    public Button brakeButton;
    public Image steeringWheelImage;
    public GameObject[] allOtherUIButtons;

    [Header("Gear Shift UI (Slider Setup)")]
    public Slider gearShiftSlider;
    public Image gearShiftImage;

    [Header("Tutorial Settings")]
    public float moveDistanceForTutorial = 2.0f;
    public float turnDetectDistance = 3.0f;

    private Vector3 carStartPos;
    private bool brakingComplete = false;
    private bool turningComplete = false;

    private int lastGearIndex = -1;

    public TMP_Text scoreText; // Assign in Inspector

    private readonly string[] gearHints = {
        "<b>P</b> = Park (vehicle won't move)",
        "<b>R</b> = Reverse (move backward)",
        "<b>N</b> = Neutral (free wheel, no drive)",
        "<b>D</b> = Drive (move forward)"
    };

    // (Optional) Lock to disable dialog after tutorial, if needed
    private bool dialogLocked = false;

    void Start()
    {
        carControls.carPoweredOn = true;
        HideAllControls();
        // Hide Wade image and text at the start
        if (wadeImage != null) wadeImage.SetActive(false);
        if (wadeText != null) wadeText.gameObject.SetActive(false);
        StartCoroutine(RunTutorialSequence());
    }

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + StageScoreManager.Instance.GetPoints();
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

    public override void ShowWade(string text)
    {
        if (dialogLocked) return;
        if (wadeImage != null) wadeImage.SetActive(true);
        if (wadeText != null)
        {
            wadeText.gameObject.SetActive(true);
            wadeText.text = text;
        }
    }

    public override void HideWade()
    {
        if (wadeImage != null) wadeImage.SetActive(false);
        if (wadeText != null) wadeText.gameObject.SetActive(false);
    }

    // Optionally lock dialog after tutorial, if desired
    public void EndTutorial()
    {
        dialogLocked = true;
        HideWade();
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

    // ===== GEAR TUTORIAL: Event Listener & Hint =====
    private void OnGearSliderValueChanged(float value)
    {
        int gearIndex = Mathf.RoundToInt(value);
        if (gearIndex != lastGearIndex)
        {
            ShowGearHint(gearIndex);
            lastGearIndex = gearIndex;
        }
    }

    private void ShowGearHint(int gearIndex)
    {
        gearIndex = Mathf.Clamp(gearIndex, 0, gearHints.Length - 1);
        ShowWade(gearHints[gearIndex]);
    }

    IEnumerator RunTutorialSequence()
    {
        // ========== STEP 0: INTERACTIVE GEAR SHIFT TUTORIAL ==========
        HideAllControls();
        if (gearShiftImage != null) gearShiftImage.gameObject.SetActive(true);
        if (gearShiftSlider != null) gearShiftSlider.gameObject.SetActive(true);

        HighlightImage(gearShiftImage);

        lastGearIndex = Mathf.RoundToInt(gearShiftSlider.value);
        ShowGearHint(lastGearIndex);

        gearShiftSlider.onValueChanged.AddListener(OnGearSliderValueChanged);

        yield return new WaitUntil(() => gearShiftSlider != null && Mathf.Approximately(gearShiftSlider.value, 3));

        gearShiftSlider.onValueChanged.RemoveListener(OnGearSliderValueChanged);

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
}
