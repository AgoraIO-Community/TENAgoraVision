﻿using UnityEngine;
using Agora.Rtc;

public class SphereVisualizer : ISoundVisualizer
{
    [SerializeField]
    float sizeMultiplier = 10f; // Multiplier for sphere size

    [SerializeField]
    float rotationSpeed = 100.0f; // Rotation speed in degrees per second

    [SerializeField]
    bool showSizeMotion;

    [SerializeField]
    bool showRotationMotion;

    public int CHANNEL = 1;
    public int SAMPLE_RATE = 44100;

    float[] _pcmData = new float[0];

    private void Update()
    {
        if (showSizeMotion)
        {
            ShowSizeMotion();
        }
        if (showRotationMotion)
        {
            ShowRotationMotion();
        }
    }

    public override void Init(IRtcEngine RtcEngine)
    {
        RtcEngine.SetPlaybackAudioFrameBeforeMixingParameters(SAMPLE_RATE, CHANNEL);
        RtcEngine.RegisterAudioFrameObserver(new AgoraAudioObserver(this),
             AUDIO_FRAME_POSITION.AUDIO_FRAME_POSITION_BEFORE_MIXING,
            OBSERVER_MODE.RAW_DATA);
    }

    public override void UpdateVisualizer(float[] pcmData)
    {
        _pcmData = pcmData;
    }

    void ShowRotationMotion()
    {
        float energy = GetAverageEnergy();
        // Rotate the sphere around the y-axis
        transform.Rotate(0, (energy * sizeMultiplier * 450 + rotationSpeed) * Time.deltaTime, 0);
    }

    void ShowSizeMotion()
    {
        float energy = GetAverageEnergy();
        // Adjust the size of the sphere based on the average energy
        float newSize = 1f + energy * sizeMultiplier;
        transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    float GetAverageEnergy()
    {
        float sum = 0.0000001f;
        if (_pcmData.Length == 0)
        {
            return sum;
        }


        // Calculate the sum of the absolute values of the PCM data
        foreach (float sample in _pcmData)
        {
            sum += Mathf.Abs(sample);
        }

        // Calculate the average energy
        float average = sum / _pcmData.Length;
        return average;
    }
}
