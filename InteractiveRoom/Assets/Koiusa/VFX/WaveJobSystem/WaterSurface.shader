Shader "Water/Surface"
{

	Properties
	{
		_Color("Color", color) = (1, 1, 1, 0)
		_DispTex("Disp Texture", 2D) = "gray" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MinDist("Min Distance", Range(0.1, 50)) = 10
		_MaxDist("Max Distance", Range(0.1, 50)) = 25
		_TessFactor("Tessellation", Range(1, 50)) = 10
		_Displacement("Displacement", Range(0, 1.0)) = 0.3
	}

	SubShader
	{

		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
		Pass {
			Tags{"LightMode" = "UniversalForward"}
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "./CustomTessellation.hlsl"

			float _TessFactor;
			float _Displacement;
			float _MinDist;
			float _MaxDist;
			sampler2D _DispTex;
			float4 _DispTex_TexelSize;
			half4 _Color;
			half _Glossiness;
			half _Metallic;

			Varyings vert(Attributes v)
			{
				float d = tex2Dlod(_DispTex, float4(v.uv.xy, 0, 0)).r * _Displacement;
				v.vertex.xyz += v.normal * d;

				Varyings output;
				v.vertex.xyz += (v.normal) * _Displacement;
				output.vertex = TransformObjectToHClip(v.vertex.xyz);
				output.color = v.color;
				output.normal = v.normal;
				output.uv = v.uv;

				output.color.rgb = _Color.rgb;
				//output.Metallic = _Metallic;
				//output.Smoothness = _Glossiness;
				output.color.a = _Color.a * (0.5 + 0.5 * clamp(tex2Dlod(_DispTex, float4(v.uv,0,0)).r, 0, 1));

				float3 duv = float3(_DispTex_TexelSize.xy, 0);
				half v1 = tex2Dlod(_DispTex, float4(v.uv - duv.xz,0,0)).y;
				half v2 = tex2Dlod(_DispTex, float4(v.uv + duv.xz, 0, 0)).y;
				half v3 = tex2Dlod(_DispTex, float4(v.uv - duv.zy, 0, 0)).y;
				half v4 = tex2Dlod(_DispTex, float4(v.uv + duv.zy, 0, 0)).y;
				output.normal = normalize(float3(v1 - v2, v3 - v4, 0.3));

				return output;
			}


			[UNITY_domain("tri")]
			Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
			{
				Attributes v;

	#define DomainPos(fieldName) v.fieldName = \
					patch[0].fieldName * barycentricCoordinates.x + \
					patch[1].fieldName * barycentricCoordinates.y + \
					patch[2].fieldName * barycentricCoordinates.z;

				DomainPos(vertex)
					DomainPos(uv)
					DomainPos(color)
					DomainPos(normal)

					return vert(v);
			}

			half4 frag(Varyings IN) : SV_Target
			{
				half4 tex = tex2D(_DispTex, IN.uv);
				tex.a = _Color.a * (0.5 + 0.5 * clamp(tex2Dlod(_DispTex, float4(IN.uv, 0, 0)).r, 0, 1));
				return tex;
			}

			ENDHLSL

		}
	}

	FallBack "Diffuse"

}