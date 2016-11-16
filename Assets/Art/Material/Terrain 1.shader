Shader "Custom/Terrain Splat" {
    Properties {
        _MainTex ("Splat 1 (RGB)", 2D) = "white" {}
        _Color ("Splat 1", Color) = (0.5, 0.5, 0.5, 1)
        _Splat2 ("Splat 2 Texture (RGB)", 2D) = "white" {} 
        _Color2 ("Splat 2 Color", Color) = (1, 1, 1, 1)            
        _Splat3 ("Splat 3 Texture (RGB)", 2D) = "white" {} 
        _Color3 ("Splat 3 Color", Color) = (1, 1, 1, 1)            
        _SplatMap ("Splatmap (RGB)", 2D) = "white" {}   
    }
  
    SubShader
    {
        Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }
        LOD 200
        Cull Back
        Fog {Mode Off}
        ZWrite On        

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        sampler2D _Splat2;
        sampler2D _Splat3;
        sampler2D _SplatMap;
        fixed3 _Color;
        fixed3 _Color2;
        fixed3 _Color3;

        struct Input {   
            float2 uv_MainTex;
            float2 uv_Splat2;
            float2 uv_Splat3;
            float2 uv_SplatMap;
        };

        void vert (inout appdata_full v, out Input data) {
            UNITY_INITIALIZE_OUTPUT(Input,data);
            half pos = length(mul (UNITY_MATRIX_MV, v.vertex).xyz);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            fixed3 color = _Color;

            fixed4 splat = tex2D (_SplatMap, IN.uv_SplatMap);
            float3 mixedDiffuse = 0.0;
            mixedDiffuse += splat.r * tex2D(_MainTex, IN.uv_MainTex) * _Color;
            mixedDiffuse += splat.g * tex2D(_Splat2, IN.uv_Splat2) * _Color2;
            mixedDiffuse += splat.b * tex2D(_Splat3, IN.uv_Splat3) * _Color3;

            o.Albedo = mixedDiffuse;  // + (vignette + fixed4(0, 0.0013, 0.0026, 0)); 
        }
        ENDCG
    }
    FallBack "Diffuse"
}