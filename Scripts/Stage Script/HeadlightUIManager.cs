using UnityEngine;
using UnityEngine.UI;

public class HeadlightUIManager : MonoBehaviour
{
    public Image batteryFillImage; // Must be a Filled Image type
    public CarlightController carlightController;

    void Update()
    {
        if (carlightController == null || batteryFillImage == null) return;

        float current = carlightController.GetCurrentHeadlightTime();
        float max = carlightController.maxHeadlightTime;

        batteryFillImage.fillAmount = current / max;
    }
}
