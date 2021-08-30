using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour
{
    [Header("Setting")]
    [Tooltip("If checked, each value will be previewed on the screen.")]
    public bool DebugView = false;
    [SerializeField, Range(0, SampleCount-1)]
    private uint DebugView_Hz = 0;

    private const uint SampleCount = 1024;
    private const float ClampBottom = -80.0f;
    private const float FalldownPerTick = 0.1f;
    private const float LevelRange = 5.0f;

    private AudioSource _audio;
    private MusicUnity _musicUnity;

    float[] spectrum = new float[SampleCount];

    public float[] Hz { get; set; }
    public float[] DbfsPerHz { get; set; }
    public float Pitch { get; set; }
    public float Dbfs { get; set; }
    public float Peak { get; set; }
    public float CurrentPeak { get; set; }
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
            }
            Hz = CalcHz(spectrum);
            DbfsPerHz = CalcDecibelPerHz();
            Pitch = CalcPitch(spectrum);
            Dbfs = CalcDecibel();
            CurrentPeak = CalcPeakFallDownValue(Dbfs);
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

    private float[] CalcDecibelPerHz()
    {
        float[] dbValue = new float[SampleCount];
        float[] samples = new float[SampleCount];

        _audio.GetOutputData(samples, 0);
        int i = 0;
        foreach (var sample in samples)
        {
            var pow = sample * sample;
            var rmsValue = Mathf.Sqrt(pow / SampleCount);
            dbValue[i] = 20.0f * Mathf.Log10(rmsValue);
            dbValue[i] = Mathf.Clamp(dbValue[i], ClampBottom, float.MaxValue);
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
    /// bin Ç128å¬Ç…ï™óﬁÇµÇΩÇ‡ÇÃÇ©ÇÁÅAì¡íËé¸îgêîãÊàÊÇÃílÇÃïΩãœíl
    /// </summary>
    /// <param name="spectrum"></param>
    /// <returns></returns>
    private float CalcPitch(float[] spectrum)
    {
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

            return f * AudioSettings.outputSampleRate * 0.5f * maxIndex / SampleCount;
        }
        else
        {
            return 0;
        }
    }

    private float CalcPeakFallDownValue(float db)
    {
        var delta = Time.deltaTime;

        Peak = Mathf.Max(Peak - FalldownPerTick * delta, ClampBottom);
        Peak = Mathf.Clamp(db, Peak, 0.0f);

        var minValue = Peak - LevelRange;
        return (Mathf.Clamp(db, minValue, Peak) - minValue) / LevelRange;
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
                GUILayout.Label($"CurrentPeak: {CurrentPeak}");
            }
        }
    }
}
