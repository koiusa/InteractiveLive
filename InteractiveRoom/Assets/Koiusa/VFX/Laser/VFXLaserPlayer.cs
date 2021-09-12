using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// ��莞�Ԃ��Ƃ�Laser������MonoBehaviour�N���X
/// </summary>
public class VFXLaserPlayer : MonoBehaviour
{
    [SerializeField] private VisualEffect _visualEffect; // ���[�U�[VisualEffect
    [SerializeField] private Transform laserTarget; // ���[�U�[���_����������Ώ�
    [SerializeField] private float interval = 4f; // ���[�U�[�����Ԋu

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            _visualEffect.Reinit();
            _visualEffect.Play();
        }
    }

    void Update()
    {
        // Ray���̍쐬
        var origin = _visualEffect.GetVector3("LaserSrcPosition"); // ���[�U�[�n�_
        var direction = (laserTarget.position - origin).normalized; // ���[�U�[�����x�N�g��
        var ray = new Ray(origin, direction);

        // Raycast���s
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ray�����������ʒu�����[�U�[�I�_�ɐݒ�
            _visualEffect.SetVector3("LaserDstPosition", hit.point);

            // �@�����̐ݒ�
          //  _visualEffect.SetVector3("Collision Normal", hit.normal);
        }
        else
        {
            // ���[�U�[�I�_�̐ݒ�
            _visualEffect.SetVector3("LaserDstPosition", laserTarget.position);

            // �@�����̐ݒ�
           // _visualEffect.SetVector3("Collision Normal", new Vector3(0f, 1f, 0f));
        }
    }
}