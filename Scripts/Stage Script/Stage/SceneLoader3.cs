using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader3 : MonoBehaviour
{
    // Call this method to load Stage3
    public void GoToStage3()
    {
        SceneManager.LoadScene("Stage 3");
    }
}