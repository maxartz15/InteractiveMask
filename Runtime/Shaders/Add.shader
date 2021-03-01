Shader "TAO/InteractiveMask/Add"
{
	Properties
	{
		[HideInInspector] [NoScaleOffset] _MainTex("main", 2D) = "white" {}
		_Channels("Channels", Vector) = (1, 1, 1, 1)
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
			float4 _Channels;

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
				half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Channels;
				return color;
			}

			ENDHLSL
		}
	}
}