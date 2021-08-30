using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class WaveOutputter : MonoBehaviour
{
    private AudioSource audio;
    float[] waveData_ = new float[1024];

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        audio.GetOutputData(waveData_, 1);

        var volume = waveData_.Select(x => x * x).Sum() / waveData_.Length;
        transform.localScale = Vector3.one * volume;
    }
}