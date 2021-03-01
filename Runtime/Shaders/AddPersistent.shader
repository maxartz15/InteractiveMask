Shader "TAO/InteractiveMask/AddPersistent"
{
	Properties
	{
		[HideInInspector] [NoScaleOffset] _MainTex("main", 2D) = "white" {}
		[NoScaleOffset] _Persistent("persistent", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		Blend One One

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float2 uv           : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionHCS  : SV_POSITION;
				float2 uv           : TEXCOORD0;
			};

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);			
			TEXTURE2D(_Persistent);
			SAMPLER(sampler_Persistent);

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
			CBUFFER_END

			Varyings vert(Attributes IN)
			{
				Varyings OUT;

				OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

				return OUT;
			}
	          
			half4 frag(Varyings IN) : SV_Target
			{
				half4 color = SAMPLE_TEXTURE2D(_Persistent, sampler_Persistent, IN.uv) + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
				return color;
			}

			ENDHLSL
		}
	}
}