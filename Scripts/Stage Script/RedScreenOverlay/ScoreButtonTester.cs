using UnityEngine;

public class ScoreButtonTester : MonoBehaviour
{
    public void Add200Points()
    {
        if (StageScoreManager.Instance != null)
            StageScoreManager.Instance.AddPoints(200);
    }

    public void Subtract200Points()
    {
        if (StageScoreManager.Instance != null)
            StageScoreManager.Instance.AddPoints(-200);
    }
}