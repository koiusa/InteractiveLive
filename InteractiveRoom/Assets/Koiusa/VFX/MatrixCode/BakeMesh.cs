using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class BakeMesh : MonoBehaviour
{
    public Texture2D BakedTexture { get; private set; }

    private VisualEffect _vfx = null;
    private Mesh _mesh = null;

    private Color[] _colorBuffer = null;

    private void Awake()
    {
        Initialize();
        // �v���p�e�B�uPositionMap�v �ɒ��_�̃|�W�V������񂪕ۑ����ꂽ Texture2D ���Z�b�g
        _vfx = GetComponent<VisualEffect>();
        _vfx.SetTexture($"PositionMap", BakedTexture);
    }

    public void Initialize()
    {
        // �A�^�b�`���ꂽ�I�u�W�F�N�g�� MeshFilter ���烁�b�V���̏����擾
        _mesh = GetComponent<MeshFilter>().mesh;

        // ���_�̃|�W�V���������擾
        Vector3[] vertices = _mesh.vertices;

        // ���_��
        int count = vertices.Length;

        // Texture2D �� h �~ w �Ƀ|�W�V�������𗎂Ƃ����ނ��߁A���_���̕��������擾
        float r = Mathf.Sqrt(count);

        // w�Ah �𐮐������邽�߁A���_���̕�������؂�̂�
        // �؂�グ�̏ꍇ�A w �~ h �����_�����傫���Ȃ�ATexture2D �� (0,0,0, 0) �̏�񂪗]��̂Œ���
        int size = (int)Mathf.Floor(r);

        // Texture2D �ɕۑ����邽�߂̃o�b�t�@
        _colorBuffer = new Color[size * size];

        // Texture2D�iAttributeMap�j�̃Z�b�g�A�b�v
        BakedTexture = new Texture2D(size, size, TextureFormat.RGBAFloat, false);
        BakedTexture.filterMode = FilterMode.Point;
        BakedTexture.wrapMode = TextureWrapMode.Clamp;

        // Texture2D �ɒ��_�̃|�W�V��������ۑ�
        UpdatePositionMap();
    }

    private void UpdatePositionMap()
    {
        // for ���� foreach �̕������������|�I�ɑ������߁Aforeach ���̗p
        // _mesh.vertices �̕��� Texture2D �̃T�C�Y�iw �~ h�j���傫�����߁A_colorBuffer.Length �܂ŌJ��Ԃ��悤�ɒ���
        int idx = 0;
        foreach (Vector3 vert in _mesh.vertices.Take(_colorBuffer.Length))
        {
            _colorBuffer[idx] = VectorToColor(vert);
            idx++;
        }

        // Texture2D �ɒ��_�̃|�W�V�������� Texture2D �ɃZ�b�g
        BakedTexture.SetPixels(_colorBuffer);
        BakedTexture.Apply();
    }

    private Color VectorToColor(Vector3 v)
    {
        return new Color(v.x, v.y, v.z, 0.0f);
    }

}