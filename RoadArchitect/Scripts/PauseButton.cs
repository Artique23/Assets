using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    public GameObject pausePanel; // Assign your PausePanel in the Inspector


    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // Hide pause panel on start
    }

    // Call this method from your Pause Button’s OnClick()
    public void PauseGame()
    {
        Time.timeScale = 0f; // Pauses the game
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    // Call this from ResumeButton’s OnClick()
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume normal time
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // Call this from MainMenuButton’s OnClick()
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Always reset timescale before scene change!
        SceneManager.LoadScene("MainMenuScene"); // Change this to your main menu scene name
    }
}
