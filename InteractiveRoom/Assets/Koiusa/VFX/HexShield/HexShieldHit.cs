using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HexShieldHit : MonoBehaviour
{
    public GameObject ripplesVFX;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var ripples = Instantiate(ripplesVFX, transform) as GameObject;
            var visualEffect = ripples.transform.GetComponent<VisualEffect>();
            visualEffect.SetVector3("SpherePosition", transform.position);
            visualEffect.SetVector3("HitPosition", collision.contacts[0].point);
            Destroy(ripples, 2);
        }
    }
}
