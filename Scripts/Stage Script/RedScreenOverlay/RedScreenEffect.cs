using UnityEngine;
using UnityEngine.UI;

public class RedScreenEffect : MonoBehaviour
{
    public StageScoreManager scoreManager; // Assign in Inspector
    public GameObject redOverlay;          // Assign in Inspector
    public float breathSpeed = 1.5f;       // Breathing speed
    public float minAlpha = 0.3f;          // Minimum alpha
    public float maxAlpha = 0.7f;          // Maximum alpha
    private bool forceShow = false;        // Force show overlay
    private CanvasGroup overlayGroup;

    void Start()
    {
        if (redOverlay != null)
            overlayGroup = redOverlay.GetComponent<CanvasGroup>();
    }

        public void ForceShowOverlay(bool show)
    {
        forceShow = show;
        if (redOverlay != null)
            redOverlay.SetActive(show);
        if (overlayGroup != null && show)
            overlayGroup.alpha = maxAlpha;
    }


    void Update()
    {
        if (scoreManager != null && redOverlay != null && overlayGroup != null)
        {
            bool shouldShow =
                forceShow ||
                (scoreManager.GetPoints() >= -500 && scoreManager.GetPoints() <= -400);

            if (shouldShow)
            {
                redOverlay.SetActive(true);
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * breathSpeed, 1));
                overlayGroup.alpha = alpha;
            }
            else
            {
                redOverlay.SetActive(false);
            }
        }
    }
}