using UnityEngine;
using UnityEngine.VFX;


public class MeshParticleUnit : MonoBehaviour
{
    public static class PropName
    {
        public const string PositionMap = "PositionMap";
        //public const string UVMap = "UVMap";
        //public const string ModelMainTex = "ModelMainTex";
        public const string NormalMap = "NormalMap";
        public const string VtxCount = "VtxCount";
        public const string Mesh = "Mesh";
    }

    public VisualEffect effect;
    public MapSet mapSet;
    //public Texture modelMainTex;


    void Update()
    {
        //effect.SetTexture(PropName.PositionMap, mapSet.position);
        //effect.SetTexture(PropName.UVMap, mapSet.uv);
        //effect.SetTexture(PropName.ModelMainTex, modelMainTex);
        effect.SetTexture(PropName.NormalMap, mapSet.normal);
        effect.SetInt(PropName.VtxCount, mapSet.vtxCount);
        effect.SetMesh(PropName.Mesh, mapSet.mesh);
    }
}