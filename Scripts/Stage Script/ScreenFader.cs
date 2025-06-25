using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    private CanvasGroup canvasGroup;

    

    void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public IEnumerator FadeInOut(float fadeDuration, float holdDuration)
    {
        yield return Fade(1f, fadeDuration);         // Fade to black
        yield return new WaitForSeconds(holdDuration);
        yield return Fade(0f, fadeDuration);         // Fade back in
    }
}
