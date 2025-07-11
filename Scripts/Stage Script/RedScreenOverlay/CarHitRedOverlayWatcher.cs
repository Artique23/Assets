using UnityEngine;
using System.Reflection;

public class CarHitRedOverlayWatcher : MonoBehaviour
{
    public CarControls carControls;         // Assign in Inspector
    public RedScreenEffect redScreenEffect; // Assign in Inspector

    private bool overlayShown = false;
    private FieldInfo hitsField;

    void Start()
    {
        // Cache the field info for efficiency
        hitsField = typeof(CarControls).GetField("autonomousVehicleHits", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void Update()
    {
        if (carControls != null && redScreenEffect != null && hitsField != null)
        {
            int hits = (int)hitsField.GetValue(carControls);

            if (hits == 2)
            {
                if (!overlayShown)
                {
                    redScreenEffect.ForceShowOverlay(true);
                    overlayShown = true;
                }
            }
            else
            {
                if (overlayShown)
                {
                    redScreenEffect.ForceShowOverlay(false);
                    overlayShown = false;
                }
            }
        }
    }
}