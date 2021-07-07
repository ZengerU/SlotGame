// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteBlender"
{
    Properties
    {
        [HideInInspector]_MainTex("Ignore This", 2D) = "" {} //This is put here because spriterenderer requires it.
        _FirstTex("Sprite Texture", 2D) = "" {}
        _SecTex("Effect Texture", 2D) = ""{}
        _MixRange("Mixing Power", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha //Premultiplied transparency
        //Blend SrcAlpha OneMinusSrcAlpha // Traditional  transparency

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoordMain : TEXCOORD0;
                float2 texcoordSec : TEXCOORD1;
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoordMain : TEXCOORD0;
                float2 texcoordSec : TEXCOORD1;
            };

            //fixed4 _Color;  //i will use only default color(from spriteRenderer)
            fixed _MixRange;

            VertexOutput vert(VertexInput IN)
            {
                VertexOutput OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoordMain = IN.texcoordMain;
                OUT.texcoordSec = IN.texcoordSec;
                OUT.color = IN.color /* * _Color*/;
                return OUT;
            }

            sampler2D _SecTex;
            sampler2D _FirstTex;
            fixed4 SampleSpriteTexture(sampler2D tex, float2 uv)
            {
                fixed4 color = tex2D(tex, uv);
                return color;
            }

            fixed4 frag(VertexOutput IN) : SV_Target
            {
                fixed4 t1 = SampleSpriteTexture(_FirstTex, IN.texcoordMain) * IN.color;
                t1.rgb *= t1.a; //w/o this the texture will have transparency with white background
                fixed4 t2 = SampleSpriteTexture(_SecTex, IN.texcoordSec) * IN.color;
                t2.rgb *= t2.a;
                fixed4 res = lerp(t1, t2, _MixRange);
                return res;
            }
            ENDCG
        }
    }
}