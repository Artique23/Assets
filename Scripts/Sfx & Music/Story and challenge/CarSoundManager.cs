using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // For TMP_Text speedometer
using UnityEngine.Audio;


public class CarSoundManager : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Mixer Groups")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("Audio Sources")]
    public AudioSource idleSource;     // For idle engine hum (always playing when car is on)
    public AudioSource hornSource;     // For horn sound (plays on button press)
    public AudioSource HazardSource;
    public AudioSource LeftSignalSource;
    public AudioSource RightSignalSource;
    public AudioSource ButtonSource;

    [Header("Audio Clips")]
    public AudioClip engineIdleClip;   // Low RPM idle loop
    public AudioClip HornClip;       // Car horn sound
    public AudioClip HazardClip;     // Hazard sound (if needed, not used in this example)
    public AudioClip LeftRightSignalClip; // Left turn signal sound (if needed, not used in this example)
    public AudioClip ButtonClip;     // Button click sound (if needed, not used in this example)

    [Header("Car Controls Reference")]
    public CarControls carControls;    // Script controlling car input and state

    [Header("UI Horn Button (Assign in Inspector)")]
    public Button hornButton;
    public Button hazardButton;
    public Button leftSignalButton;
    public Button rightSignalButton;
    public Button highbeamButton;

    private bool hazardsActive = false;
    private bool leftSignalActive = false;
    private bool rightSignalActive = false;

    public static CarSoundManager Instance { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;

        hornButton?.onClick.AddListener(() => PlayHorn());
        hazardButton?.onClick.AddListener(() => ToggleLoopAudio(HazardSource, ref hazardsActive));
        leftSignalButton?.onClick.AddListener(() => ToggleLoopAudio(LeftSignalSource, ref leftSignalActive));
        rightSignalButton?.onClick.AddListener(() => ToggleLoopAudio(RightSignalSource, ref rightSignalActive));
        highbeamButton?.onClick.AddListener(() => ButtonSource?.PlayOneShot(ButtonClip));



    }
    public void StopAllCarSounds()
    {
        idleSource?.Stop();
        hornSource?.Stop();
        HazardSource?.Stop();
        LeftSignalSource?.Stop();
        RightSignalSource?.Stop();
        ButtonSource?.Stop();

        hazardsActive = false;
        leftSignalActive = false;
        rightSignalActive = false;
    }

    void Start()
    {
        // Make sure all references are assigned
        if (idleSource == null || engineIdleClip == null || carControls == null)
        {
            Debug.LogError("CarSoundManager: Missing references!");
            return;
        }

        SetupAudioSource(idleSource, engineIdleClip, true);
        SetupAudioSource(hornSource, HornClip);
        SetupAudioSource(HazardSource, HazardClip, true);
        SetupAudioSource(LeftSignalSource, LeftRightSignalClip, true);
        SetupAudioSource(RightSignalSource, LeftRightSignalClip, true);
        SetupAudioSource(ButtonSource, ButtonClip);

        SetSFXGroupAndVolume(idleSource);
        SetSFXGroupAndVolume(hornSource);
        SetSFXGroupAndVolume(HazardSource);
        SetSFXGroupAndVolume(LeftSignalSource);
        SetSFXGroupAndVolume(RightSignalSource);
        SetSFXGroupAndVolume(ButtonSource);


        // Start idle sound (it will stay on when the car is on)
        idleSource.Play();
    }

    void Update()
    {


        if (!carControls.carPoweredOn)
        {
            // If car is off, stop both sounds
            if (idleSource.isPlaying) idleSource.Stop();
            if (LeftSignalSource.isPlaying) LeftSignalSource.Stop();
            if (RightSignalSource.isPlaying) RightSignalSource.Stop();
            if (ButtonSource.isPlaying) ButtonSource.Stop();
            if (HazardSource.isPlaying) HazardSource.Stop();

            return;
        }

        // Ensure idle engine sound is always playing
        if (!idleSource.isPlaying)
        {
            idleSource.Play();
        }

        // Determine if the player is accelerating
        bool accelerating = carControls.presentAcceleration > 0.1f;

        // Blend pitch of both sounds based on speed
        float speedRatio = Mathf.Clamp01(carControls.GetCurrentSpeed() / carControls.speedLimit);
        float targetPitch = Mathf.Lerp(1f, 1.45f, speedRatio); // 1x to 2x pitch based on speed

        idleSource.pitch = Mathf.Lerp(idleSource.pitch, targetPitch, Time.deltaTime * 3f);
    }

    private void SetupAudioSource(AudioSource source, AudioClip clip, bool loop = false)
    {
        if (source != null && clip != null)
        {
            source.clip = clip;
            source.loop = loop;
            source.playOnAwake = false;
            source.volume = 1f;
        }
    }

    private void ToggleLoopAudio(AudioSource source, ref bool state)
    {
        if (source == null) return;
        state = !state;
        if (state) source.Play();
        else source.Stop();
    }

    public void PlayHorn()
    {
        if (HornClip != null && hornSource != null)
        {
            hornSource.PlayOneShot(HornClip);
        }
    }
    private void SetSFXGroupAndVolume(AudioSource source)
    {
        if (source != null && AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
        {
            source.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
            source.volume = AudioManager.Instance.sfxVolume;
        }
    }

    public void MuteCarSFX(bool mute)
    {
        if (audioMixer != null)
        {
            // Set to -80 dB (silence) when mute is true, or 0 dB (normal) when false
            audioMixer.SetFloat("SFXVolume", mute ? -80f : 0f);
        }
    }
}


//Headers 
    // public AudioSource OnOffSource; 
    //public Button onOffButton; 
    // public AudioClip CarOnClip;   
    // public AudioClip CarOffClip; 
//Awake
        // if (onOffButton != null)
        // {
        //     onOffButton.onClick.AddListener(OnOnOffPressed);
        // }

//update
        // OnOffSource.volume = 1f;
        // OnOffSource.loop = false;
        // OnOffSource.playOnAwake = false;

// private void OnOnOffPressed()
// {
//     if (OnOffSource != null)
//     {
//         if (carControls.carPoweredOn)
//         {
//             // Car is ON, so play OFF sound and turn off the car
//             if (CarOffClip != null)
//                 OnOffSource.PlayOneShot(CarOffClip);

//             carControls.carPoweredOn = false;
//         }
//         else
//         {
//             // Car is OFF, so play ON sound and turn on the car
//             if (CarOnClip != null)
//                 OnOffSource.PlayOneShot(CarOnClip);

//             carControls.carPoweredOn = true;
//         }
//     }
// }
