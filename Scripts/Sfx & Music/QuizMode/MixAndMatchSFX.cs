using UnityEngine;

public class MixAndMatchSFX : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    public void PlayCorrect()
    {
        if (sfxSource != null && correctClip != null)
            sfxSource.PlayOneShot(correctClip);
    }

    public void PlayWrong()
    {
        if (sfxSource != null && wrongClip != null)
            sfxSource.PlayOneShot(wrongClip);
    }
}