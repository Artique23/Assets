using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenusUIScript : MonoBehaviour
{
    [SerializeField] GameObject GeneralUI, MainMenuUI, StartMenuUI, StoryModeUI,SettingsUI;
    // Start is called before the first frame update
    void Start()
    {
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(true);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // LEVELS Loading Scenes Code
    public void LoadLevel1()
    {
        Debug.Log("Loaded Level 1...");
        SceneManager.LoadScene("Level1Scene");
    }
    public void LoadLevel2()
    {
        Debug.Log("Loaded Level 2...");
        SceneManager.LoadScene("Level2Scene");
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
    // End for LEVELS Loading Scenes Code

    // QUIZ Loading Scenes Code
    public void LoadMCQEasy()
    {
        Debug.Log("Loaded MCQEasy...");
        SceneManager.LoadScene("1QuizEasy");
    }
    public void LoadMCQAverage()
    {
        Debug.Log("Loaded MCQAverage...");
        SceneManager.LoadScene("2QuizAverage");
    }
        public void LoadMCQDifficult()
    {
        Debug.Log("Loaded MCQAverage...");
        SceneManager.LoadScene("3QuizDifficult");
    }
    // End for QUIZ Loading Scenes Code
}