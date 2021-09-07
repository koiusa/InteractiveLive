using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MousePointer : MonoBehaviour
{
    // 位置座標
    private Vector3 position;
    // スクリーン座標をワールド座標に変換した位置座標
    private Vector3 screenToWorldPointPosition;
    // Z軸修正
    private float distance = 10f;

    public Transform Target;
    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            // Vector3でマウス位置座標を取得する
            var screenPos = Mouse.current.position.ReadValue();
            position = new Vector3(screenPos.x, screenPos.y, distance);
            // マウス位置座標をスクリーン座標からワールド座標に変換する
            screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);

            // ワールド座標に変換されたマウス座標を代入
            Target.position = screenToWorldPointPosition;
        }
    }
}
