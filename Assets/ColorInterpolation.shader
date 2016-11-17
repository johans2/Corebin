// Bases on Unity default shader Mobile-Diffuse.
// Interpolates between two colors.
// Unlit (only Emission, no Albedo).


Shader "Resolution/ColorInterpolation-Unlit" {
Properties {
	_ColorA ("ColorA", Color) = (1,1,1,1)
	_ColorB ("ColorB", Color) = (1,1,1,1)
	_LerpValue("LerpValue", range(0,1)) = .0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100

CGPROGRAM
#pragma surface surf Lambert noambient

fixed4 _ColorA;
fixed4 _ColorB;
fixed _LerpValue;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	o.Emission = lerp(_ColorA, _ColorB, _LerpValue);
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
