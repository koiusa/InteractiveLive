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
        // プロパティ「PositionMap」 に頂点のポジション情報が保存された Texture2D をセット
        _vfx = GetComponent<VisualEffect>();
        _vfx.SetTexture($"PositionMap", BakedTexture);
    }

    public void Initialize()
    {
        // アタッチされたオブジェクトの MeshFilter からメッシュの情報を取得
        _mesh = GetComponent<MeshFilter>().mesh;

        // 頂点のポジション情報を取得
        Vector3[] vertices = _mesh.vertices;

        // 頂点数
        int count = vertices.Length;

        // Texture2D で h × w にポジション情報を落とし込むため、頂点数の平方根を取得
        float r = Mathf.Sqrt(count);

        // w、h を整数化するため、頂点数の平方根を切り捨て
        // 切り上げの場合、 w × h が頂点数より大きくなり、Texture2D に (0,0,0, 0) の情報が余るので注意
        int size = (int)Mathf.Floor(r);

        // Texture2D に保存するためのバッファ
        _colorBuffer = new Color[size * size];

        // Texture2D（AttributeMap）のセットアップ
        BakedTexture = new Texture2D(size, size, TextureFormat.RGBAFloat, false);
        BakedTexture.filterMode = FilterMode.Point;
        BakedTexture.wrapMode = TextureWrapMode.Clamp;

        // Texture2D に頂点のポジション情報を保存
        UpdatePositionMap();
    }

    private void UpdatePositionMap()
    {
        // for よりも foreach の方が処理が圧倒的に速いため、foreach を採用
        // _mesh.vertices の方が Texture2D のサイズ（w × h）より大きいため、_colorBuffer.Length まで繰り返すように調整
        int idx = 0;
        foreach (Vector3 vert in _mesh.vertices.Take(_colorBuffer.Length))
        {
            _colorBuffer[idx] = VectorToColor(vert);
            idx++;
        }

        // Texture2D に頂点のポジション情報を Texture2D にセット
        BakedTexture.SetPixels(_colorBuffer);
        BakedTexture.Apply();
    }

    private Color VectorToColor(Vector3 v)
    {
        return new Color(v.x, v.y, v.z, 0.0f);
    }

}