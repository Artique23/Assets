using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ModularClick : MonoBehaviour
{


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
        sfxSource.volume = sfxVolume;

    }


    public void PlayButtonClick()
    {
        if (buttonClickSFX != null && !sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(buttonClickSFX, sfxVolume);
        }
    }


}
