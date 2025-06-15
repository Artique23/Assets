using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RodmanAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator rodmanAnimator;
    [SerializeField] private Button buyCarButton; // Changed from shockButton
    private string[] animations = { "Look", "Wave", "Clap" };
    private bool isAnimating = false;

    void Start()
    {
        StartCoroutine(PlayRandomAnimation());
        
        // Add button listener for buy car
        if (buyCarButton != null)
        {
            buyCarButton.onClick.AddListener(PlayBuyCarReaction);
        }
    }

    // Renamed method to better reflect its purpose
    public void PlayBuyCarReaction()
    {
        if (!isAnimating)
        {
            StartCoroutine(PlayShock());
        }
    }

    // Add this coroutine
    private IEnumerator PlayShock()
    {
        isAnimating = true;
        rodmanAnimator.SetTrigger("Shock");
        yield return new WaitForSeconds(GetAnimationLength("Shock"));
        isAnimating = false;
    }

    private IEnumerator PlayRandomAnimation()
    {
        while (true)
        {
            if (!isAnimating)
            {
                // Wait for 8 seconds of idle
                yield return new WaitForSeconds(6.5f);

                // Choose and play random animation
                int randomIndex = Random.Range(0, animations.Length);
                rodmanAnimator.SetTrigger(animations[randomIndex]);
                isAnimating = true;

                // Wait for current animation to finish
                yield return new WaitForSeconds(GetAnimationLength(animations[randomIndex]));
                isAnimating = false;
            }
            yield return null;
        }
    }

    private float GetAnimationLength(string animationName)
    {
        // Get the animation clip length from the animator
        AnimatorClipInfo[] clipInfo = rodmanAnimator.GetCurrentAnimatorClipInfo(0);
        foreach (AnimatorClipInfo clip in clipInfo)
        {
            if (clip.clip.name.Contains(animationName))
            {
                return clip.clip.length;
            }
        }
        return 1f; // Default duration if animation not found
    }
}
