using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMusic : MonoBehaviour

 {
    public AudioClip[] musicClips; // Assign 3 clips in the inspector
    public float fadeDuration = 2f; // Time for fade in/out
    public float volume = 1f;       // Max volume for music

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = 0;
        StartCoroutine(PlayMusicLoop());
    }

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            AudioClip clip = musicClips[currentTrackIndex];
            audioSource.clip = clip;
            audioSource.Play();
            yield return StartCoroutine(FadeIn());

            // Wait for the clip duration minus fade time
            yield return new WaitForSeconds(clip.length - fadeDuration);

            yield return StartCoroutine(FadeOut());

            // Move to next track
            currentTrackIndex = (currentTrackIndex + 1) % musicClips.Length;
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0, volume, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = volume;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(volume, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();
    }

}
