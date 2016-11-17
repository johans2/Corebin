// 
// Diffuse shader with crossfading of emission texture
//

Shader "Slowpoke/CrossFadeEmissionMat"{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_SecondTex("SecondTex", 2D) = "white" {}
		_Progress("Progress", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM
#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;
		sampler2D _SecondTex;
		float _Progress;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		float4 tex = tex2D(_MainTex, IN.uv_MainTex);
		float4 emissionTex = tex2D(_SecondTex, IN.uv_MainTex);
		float4 emissionColor = emissionTex * _Progress;
		o.Albedo = tex.rgb;
		o.Emission.rgb = emissionColor;
		o.Alpha = 1;
	}
	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
