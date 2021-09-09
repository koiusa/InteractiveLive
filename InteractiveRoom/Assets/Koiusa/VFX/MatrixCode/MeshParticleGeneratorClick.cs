using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MeshParticleGeneratorClick : MonoBehaviour
{
    public MeshParticle meshParticlePrefab;
    public float interval;
    float lastClickTime;

    public GameObject model;
    private void Awake()
    {
        startEffect(model);
    }

    private void Update()
    {
        //if (Mouse.current.leftButton.wasPressedThisFrame && Time.time - lastClickTime > interval)
        //{
        //    lastClickTime = Time.time;
        //    var ray = Camera.main.ScreenPointToRay(new Vector3 (Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y,0f));

        //    if (Physics.Raycast(ray, out var hit))
        //    {
        //        var other = hit.collider;
        //        startEffect(other.gameObject);
        //    }

        //}
    }

    private void startEffect(GameObject model)
    {
        var mp = Instantiate(meshParticlePrefab);

        mp.model = model;
        mp.startEffect = true;
    }
}
