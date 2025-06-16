using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScoreManager : MonoBehaviour
{

    public static StageScoreManager Instance;
    public int totalPoints = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persists score if scene reloads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        totalPoints += points;
        Debug.Log("Points updated. Total: " + totalPoints);
    }

    public int GetPoints()
    {
        return totalPoints;
    }

    // Add this to your StageScoreManager.cs file
    public void SetPointsForTesting(int points)
    {
        this.totalPoints = points;
    }
}
