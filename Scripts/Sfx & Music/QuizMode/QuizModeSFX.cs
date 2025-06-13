using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizModeSFX : MonoBehaviour
{
 public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip buttonClickClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip timerWarningClip;

    public void PlayButtonClick() => PlaySFX(buttonClickClip);
    public void PlayCorrect() => PlaySFX(correctClip);
    public void PlayWrong() => PlaySFX(wrongClip);
    public void PlayTimerWarning() => PlaySFX(timerWarningClip);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}
