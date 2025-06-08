using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ParkingZone : MonoBehaviour
{
    public StageBaseManager stageBaseManager;
    public GameObject highlightVisual; // Assign in Inspector
    public float minParkTime = 2.0f;
    public float maxParkSpeed = 0.5f;
    private float parkedTimer = 0f;
    private bool playerInZone = false;
    private Rigidbody playerRb;

    private bool highlightActive = false;

    public GameObject finalGameMarker; // Assign in Inspector: the marker GameObject
    private bool markerGone = false;

    // For pulsing effect
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;
    private Vector3 baseScale;

    // For emission (optional)
    public Renderer highlightRenderer; // assign in Inspector if using emission
    public Color glowColor = Color.yellow;
    public float emissionPulseSpeed = 2f;
    public float emissionMin = 0.5f;
    public float emissionMax = 2f;

    // WIN UI references
    public GameObject winPanel;       // NEW: Assign your Win UI panel
    public TMP_Text scoreText;        // NEW: Assign in Inspector
    public Image[] starImages;        // NEW: 3 stars, left to right
    public Sprite fullStarSprite;     // NEW: full/star-on sprite
    public Sprite emptyStarSprite;    // NEW: empty/star-off sprite

    void Start()
    {
        if (highlightVisual != null)
        {
            highlightVisual.SetActive(false);
            baseScale = highlightVisual.transform.localScale;
        }

        // Hide win panel at start
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void ActivateParkingHighlight()
    {
        if (highlightVisual != null)
        {
            highlightVisual.SetActive(true);
            highlightActive = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerRb = other.attachedRigidbody;
            parkedTimer = 0f;

            // Show highlight if not already active
            if (highlightVisual != null && !highlightActive)
            {
                highlightVisual.SetActive(true);
                highlightActive = true;
            }

            ShowDialogAutoHide("Park your car inside the highlighted space and stop.", 1f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            parkedTimer = 0f;
            playerRb = null;
            ShowDialogAutoHide("Try again! Park fully in the space and come to a stop.", 1f);
        }
    }

    void Update()
    {

        finalGameMarker = markerGone ? null : finalGameMarker;
        // Show highlight when marker is destroyed/inactive, even if player hasn't entered yet
        if (!markerGone && (finalGameMarker == null || !finalGameMarker.activeSelf))
        {
            markerGone = true;
            if (highlightVisual != null)
            {
                highlightVisual.SetActive(true);
                highlightActive = true;
            }
            ShowDialogAutoHide("Proceed to the highlighted parking spot!", 1f);
        }

        // Pulsing effect (scale)
        if (highlightVisual != null && highlightVisual.activeSelf)
        {
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            highlightVisual.transform.localScale = baseScale * scale;

            // Optional: Emission pulse for glowing
            if (highlightRenderer != null)
            {
                float emission = Mathf.Lerp(emissionMin, emissionMax, (Mathf.Sin(Time.time * emissionPulseSpeed) + 1f) / 2f);
                highlightRenderer.material.SetColor("_EmissionColor", glowColor * emission);
            }
        }

        // Parking logic
        if (playerInZone && playerRb != null)
        {
            if (playerRb.velocity.magnitude < maxParkSpeed)
            {
                parkedTimer += Time.deltaTime;
                if (parkedTimer >= minParkTime)
                {
                    // Success!
                    StageScoreManager.Instance.AddPoints(100);

                    // Show win panel!
                    ShowWinPanel();

                    if (highlightVisual != null)
                        highlightVisual.SetActive(false);
                    enabled = false;
                }
            }
            else
            {
                parkedTimer = 0f;
            }
        }
    }

    // Show dialog and auto-hide after duration
    void ShowDialogAutoHide(string msg, float delay)
    {
        if (stageBaseManager != null)
        {
            stageBaseManager.ShowWade(msg);
            StopAllCoroutines(); // Prevent multiple overlapping hides
            StartCoroutine(HideDialogAfter(delay));
        }
    }

    IEnumerator HideDialogAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        stageBaseManager.HideWade();
    }

    // WIN UI logic
    void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (scoreText != null)
            scoreText.text = "Total Score: " + StageScoreManager.Instance.GetPoints();

        int score = StageScoreManager.Instance.GetPoints();
        int stars = CalculateStars(score);

        // Enable only the number of stars earned
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
                starImages[i].gameObject.SetActive(i < stars);
        }
    }


    // Adjust these thresholds to your own scoring system
    int CalculateStars(int score)
    {
        if (score >= 250) return 3;
        if (score >= 150) return 2;
        if (score >= 50) return 1;
        return 0;
    }
}
