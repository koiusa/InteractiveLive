using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour
{
    private const uint SampleCount = 1024;
    private const float ClampBottom = -80.0f;
    private const float FalldownPerTick = 0.1f;
    private const float LevelRange = 5.0f;

    private AudioSource _audio;
    private MusicUnity _musicUnity;

    float[] spectrum = new float[SampleCount];
    void Start()
    {
        _musicUnity = GetComponent<MusicUnity>();
        _audio = _musicUnity.transform.Find("audioSource_0").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_musicUnity.State == Music.PlayState.Playing)
        {
            _audio.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);
            for (int i = 1; i < spectrum.Length - 1; ++i)
            {
                Debug.DrawLine(
                        new Vector3(i - 1, spectrum[i] + 10, 0),
                        new Vector3(i, spectrum[i + 1] + 10, 0),
                        Color.red);
                Debug.DrawLine(
                        new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2),
                        new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2),
                        Color.cyan);
                Debug.DrawLine(
                        new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1),
                        new Vector3(Mathf.Log(i), spectrum[i] - 10, 1),
                        Color.green);
                Debug.DrawLine(
                        new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3),
                        new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3),
                        Color.yellow);

                var hz = AudioSettings.outputSampleRate * 0.5f * i / SampleCount;

            }
            var pitch = CalcPitch(spectrum);
            var dbfs = CalcDecibel();
            var peek = CalcPeakFallDownValue(dbfs);
        }

    }

    private float CalcDecibel()
    {
        float[] samples = new float[SampleCount];

        _audio.GetOutputData(samples, 0);

        var sum = 0.0f;
        foreach (var sample in samples)
        {
            sum += sample * sample;
        }

        var rmsValue = Mathf.Sqrt(sum / SampleCount);
        var dbValue = 20.0f * Mathf.Log10(rmsValue);
        dbValue = Mathf.Clamp(dbValue, ClampBottom, float.MaxValue);
        return dbValue;
    }

    private float CalcPitch(float[] samples)
    {
        var maxValue = 0.0f;
        var maxIndex = 0;

        for (var i = 0; i < SampleCount; i++)
        {
            var spectrum = samples[i];
            if (maxValue > spectrum)
                continue;

            maxValue = spectrum;
            maxIndex = i;
        }

        if (maxIndex > 0)
        {
            var l = samples[maxIndex - 1] / samples[maxIndex];
            var r = samples[maxIndex + 1] / samples[maxIndex];
            var f = maxIndex + 0.5f * (r * r - l * l);

            return f * AudioSettings.outputSampleRate * 0.5f * maxIndex / SampleCount;
        }
        else
        {
            return 0;
        }
    }

    private float CalcPeakFallDownValue(float db)
    {
        float peak = 0;
        var delta = Time.deltaTime;

        peak = Mathf.Max(peak - FalldownPerTick * delta, ClampBottom);
        peak = Mathf.Clamp(db, peak, 0.0f);

        var minValue = peak - LevelRange;
        return (Mathf.Clamp(db, minValue, peak) - minValue) / LevelRange;
    }
}
