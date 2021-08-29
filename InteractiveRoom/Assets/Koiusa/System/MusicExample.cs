using DG.Tweening;
using UnityEngine;

public class MusicExample : MonoBehaviour
{
    private void Update()
    {
        // ���j�b�g���ς�����t���[���� true �ɂȂ�
        if (Music.IsJustChanged)
        {
            print("IsJustChanged");

        }
        // ���߂ɗ����t���[���� true �ɂȂ�
        if (Music.IsJustChangedBar())
        {
            print("IsJustChangedBar");
            DOTween
                .To(value => OnRotate(value), 0, 1, 0.5f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => transform.localEulerAngles = new Vector3(45, 45, 0));
        }
        // ���ɗ����t���[���� true �ɂȂ�
        if (Music.IsJustChangedBeat())
        {
            print("IsJustChangedBeat");
            DOTween
                .To(value => OnScale(value), 0, 1, 0.1f)
                .SetEase(Ease.InQuad)
                .SetLoops(2, LoopType.Yoyo)
;
        }
        // �w�肵�����߁A���A���j�b�g�ɗ����t���[���� true �ɂȂ�
        if (Music.IsJustChangedAt(1, 2, 3))
        {
            print("IsJustChangedAt");
        }
    }

    private void OnScale(float value)
    {
        var scale = Mathf.Lerp(1, 1.2f, value);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnRotate(float value)
    {
        var rot = transform.localEulerAngles;
        rot.z = Mathf.Lerp(0, 360, value);
        transform.localEulerAngles = rot;
    }
}