using naichilab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class StartIdleScene : MonoBehaviour
{
    UnityAction task;
    // Use this for initialization
    void Start()
    {
        task = ChangeScene;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            task.Invoke();
        }
    }

    void ChangeScene()
    {
        //SceneManager.LoadScene("InteractiveRoom");
        FadeManager.Instance.LoadScene("InteractiveRoom", 1.0f);
    }
}
