using UnityEngine;
using System.Collections;

// 空の Audio Source を作って置く
[RequireComponent(typeof(AudioSource))]
public class MicrophoneSource : MonoBehaviour
{
    void Start()
    {
        // 空の Audio Sourceを取得
        var audio = GetComponent<AudioSource>();
        // Audio Source の Audio Clip をマイク入力に設定
        // 引数は、デバイス名（null ならデフォルト）、ループ、何秒取るか、サンプリング周波数
        audio.clip = Microphone.Start(null, false, 10, 44100);
        // マイクが Ready になるまで待機（一瞬）
        while (Microphone.GetPosition(null) <= 0) { }
        // 再生開始（録った先から再生、スピーカーから出力するとハウリングします）
        audio.Play();
    }
}