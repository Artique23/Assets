using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource engineSource;
    public AudioSource brakeSource;
    public AudioSource leftSignalSource;
    public AudioSource rightSignalSource;

    [Header("Audio Clips")]
    public AudioClip engineIdleClip;
    public AudioClip engineRevClip;
    public AudioClip brakeClip;
    public AudioClip turnSignalClip;

    [Header("Car Controls Reference")]
    public CarControls carControls;

    // Internal state
    private bool engineRevving = false;
    private bool brakePlaying = false;
    private bool leftSignalPlaying = false;
    private bool rightSignalPlaying = false;

    void Start()
    {
        if (engineSource != null && engineIdleClip != null)
        {
            engineSource.clip = engineIdleClip;
            engineSource.loop = true;
            engineSource.Play();
        }

        if (leftSignalSource != null && turnSignalClip != null)
        {
            leftSignalSource.clip = turnSignalClip;
            leftSignalSource.loop = true;
        }
        if (rightSignalSource != null && turnSignalClip != null)
        {
            rightSignalSource.clip = turnSignalClip;
            rightSignalSource.loop = true;
        }
    }

    void Update()
    {
        if (carControls == null || !carControls.carPoweredOn)
        {
            // Car is off, stop all sounds except engine idle
            if (engineSource != null && engineSource.clip != engineIdleClip)
            {
                engineSource.clip = engineIdleClip;
                engineSource.loop = true;
                engineSource.Play();
            }
            StopSound(brakeSource, ref brakePlaying);
            StopSound(leftSignalSource, ref leftSignalPlaying);
            StopSound(rightSignalSource, ref rightSignalPlaying);
            return;
        }

        // --- Engine Sound ---
        bool accelerating = carControls.presentAcceleration > 0.1f; // or carControls.pedalInput > 0.1f;
        if (accelerating && !engineRevving)
        {
            if (engineSource != null && engineRevClip != null)
            {
                engineSource.clip = engineRevClip;
                engineSource.loop = true;
                engineSource.Play();
            }
            engineRevving = true;
        }
        else if (!accelerating && engineRevving)
        {
            if (engineSource != null && engineIdleClip != null)
            {
                engineSource.clip = engineIdleClip;
                engineSource.loop = true;
                engineSource.Play();
            }
            engineRevving = false;
        }

        // Optionally: blend pitch with speed for realism
        if (engineSource != null)
        {
            float speed = carControls.GetCurrentSpeed();
            engineSource.pitch = Mathf.Lerp(1f, 2f, speed / carControls.speedLimit);
        }

        // --- Brake Sound ---
        bool braking = carControls.brakeInput > 0.1f;
        if (braking && !brakePlaying)
        {
            if (brakeSource != null && brakeClip != null)
            {
                brakeSource.clip = brakeClip;
                brakeSource.loop = false;
                brakeSource.Play();
            }
            brakePlaying = true;
        }
        else if (!braking && brakePlaying)
        {
            StopSound(brakeSource, ref brakePlaying);
        }

        // --- Turn Signals ---
        if (carControls.leftSignalOn && !leftSignalPlaying)
        {
            if (leftSignalSource != null)
            {
                leftSignalSource.Play();
            }
            leftSignalPlaying = true;
        }
        else if (!carControls.leftSignalOn && leftSignalPlaying)
        {
            StopSound(leftSignalSource, ref leftSignalPlaying);
        }

        if (carControls.rightSignalOn && !rightSignalPlaying)
        {
            if (rightSignalSource != null)
            {
                rightSignalSource.Play();
            }
            rightSignalPlaying = true;
        }
        else if (!carControls.rightSignalOn && rightSignalPlaying)
        {
            StopSound(rightSignalSource, ref rightSignalPlaying);
        }
    }

    void StopSound(AudioSource source, ref bool stateFlag)
    {
        if (source != null)
        {
            source.Stop();
        }
        stateFlag = false;
    }
}
