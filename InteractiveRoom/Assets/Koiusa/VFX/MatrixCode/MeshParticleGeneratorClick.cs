using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MeshParticleGeneratorClick : MonoBehaviour
{
    public MeshParticle meshParticlePrefab;
    public float interval;
    float lastClickTime;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time - lastClickTime > interval)
        {
            lastClickTime = Time.time;
            var ray = Camera.main.ScreenPointToRay(new Vector3 (Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y,0f));

            if (Physics.Raycast(ray, out var hit))
            {
                var other = hit.collider;
                var renderer = other.GetComponent<Renderer>();
                var mp = Instantiate(meshParticlePrefab);

                mp.model = other.gameObject;
                mp.startEffect = true;

                //StartCoroutine(DestroyDelay(mp.gameObject, mp.effectDiableDelay + 1f));
            }

        }
    }
}
