using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MainMenuSFX : MonoBehaviour
{
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
        DontDestroyOnLoad(gameObject);

        // Main music source (this component)
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();

        // Add and configure SFX audio source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene current)
    {
        if (!isFadingOut)
            StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFadingOut = true;

        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
        Destroy(gameObject);
    }

    public void PlayButtonClick()
    {
        if (buttonClickSFX != null && !sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(buttonClickSFX, sfxVolume);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


}
