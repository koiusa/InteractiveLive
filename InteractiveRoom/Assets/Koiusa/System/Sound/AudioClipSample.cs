using UnityEngine;
using System.Collections;

public class AudioClipSample : MonoBehaviour
{
    public AudioClip clip;
    public int lengthYouWant;

    void Start()
    {
        var data = new float[lengthYouWant];
        clip.GetData(data, 0);
    }
}