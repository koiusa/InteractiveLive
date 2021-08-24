//  UIFollowTarget.cs
//  https://tsubakit1.hateblo.jp/entry/2016/03/01/020510

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Koiusa.InteractiveRoom
{
	public class UIFollowTarget : MonoBehaviour
	{
		protected RectTransform rectTransform = null;

		[SerializeField] 
		public Transform target = null;

		[SerializeField]
		public Canvas canvas;

		void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}

		void Update()
		{
			if (target != null)
			{
				var pos = Vector2.zero;
				var uiCamera = Camera.main;
				var worldCamera = Camera.main;
				var canvasRect = canvas.GetComponent<RectTransform>();

				var screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, target.position);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out pos);
				rectTransform.localPosition = pos;
            }
		}
	}
}