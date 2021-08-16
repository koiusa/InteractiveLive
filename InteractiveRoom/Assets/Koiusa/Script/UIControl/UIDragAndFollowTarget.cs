//  UIDragAndFollowTarget.cs
//  https://tsubakit1.hateblo.jp/entry/2016/03/01/020510

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Koiusa.InteractiveRoom
{
	public class UIDragAndFollowTarget : UIFollowTarget, IDragHandler
	{
		void IDragHandler.OnDrag(PointerEventData ev)
		{
			var ray = RectTransformUtility.ScreenPointToRay(Camera.main, ev.position);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				target.position = hit.point + hit.normal * 0.5f;
			}
		}
	}
}