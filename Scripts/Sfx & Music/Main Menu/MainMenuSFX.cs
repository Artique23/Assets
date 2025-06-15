using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MainMenuSFX : MonoBehaviour
{
    public AudioMixer audioMixer;                  // Add this in Inspector
    public AudioMixerGroup musicGroup;             // For routing
    public AudioMixerGroup sfxGroup;
    public AudioClip menuMusic;
    public float musicVolume = 1f;
    public float fadeDuration = 2f;

    [Header("SFX")]
    public AudioClip buttonClickSFX;
    public float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private bool isFadingOut = false;

    void Awake()
    {
        // Main music source
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.Play();

        // Add and configure SFX audio source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Apply current volume settings from AudioManager
        ApplyVolumesFromManager();
    }

    private void OnSceneUnloaded(Scene current)
    {
        if (!isFadingOut)
            StartCoroutine(FadeOutAndDestroy());
    }
        private void ApplyVolumesFromManager()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ApplyVolumes();
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFadingOut = true;

        float startVolume = 1f;
        audioMixer.GetFloat("MusicVolume", out float currentDb);
        startVolume = Mathf.Pow(10f, currentDb / 20f); // Convert dB to linear

        float timer = 0f;
        while (timer < fadeDuration)
        {
            float lerpedVolume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            float db = Mathf.Log10(Mathf.Clamp(lerpedVolume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("MusicVolume", db);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        audioMixer.SetFloat("MusicVolume", -80f); // Mute
        musicSource.Stop();
        Destroy(gameObject);
    }

    public void PlayButtonClick()
    {
        if (buttonClickSFX != null && !sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(buttonClickSFX);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


}
