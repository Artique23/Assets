using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    public GameObject pausePanel; // Assign your PausePanel in the Inspector

    public LevelLoader levelLoader; // Reference to LevelLoader for scene management

    void Start()
    {
        // Find LevelLoader if not assigned in Inspector
        if (levelLoader == null)
            levelLoader = FindObjectOfType<LevelLoader>();
            
        if (levelLoader == null)
            Debug.LogWarning("LevelLoader not found! Main menu and retry functions won't work.");
            
        if (pausePanel != null)
            pausePanel.SetActive(false); // Hide pause panel on start
    }

    // Call this method from your Pause Button's OnClick()
    public void PauseGame()
    {
        Time.timeScale = 0f; // Pauses the game
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    // Call this from ResumeButton's OnClick()
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume normal time
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // Call this from MainMenuButton's OnClick()
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Always reset timescale before scene change!
        
        if (levelLoader != null)
        {
            levelLoader.LoadMainMenu(); // Load the main menu scene
        }
        else
        {
            Debug.LogWarning("LevelLoader not found, loading main menu directly");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f; // Reset time scale
        
        if (levelLoader != null)
        {
            StartCoroutine(RetryCurrentLevel());
        }
        else
        {
            Debug.LogWarning("LevelLoader not found, reloading scene directly");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    // Helper method to handle coroutine
    private IEnumerator RetryCurrentLevel()
    {
        if (levelLoader != null)
        {
            yield return StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().name));
        }
    }
}
