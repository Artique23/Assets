using UnityEngine;
using UnityEngine.SceneManagement;

public class LightingReset : MonoBehaviour
{
    void OnEnable()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only execute this in the main menu scene
        if (scene.name == "MainMenuScene")
        {
            // Force realtime lighting update
            
            DynamicGI.UpdateEnvironment();
            
            // If you have baked lighting, also reset shadow maps
            ResetShadows();
        }
    }

    void ResetShadows()
    {
        // Find all lights in the scene
        Light[] lights = FindObjectsOfType<Light>();
        
        foreach (Light light in lights)
        {
            // Toggle shadow type to force shadow recalculation
            if (light.shadows != LightShadows.None)
            {
                LightShadows original = light.shadows;
                light.shadows = LightShadows.None;
                
                // Use Invoke to reset after a short delay
                StartCoroutine(ResetLightShadows(light, original, 0.1f));
            }
        }
    }
    
    private System.Collections.IEnumerator ResetLightShadows(Light light, LightShadows originalShadowType, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Reset to original shadow type
        if (light != null)
            light.shadows = originalShadowType;
    }
}