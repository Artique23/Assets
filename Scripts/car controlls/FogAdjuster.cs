using UnityEngine;

public class FogAdjuster : MonoBehaviour
{
    public CarlightController carlightController; // Drag your player car's CarlightController here

    [Header("Fog Settings")]
    public float fogWithHeadlights = 0.05f;
    public float fogWithoutHeadlights = 0.2f;

    void Update()
    {
        if (carlightController == null) return;

        // Check if headlights are ON and set fog accordingly
        if (carlightController.HeadlightsAreOn())
        {
            RenderSettings.fogDensity = fogWithHeadlights;
        }
        else
        {
            RenderSettings.fogDensity = fogWithoutHeadlights;
        }
    }

    
}
