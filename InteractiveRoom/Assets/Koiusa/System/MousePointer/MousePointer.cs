using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MousePointer : MonoBehaviour
{
    // �ʒu���W
    private Vector3 position;
    // �X�N���[�����W�����[���h���W�ɕϊ������ʒu���W
    private Vector3 screenToWorldPointPosition;
    // Z���C��
    private float distance = 10f;

    public Transform Target;
    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            // Vector3�Ń}�E�X�ʒu���W���擾����
            var screenPos = Mouse.current.position.ReadValue();
            position = new Vector3(screenPos.x, screenPos.y, distance);
            // �}�E�X�ʒu���W���X�N���[�����W���烏�[���h���W�ɕϊ�����
            screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);

            // ���[���h���W�ɕϊ����ꂽ�}�E�X���W����
            Target.position = screenToWorldPointPosition;
        }
    }
}
