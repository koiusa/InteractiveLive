using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour
{
    private AudioSource audio_;

    private MusicUnity _musicUnity;

    float[] spectrum = new float[1024];
    void Start()
    {
        _musicUnity = GetComponent<MusicUnity>();
        audio_ = _musicUnity.transform.Find("audioSource_0").GetComponent<AudioSource>();//GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_musicUnity.State == Music.PlayState.Playing)
        {
            audio_.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);
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
        }
    }
}
