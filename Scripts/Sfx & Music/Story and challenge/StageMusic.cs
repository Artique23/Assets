using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMusic : MonoBehaviour

 {
    public AudioClip[] musicClips;     // Assign clips in Inspector
    public float fadeDuration = 2f;    // Time for fade in/out
    public float volume = 1f;          // Max volume

    private AudioSource[] audioSources;
    private int currentSourceIndex = 0;
    private int lastClipIndex = -1;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines(); // Stop the music loop and any fades

        if (audioSources != null)
        {
            foreach (var source in audioSources)
            {
                if (source != null)
                {
                    source.Stop();
                    source.volume = 0;
                }
            }
        }
    }
    private void Start()
    {
        // Create 2 AudioSources for crossfading
        audioSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].loop = false;
            audioSources[i].volume = 0;
        }

        StartCoroutine(PlayMusicLoop());
    }

   private IEnumerator PlayMusicLoop()
{
    // Select initial random clip
    int newClipIndex = Random.Range(0, musicClips.Length);
    lastClipIndex = newClipIndex;
    AudioClip nextClip = musicClips[newClipIndex];

    int nextSourceIndex = currentSourceIndex;
    AudioSource nextSource = audioSources[nextSourceIndex];
    nextSource.clip = nextClip;
    nextSource.volume = 0f;
    nextSource.Play();

    StartCoroutine(FadeIn(nextSource));

    yield return new WaitForSeconds(10f);

    while (true)
    {
        do
        {
            newClipIndex = Random.Range(0, musicClips.Length);
        } while (newClipIndex == lastClipIndex && musicClips.Length > 1);

        lastClipIndex = newClipIndex;
        nextClip = musicClips[newClipIndex];

        int newSourceIndex = 1 - currentSourceIndex;
        AudioSource newSource = audioSources[newSourceIndex];
        newSource.clip = nextClip;
        newSource.volume = 0f;
        newSource.Play();

        StartCoroutine(FadeIn(newSource));
        StartCoroutine(FadeOut(audioSources[currentSourceIndex]));

        currentSourceIndex = newSourceIndex;

        yield return new WaitForSeconds(nextClip.length - fadeDuration);
    }
}

    private IEnumerator FadeIn(AudioSource source)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            source.volume = Mathf.Lerp(0, volume, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = volume;
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            source.volume = Mathf.Lerp(volume, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 0;
        source.Stop();
    }
}