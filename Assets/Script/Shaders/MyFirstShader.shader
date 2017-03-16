Shader "Custom/My First Shader"
{
    Properties
    {
        _Tint ("Tint Colour", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct Interpolators
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                //float3 localPosition : TEXCOORD0;
            };

            struct VertexData
            {
                 float4 position : POSITION;
                 float2 uv : TEXCOORD0;
            };

            Interpolators MyVertexProgram(VertexData vertexData)
            {
                Interpolators interpolators;

                //interpolators.localPosition = position.xyz;
                interpolators.position = mul(UNITY_MATRIX_MVP, vertexData.position);
                interpolators.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);

                return interpolators;
            }

            float4 MyFragmentProgram(Interpolators interpolators) : SV_TARGET
            {
                return tex2D(_MainTex, interpolators.uv) * _Tint;
            }

            ENDCG
        }
    }
}