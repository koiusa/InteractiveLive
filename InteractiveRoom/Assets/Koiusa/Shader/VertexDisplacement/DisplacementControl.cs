using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
public class DisplacementControl : MonoBehaviour
{
    public float displacementAmount;
    public ParticleSystem explosionParticles;
    MeshRenderer meshRenderer;
    StarterAssetsInputs inputs;
    void Start()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        displacementAmount = Mathf.Lerp(displacementAmount, 0, Time.deltaTime);
        meshRenderer.material.SetFloat("_Amount", displacementAmount);

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            displacementAmount += 1f;
        }
    }
}
