using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader2 : MonoBehaviour
{
    // Call this method to load Stage2
    public void GoToStage2()
    {
        SceneManager.LoadScene("Stage 2");
    }
}