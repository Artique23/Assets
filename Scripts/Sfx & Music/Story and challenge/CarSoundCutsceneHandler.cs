using UnityEngine;

public class CarSoundCutsceneHandler : MonoBehaviour
{
    private CutsceneScript cutscene;

    void Start()
    {
        // Try to find the cutscene script in the scene
        cutscene = FindObjectOfType<CutsceneScript>();

        if (cutscene == null)
        {
            Debug.LogWarning("No CutsceneScript found in the scene.");
            return;
        }

        // Check if the cutscene is already active when scene loads
        if (cutscene.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false); // Disable this car sound root
        }

        // Listen for when the cutscene completes
        cutscene.onCutsceneComplete.AddListener(ReactivateSelf);
    }

    private void ReactivateSelf()
    {
        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (cutscene != null)
            cutscene.onCutsceneComplete.RemoveListener(ReactivateSelf);
    }
}
