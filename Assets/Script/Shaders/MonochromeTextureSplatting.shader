// Created by Matthew F Keating with the help of Catlike Coding Tutorials.
//  --- http://catlikecoding.com/unity/tutorials/rendering/part-3 ---

Shader "Custom/Monochrome Texture Splatting"
{
    Properties
    {
        _MainTex ("Splat Map", 2D) = "white" {}
        [NoScaleOffset] _Texture1 ("Black Texture", 2D) = "white" {}
        [NoScaleOffset] _Texture2 ("White Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Texture1, _Texture2;

            struct VertexData
            {
                 float4 position : POSITION;
                 float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvSplat : TEXCOORD1;
            };

          

            Interpolators MyVertexProgram(VertexData vertexData)
            {
                Interpolators interpolators;

                interpolators.position = mul(UNITY_MATRIX_MVP, vertexData.position);
                interpolators.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);

                return interpolators;
            }

            float4 MyFragmentProgram(Interpolators interpolators) : SV_TARGET
            {
                float4 splat = tex2D(_MainTex, interpolators.uvSplat);

                return tex2D(_Texture1, interpolators.uv) * splat.r + tex2D(_Texture2, interpolators.uv * (1 - splat.r));
            }

            ENDCG
        }
    }
}