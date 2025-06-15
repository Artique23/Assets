using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; 

public class MixAndMatchSFX : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;
    [Header("Quiz Logic")]
    public QuizManager quizManager;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Mixer Groups")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioSource sfxSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip instructionAudioClip;
    public AudioClip summaryPanelClip;

    void Awake()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null && sfxSource != null)
            sfxSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
    }
    public void PlayCorrect()
    {
        if (sfxSource != null && correctClip != null)
            sfxSource.PlayOneShot(correctClip, AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 1f);

    }

    public void PlayWrong()
    {
        if (sfxSource != null && wrongClip != null)
            sfxSource.PlayOneShot(wrongClip, AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 1f);

    }
        public void ShowPanel()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(true);

        PlayInstructionAudio();
    }

    public void HidePanel()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        sfxSource.Stop();
    }
    public void PlaySummaryPanelSFX()
    {
        if (summaryPanelClip != null && sfxSource != null)
            sfxSource.PlayOneShot(summaryPanelClip);
    }

    public void PlayInstructionAudio()
    {
        if (instructionAudioClip != null)
        {
            float volume = AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 1f;
            sfxSource.PlayOneShot(instructionAudioClip, volume);
        }
    }
}