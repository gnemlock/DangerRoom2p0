// Created by Matthew F Keating with the help of Catlike Coding Tutorials.
//  --- http://catlikecoding.com/unity/tutorials/rendering/part-3 ---

"Custom/Textured With Detail"
{
    Properties
    {
        _Tint ("Tint Colour", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _DetailTex ("Detail Texture", 2D) = "gray" {}
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
            sampler2D _MainTex, _DetailTex;
            float4 _MainTex_ST, _DetailTex_ST;

            struct Interpolators
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvDetail : TEXCOORD1;
            };

            struct VertexData
            {
                 float4 position : POSITION;
                 float2 uv : TEXCOORD0;
            };

            Interpolators MyVertexProgram(VertexData vertexData)
            {
                Interpolators interpolators;

                interpolators.position = mul(UNITY_MATRIX_MVP, vertexData.position);
                interpolators.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);
                interpolators.uvDetail = TRANSFORM_TEX(vertexData.uv, _DetailTex);

                return interpolators;
            }

            float4 MyFragmentProgram(Interpolators interpolators) : SV_TARGET
            {
                float4 colour = tex2D(_MainTex, interpolators.uv) * _Tint;
                colour *= tex2D(_MainTex, interpolators.uvDetail) * unity_ColorSpaceDouble;
                return colour;
            }

            ENDCG
        }
    }
}