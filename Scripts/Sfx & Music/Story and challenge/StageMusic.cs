using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMusic : MonoBehaviour

 {
    public AudioClip[] musicClips;     // Assign clips in Inspector
    public float fadeDuration = 2f;    // Time for fade in/out
    public float volume = 1f;          // Max volume

    private AudioSource[] audioSources;
    private int currentSourceIndex = 0;
    private int lastClipIndex = -1;

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
         yield return new WaitForSeconds(10f);
        while (true)
        {
            // Get a random clip index that's not the same as the last
            int newClipIndex;
            do
            {
                newClipIndex = Random.Range(0, musicClips.Length);
            } while (newClipIndex == lastClipIndex && musicClips.Length > 1);

            lastClipIndex = newClipIndex;
            AudioClip nextClip = musicClips[newClipIndex];

            int nextSourceIndex = 1 - currentSourceIndex; // Alternate between 0 and 1
            AudioSource nextSource = audioSources[nextSourceIndex];
            nextSource.clip = nextClip;
            nextSource.Play();

            StartCoroutine(FadeIn(nextSource));
            StartCoroutine(FadeOut(audioSources[currentSourceIndex]));

            currentSourceIndex = nextSourceIndex;

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