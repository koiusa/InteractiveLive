using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Koiusa.InteractiveRoom
{
    public class SizeFitter : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer TargetSprite = null;

        [SerializeField]
        RectTransform SourceRect = null;

        [SerializeField]
        float padding = 0.2f;
        // Update is called once per frame
        void Update()
        {
            TargetSprite.size = new Vector2(SourceRect.rect.size.x + padding * 2f, SourceRect.rect.size.y + padding);
        }
    }
}