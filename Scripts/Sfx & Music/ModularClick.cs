using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class ModularClick : MonoBehaviour
{

    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Mixer Groups")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("SFX")]
    public AudioClip buttonClickSFX;
    public float sfxVolume = 1f;
    private AudioSource sfxSource;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    sfxSource.volume = AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : sfxVolume;
    if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
        sfxSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;

    }


    public void PlayButtonClick()
    {
        if (buttonClickSFX != null && !sfxSource.isPlaying)
        {
        float volume = AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : sfxVolume;
        sfxSource.PlayOneShot(buttonClickSFX, volume);
        }
    }


}
