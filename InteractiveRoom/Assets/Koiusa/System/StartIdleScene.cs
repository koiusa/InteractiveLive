using naichilab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartIdleScene : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Invoke("ChangeScene", 1.5f);
    }

    void ChangeScene()
    {
        //SceneManager.LoadScene("InteractiveRoom");
        FadeManager.Instance.LoadScene("InteractiveRoom", 1.0f);
    }
}
