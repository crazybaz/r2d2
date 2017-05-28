
Shader "Custom/TextureSplatting" {

    Properties {
        [NoScaleOffset] _MaskTex ("Mask 1", 2D) = "white" {}
        _Texture1 ("Texture 1 (black)", 2D) = "white" {}
        _Texture2 ("Texture 2 (red)", 2D) = "white" {}
        _Texture3 ("Texture 3 (green)", 2D) = "white" {}
        _Texture4 ("Texture 4 (blue)", 2D) = "white" {}
    }

    SubShader {
        //Tags { "Queue"="Geometry" "RenderType"="Opaque" "IgnoreProjector"="True"}
        Pass {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0
            #include "AutoLight.cginc"

            fixed4 _Color;
            sampler2D _MaskTex; //, _MainTex2, _MainTex3, _MainTex4;
            float4 _MaskTex_ST; //, _MainTex2_ST, _MainTex3_ST, _MainTex4_ST;
            float4 _Texture1_ST, _Texture2_ST, _Texture3_ST, _Texture4_ST;

            sampler2D _Texture1, _Texture2, _Texture3, _Texture4;

            struct Interpolators {
                fixed3 diff : COLOR0; // diffuse lighting color
                fixed3 ambient : COLOR1;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1

                float4 position : SV_POSITION;

                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD4;
                float2 uv3 : TEXCOORD5;
                float2 uv4 : TEXCOORD6;
                float2 uvSplat : TEXCOORD7;
            };

            Interpolators MyVertexProgram (appdata_base v) {
                Interpolators i;
                i.position = mul(UNITY_MATRIX_MVP, v.vertex);

                i.uv1 = TRANSFORM_TEX(v.texcoord, _Texture1);
                i.uv2 = TRANSFORM_TEX(v.texcoord, _Texture2);
                i.uv3 = TRANSFORM_TEX(v.texcoord, _Texture3);
                i.uv4 = TRANSFORM_TEX(v.texcoord, _Texture4);
                i.uvSplat = v.texcoord;

                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                i.diff = nl * _LightColor0.rgb;

                i.ambient = ShadeSH9(half4(worldNormal,1));

                // compute shadows data
                TRANSFER_SHADOW(i)

                return i;
            }

            float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
                float4 mask = tex2D(_MaskTex, i.uvSplat);

                fixed4 col =
                    tex2D(_Texture1, i.uv1) * (1 - mask.r - mask.g - mask.b) +
                    tex2D(_Texture2, i.uv2) * mask.r +
                    tex2D(_Texture3, i.uv3) * mask.g +
                    tex2D(_Texture4, i.uv4) * mask.b;

                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 lighting = i.diff * shadow + i.ambient;

                // multiply by lighting
                col.rgb *= lighting;

                return col;
            }

            ENDCG
        }
    }
}

