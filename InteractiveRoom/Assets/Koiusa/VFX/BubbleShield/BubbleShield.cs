using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    Renderer _renderer;
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] float _DisplacementMagnitude;
    [SerializeField] float _LerpSpeed;
    [SerializeField] float _DisolveSpeed;
    bool _ShieldOn;
    Coroutine _DisolveCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                HitShield(hit.point);
            }
        }
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            OpenCloseShield();
        }
    }

    public void HitShield(Vector3 hitpos)
    {
        _renderer.material.SetVector("HitPosition", hitpos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    public void OpenCloseShield()
    {
        float target = 1;
        if (_ShieldOn)
        {
            target = 0;
        }
        _ShieldOn = !_ShieldOn;
        if(_DisolveCoroutine != null)
        {
            StopCoroutine(_DisolveCoroutine);
        }
        _DisolveCoroutine = StartCoroutine(Coroutine_DisolveShield(target));
    }

    IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while(lerp < 1)
        {
            _renderer.material.SetFloat("DisplacementStrength", _DisplacementCurve.Evaluate(lerp) * _DisplacementMagnitude);
            lerp += Time.deltaTime * _LerpSpeed;
            yield return null;
        }
    }

    IEnumerator Coroutine_DisolveShield(float target)
    {
        float start = _renderer.material.GetFloat("Disolve");
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("Disolve", Mathf.Lerp(start,target,lerp));
            lerp += Time.deltaTime * _DisolveSpeed;
            yield return null;
        }
    }
}
