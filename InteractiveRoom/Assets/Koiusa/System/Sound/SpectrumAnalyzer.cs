using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour
{
    [Header("Setting")]
    [Tooltip("If checked, each value will be previewed on the screen.")]
    public bool DebugView = false;
    [SerializeField, Range(0, SampleCount - 1)]
    private uint DebugView_Hz = 0;

    private const uint SampleCount = 1024;
    private const float ClampBottom = -80.0f;
    private const float FalldownPerTick = 0.1f;

    private AudioSource _audio;
    private MusicUnity _musicUnity;

    float[] spectrum = new float[SampleCount];
    float[] samples = new float[SampleCount];

    List<float> prevDbfs = new List<float>();
    List<float> prevPich = new List<float>();

    public float[] Hz { get; set; }
    public float[] DbfsPerHz { get; set; }
    public float Pitch { get; set; }
    public float Dbfs { get; set; }
    public float Peak { get; set; }
    void Start()
    {
        _musicUnity = GetComponent<MusicUnity>();
        _audio = _musicUnity.transform.Find("audioSource_0").GetComponent<AudioSource>();
        Peak = 0;
    }

    void Update()
    {
        if (_musicUnity.State == Music.PlayState.Playing)
        {
            _audio.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);
            _audio.GetOutputData(samples, 0);
            //for (int i = 1; i < spectrum.Length - 1; ++i)
            //{
            //    Debug.DrawLine(
            //            new Vector3(i - 1, spectrum[i] + 10, 0),
            //            new Vector3(i, spectrum[i + 1] + 10, 0),
            //            Color.red);
            //    Debug.DrawLine(
            //            new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2),
            //            new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2),
            //            Color.cyan);
            //    Debug.DrawLine(
            //            new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1),
            //            new Vector3(Mathf.Log(i), spectrum[i] - 10, 1),
            //            Color.green);
            //    Debug.DrawLine(
            //            new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3),
            //            new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3),
            //            Color.yellow);
            //}
            Hz = CalcHz(spectrum);
            DbfsPerHz = CalcDecibelPerHz();
            Pitch = CalcPitch(spectrum);
            Dbfs = CalcDecibel();
            Peak = CalcPeakFallDownValue(Dbfs);
            WaveVisualize();
        }

    }

    private float CalcDecibel()
    {
        var sum = 0.0f;
        foreach (var sample in samples)
        {
            sum += sample * sample;
        }
        var rmsValue = Mathf.Sqrt(sum / SampleCount);
        var dbValue = 20.0f * Mathf.Log10(rmsValue);
        dbValue = Mathf.Clamp(dbValue, ClampBottom, float.MaxValue);
        return dbValue + Mathf.Abs(ClampBottom);
    }

    private float[] CalcDecibelPerHz()
    {
        float[] dbValue = new float[SampleCount];

        int i = 0;
        foreach (var sample in samples)
        {
            var pow = sample * sample;
            var rmsValue = Mathf.Sqrt(pow);
            dbValue[i] = 20.0f * Mathf.Log10(rmsValue);
            dbValue[i] = Mathf.Clamp(dbValue[i], ClampBottom, float.MaxValue) + Mathf.Abs(ClampBottom);
            i++;
        }
        return dbValue;
    }

    private float[] CalcHz(float[] spectrum)
    {
        float[] hz = new float[SampleCount];
        for (int i = 0; i < spectrum.Length; i++)
        {
            hz[i] = AudioSettings.outputSampleRate * 0.5f * i / SampleCount;
        }
        return hz;
    }

    /// <summary>
    /// bin を128個に分類したものから、特定周波数区域の値の平均値
    /// </summary>
    /// <param name="spectrum"></param>
    /// <returns></returns>
    private float CalcPitch(float[] spectrum)
    {
        var size = 1000; //数値が大きすぎるので単位変換
        var maxValue = 0.0f;
        var maxIndex = 0;

        for (var i = 0; i < SampleCount; i++)
        {
            var val = spectrum[i];
            if (maxValue > val)
                continue;

            maxValue = val;
            maxIndex = i;
        }

        if (maxIndex > 0)
        {
            var l = spectrum[maxIndex - 1] / spectrum[maxIndex];
            var r = spectrum[maxIndex + 1] / spectrum[maxIndex];
            var f = maxIndex + 0.5f * (r * r - l * l);

            return (f * AudioSettings.outputSampleRate * 0.5f * maxIndex / SampleCount) / size;
        }
        else
        {
            return 0;
        }
    }

    private float CalcPeakFallDownValue(float db)
    {
        var currentPeak = Mathf.Max(db, Peak);
        return Mathf.Lerp(currentPeak, 0.0f, FalldownPerTick * Time.deltaTime);
    }

    private void WaveVisualize()
    {
        for (int i = 1; i < spectrum.Length - 1; ++i)
        {
            Debug.DrawLine(
                    new Vector3(i - 1, DbfsPerHz[i - 1], 0),
                    new Vector3(i, DbfsPerHz[i], 0),
                    Color.yellow);
        }
        for (int i = 1; i < spectrum.Length - 1; ++i)
        {
            if (prevDbfs.Count > i)
            {
                Debug.DrawLine(
                    new Vector3(i - 1, prevDbfs[i - 1], 0),
                    new Vector3(i, prevDbfs[i], 0),
                    Color.red);
            }
            if (prevPich.Count > i)
            {
                Debug.DrawLine(
                    new Vector3(i - 1, prevPich[i - 1], 0),
                    new Vector3(i, prevPich[i], 0),
                    Color.cyan);
            }
        }
        prevDbfs.Add(Dbfs);
        while (prevDbfs.Count > SampleCount)
        {
            prevDbfs.RemoveAt(0);
        }
        prevPich.Add(Pitch);
        while (prevPich.Count > SampleCount)
        {
            prevPich.RemoveAt(0);
        }
    }

    void OnGUI()
    {
        if (DebugView)
        {
            if (_musicUnity.State == Music.PlayState.Playing)
            {
                GUILayout.Label($"Hz: {Hz[DebugView_Hz]}");
                GUILayout.Label($"DbfsPerHz: {DbfsPerHz[DebugView_Hz]}");
                GUILayout.Label($"Dbfs: {Dbfs}");
                GUILayout.Label($"Pitch: {Pitch}");
                GUILayout.Label($"Peak: {Peak}");
                GUILayout.Label($"spectrum: {spectrum[DebugView_Hz]}");
                GUILayout.Label($"samples: {samples[DebugView_Hz]}");
            }
        }
    }
}
