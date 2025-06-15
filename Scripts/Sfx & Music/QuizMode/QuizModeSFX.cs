using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class QuizModeSFX : MonoBehaviour
{


    [Header("Mixer")]
    public AudioMixer audioMixer;


    [Header("Mixer Groups")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    public AudioSource sfxSource;
    public AudioSource timerWarningSource;

    [Header("SFX Clips")]
    public AudioClip buttonClickClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip timerWarningClip;
    

    public void PlayButtonClick() => PlaySFX(buttonClickClip);
    public void PlayCorrect() => PlaySFX(correctClip);
    public void PlayWrong() => PlaySFX(wrongClip);
    // public void PlayTimerWarning() => PlaySFX(timerWarningClip);

    void Awake()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null && sfxSource != null)
            sfxSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null && timerWarningSource != null)
        timerWarningSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
    }
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 1f);
    }
    
    public void PlayTimerWarning()
{
    if (timerWarningClip != null && timerWarningSource != null && !timerWarningSource.isPlaying)
    {
        timerWarningSource.clip = timerWarningClip;
        timerWarningSource.loop = true;
        timerWarningSource.Play();
    }
}
    public void StopWarning()
    {
        if (timerWarningSource != null && timerWarningSource.isPlaying)
            timerWarningSource.Stop();
    }
}
