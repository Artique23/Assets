using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
{
    public Animator transitionFade;
    public float transitionTime = 1.5f;

    MenusUIScript menusUIScript;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    #region Loading Levels

    // LEVELS Loading Scenes Code
    public void LoadLevel1()
    {
        StartCoroutine(LoadLevel("Stage 1"));
    }
    public void LoadLevel2()
    {
        Debug.Log("Loaded Level 2...");
        SceneManager.LoadScene("Stage 2");
    }
    public void LoadLevel3()
    {
        Debug.Log("Loaded Level 3...");
        SceneManager.LoadScene("Level3Scene");
    }
    public void LoadLevel4()
    {
        Debug.Log("Loaded Level 4...");
        SceneManager.LoadScene("Level4Scene");
    }
    public void LoadLevel5()
    {
        Debug.Log("Loaded Level 5...");
        SceneManager.LoadScene("Level5Scene");
    }
    
    public void LoadChallengeMode()
    {
        Debug.Log("Loaded Challenge Mode...");
        SceneManager.LoadScene("Challenge Mode 1");
    }
    // End for LEVELS Loading Scenes Code

    // QUIZ Loading Scenes Code
    // Add this method to your Loading Levels region
    public void LoadSelectedQuiz()
    {
        // Find the MenusUIScript to get the current difficulty
        MenusUIScript menuUI = FindObjectOfType<MenusUIScript>();

        if (menuUI != null)
        {
            // Get the current selected difficulty
            int difficulty = menuUI.currentSelectedDifficulty;

            // Load the appropriate quiz based on difficulty
            switch (difficulty)
            {
                case 0: // Easy
                    Debug.Log("Loading Easy Quiz from selected difficulty...");
                    StartCoroutine(LoadLevel("1QuizEasy"));
                    break;
                case 1: // Average
                    Debug.Log("Loading Average Quiz from selected difficulty...");
                    StartCoroutine(LoadLevel("2QuizAverage"));
                    break;
                case 2: // Difficult
                    Debug.Log("Loading Difficult Quiz from selected difficulty...");
                    StartCoroutine(LoadLevel("3QuizDifficult"));
                    break;
                default:
                    Debug.LogError("Invalid difficulty selection: " + difficulty);
                    break;
            }
        }
        else
        {
            Debug.LogError("MenusUIScript not found!");
            // Fallback to Easy quiz if menu script not found
            StartCoroutine(LoadLevel("1QuizEasy"));
        }
    }
    public void LoadMainMenu()
    {
        // Reset lighting parameters before returning to main menu
        RenderSettings.fog = false;  // Turn off any fog
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;  // Reset ambient mode
        
        // Force clean up resources to prevent lighting artifacts
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        // Now load the main menu with the transition
        StartCoroutine(LoadLevel("MainMenuScene"));
    }
    // Fix the other quiz loading methods to use the animation coroutine
    
    // End for QUIZ Loading Scenes Code
    #endregion

    #region Animations for Loading Levels

    IEnumerator LoadLevel(string sceneName)
    {
        // Play the transition animation
        transitionFade.SetTrigger("Start");
        // Wait for the animation to finish
        yield return new WaitForSeconds(transitionTime);
        // Load the scene
        SceneManager.LoadScene(sceneName);
    }

    #endregion
}
