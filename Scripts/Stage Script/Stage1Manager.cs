using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage1TutorialManager : MonoBehaviour
{
    [Header("UI & References")]
    public CarControls carControls; // Assign in Inspector
    public Button acceleratorButton;
    public Button brakeButton;
    public Button steeringButton; // Assign your UI left/right or steering wheel button if you have one
    public GameObject[] allOtherUIButtons; // Put every other gameplay button/UI here

    [Header("Wade Dialogue UI")]
    public GameObject wadePopupPanel; // Assign Wade's popup panel here
    public TMP_Text wadeText; // Assign TextMeshProUGUI or Unity Text

    [Header("Tutorial Settings")]
    public float moveDistanceForTutorial = 2.0f; // Meters to consider 'moved forward'
    public float turnDetectDistance = 3.0f; // For turn point detection

    private Vector3 carStartPos;
    private bool brakingComplete = false;
    private bool turningComplete = false;

    void Start()
    {
        // Hide all controls at start except car power if needed
        HideAllControls();

        carControls.carPoweredOn = true; // Optionally force car on for tutorial start
        carControls.acceleratorButton.gameObject.SetActive(false);
        carControls.brakeButton.gameObject.SetActive(false);
        if (steeringButton != null) steeringButton.gameObject.SetActive(false);

        carStartPos = carControls.transform.position;
        StartCoroutine(RunTutorialSequence());
    }

    void HideAllControls()
    {
        carControls.acceleratorButton.gameObject.SetActive(false);
        carControls.brakeButton.gameObject.SetActive(false);
        if (steeringButton != null) steeringButton.gameObject.SetActive(false);
        foreach (var btn in allOtherUIButtons)
            btn.SetActive(false);
    }

    void ShowWade(string text)
    {
        wadePopupPanel.SetActive(true);
        wadeText.text = text;
    }

    void HideWade()
    {
        wadePopupPanel.SetActive(false);
    }

    // Highlight by e.g. making the button flash or adding Outline
    void HighlightButton(Button btn)
    {
        var outline = btn.GetComponent<Outline>();
        if (!outline) outline = btn.gameObject.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
        outline.effectDistance = new Vector2(6, 6);
    }
    void UnhighlightButton(Button btn)
    {
        var outline = btn.GetComponent<Outline>();
        if (outline) Destroy(outline);
    }

    IEnumerator RunTutorialSequence()
    {
        // STEP 1: Acceleration
        ShowWade("Let's start! <b>Press the accelerator</b> to move forward.");
        carControls.acceleratorButton.gameObject.SetActive(true);
        HighlightButton(carControls.acceleratorButton);

        // Only allow accelerator input (lock brake/steer/other controls)
        carControls.brakeButton.gameObject.SetActive(false);
        if (steeringButton != null) steeringButton.gameObject.SetActive(false);

        // Wait for car to move forward a certain distance
        yield return new WaitUntil(() => Vector3.Distance(carControls.transform.position, carStartPos) > moveDistanceForTutorial);
        UnhighlightButton(carControls.acceleratorButton);

        // STEP 2: Brake
        ShowWade("Good job! Now <b>press the brake</b> to stop.");
        carControls.brakeButton.gameObject.SetActive(true);
        HighlightButton(carControls.brakeButton);

        brakingComplete = false;
        carControls.brakeButton.onClick.RemoveAllListeners();
        carControls.brakeButton.onClick.AddListener(() =>
        {
            // Listen for near-zero speed on brake press
            StartCoroutine(CheckStopped());
        });

        yield return new WaitUntil(() => brakingComplete);
        UnhighlightButton(carControls.brakeButton);

        // STEP 3: Freely accelerate & brake
        ShowWade("Awesome! Now you can <b>freely accelerate and brake.</b>");
        carControls.acceleratorButton.gameObject.SetActive(true);
        carControls.brakeButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        HideWade();

        // STEP 4: Wait for turn point trigger (use a trigger/collider in your level)
        ShowWade("Approaching a turn! <b>Tap the steering button</b> when ready.");
        if (steeringButton != null)
        {
            steeringButton.gameObject.SetActive(true);
            HighlightButton(steeringButton);
        }

        // Wait for player to be near turn trigger (can be replaced with a real trigger/collider event)
        yield return new WaitUntil(() => IsNearTurnPoint());

        // Listen for steering button tap
        turningComplete = false;
        if (steeringButton != null)
        {
            steeringButton.onClick.RemoveAllListeners();
            steeringButton.onClick.AddListener(() =>
            {
                turningComplete = true;
            });
        }

        yield return new WaitUntil(() => turningComplete);
        if (steeringButton != null) UnhighlightButton(steeringButton);

        ShowWade("Great! Now you can control the vehicle fully.");
        yield return new WaitForSeconds(1.5f);
        HideWade();

        // STEP 5: Tutorial ends, enable all controls
        carControls.acceleratorButton.gameObject.SetActive(true);
        carControls.brakeButton.gameObject.SetActive(true);
        if (steeringButton != null) steeringButton.gameObject.SetActive(true);
        foreach (var btn in allOtherUIButtons)
            btn.SetActive(true);

        // (Tutorial is complete; continue with your stage/advanced scenarios here!)
    }

    IEnumerator CheckStopped()
    {
        // Wait until speed is near zero
        Rigidbody rb = carControls.GetComponent<Rigidbody>();
        if (rb == null) yield break;
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.3f);
        brakingComplete = true;
    }

    // Dummy logic for turn point—replace with your own trigger/collider logic!
    bool IsNearTurnPoint()
    {
        // Example: if the car moves more than 15 meters from the start, show turn
        return Vector3.Distance(carControls.transform.position, carStartPos) > 15.0f;
    }
}
