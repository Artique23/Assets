using UnityEngine;

public class TutorialPanelHandler : MonoBehaviour
{
    public GameObject tutorialPanel;      // Parent holding all tutorial panels
    public GameObject[] panels;           // Individual panel GameObjects
    public GameObject cutsceneObject;     // Cutscene GameObject to check

    private int currentPanelIndex = 0;
    private bool tutorialStarted = false;

    void Start()
    {
        tutorialPanel.SetActive(false); // Hide tutorial initially
    }

    void Update()
    {
        if (!tutorialStarted && cutsceneObject != null && !cutsceneObject.activeInHierarchy)
        {
            tutorialStarted = true;
            StartTutorial();
        }
    }

    void StartTutorial()
    {
        tutorialPanel.SetActive(true);
        currentPanelIndex = 0;
        ShowPanel(currentPanelIndex);
    }

    public void NextPanel()
    {
        if (currentPanelIndex < panels.Length - 1)
        {
            currentPanelIndex++;
            ShowPanel(currentPanelIndex);
        }
        else
        {
            EndTutorial();
        }
    }

    public void PreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            ShowPanel(currentPanelIndex);
        }
    }

    void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        // Add any post-tutorial logic here (e.g., enable player control)
    }
}
