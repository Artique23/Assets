using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengeModeManager : MonoBehaviour
{
    [Header("Challenge Settings")]
    public int startingLives = 3;
    public float challengeTime = 120f;

    [Header("Objective Markers and Parking")]
    public GameObject[] objectiveMarkers; // Assign 3 in Inspector
    public GameObject parkingZone;        // Assign in Inspector (set inactive initially)

    [Header("UI (Stars, Score, Timer)")]
    public Image[] lifeImages;        // Assign 3 star images, left to right
    public Sprite fullLifeSprite;     // Green/active star
    public Sprite emptyLifeSprite;    // Gray/empty star
    public TMP_Text timerText;        // (Optional) Timer display
    public TMP_Text scoreText;        // Score display

    private int playerLives;
    private float timer;
    private int collected = 0;
    private bool gameActive = false;
    private bool parkingActivated = false;

    void Start()
    {
        playerLives = startingLives;
        timer = challengeTime;
        collected = 0;
        gameActive = true;
        parkingActivated = false;
        

        foreach (var obj in objectiveMarkers)
            obj.SetActive(true);

        if (parkingZone != null)
            parkingZone.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;
        UpdateUI();

        if (timer <= 0f)
        {
            LoseGame();
        }
    }

    // Called by objective markers when collected
    public void CollectObjective(GameObject marker)
    {
        marker.SetActive(false);
        collected++;
        if (collected >= objectiveMarkers.Length)
        {
            ActivateParking();
        }
    }

    void ActivateParking()
    {
        if (parkingActivated) return;
        parkingActivated = true;
        if (parkingZone != null)
            parkingZone.SetActive(true);
    }

    // Called by scenario scripts to penalize
    public void LoseLife()
    {
        if (!gameActive) return;
        playerLives--;
        UpdateUI();
        if (playerLives <= 0)
        {
            LoseGame();
        }
    }

    // Called by scenario scripts to reward
    public void AddChallengePoints(int amount)
    {
        StageScoreManager.Instance.AddPoints(amount);
        UpdateUI();
    }

    void LoseGame()
    {
        gameActive = false;
        // Optionally: Show a "Game Over" screen or disable controls
    }

    void UpdateUI()
    {
        // Lives UI (stars)
        if (lifeImages != null && lifeImages.Length > 0)
        {
            for (int i = 0; i < lifeImages.Length; i++)
            {
                if (lifeImages[i] != null)
                {
                    lifeImages[i].sprite = (i < playerLives) ? fullLifeSprite : emptyLifeSprite;
                }
            }
        }

        // Score UI
        if (scoreText != null)
            scoreText.text = "Score: " + StageScoreManager.Instance.GetPoints();

        // Timer UI
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(Mathf.Max(timer, 0));
    }
}
