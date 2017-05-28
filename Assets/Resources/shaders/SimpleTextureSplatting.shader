﻿
Shader "Custom/SimpleTextureSplatting" {

    Properties {
        [NoScaleOffset] _MaskTex ("Mask 1", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Texture1 ("Texture 1 (black)", 2D) = "white" {}
        _Texture2 ("Texture 1 (red)", 2D) = "white" {}
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
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            float4 _Texture1_ST, _Texture2_ST;

            sampler2D _Texture1, _Texture2;

            struct Interpolators {
                fixed3 diff : COLOR0; // diffuse lighting color
                fixed3 ambient : COLOR1;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1

                float4 position : SV_POSITION;

                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                float2 uvSplat : TEXCOORD3;
            };

            Interpolators MyVertexProgram (appdata_base v) {
                Interpolators i;
                i.position = mul(UNITY_MATRIX_MVP, v.vertex);

                i.uv1 = TRANSFORM_TEX(v.texcoord, _Texture1);
                i.uv2 = TRANSFORM_TEX(v.texcoord, _Texture2);
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
                    tex2D(_Texture1, i.uv1) * (1 - mask.r - mask.g - mask.b) * _Color +
                    tex2D(_Texture2, i.uv2) * mask.r;

                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 lighting = i.diff * shadow + i.ambient;

                // multiply by lighting
                col.rgb *= lighting;

                return col;
            }

            ENDCG
        }

        Pass {
            Name "Caster"
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_shadowcaster
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct v2f {
                    V2F_SHADOW_CASTER;
                    float2  uv : TEXCOORD1;
                };

                uniform float4 _Texture1_ST;

                v2f vert( appdata_base v ) {
                    v2f o;
                    TRANSFER_SHADOW_CASTER(o)
                    o.uv = TRANSFORM_TEX(v.texcoord, _Texture1);
                    return o;
                }

                uniform sampler2D _Texture1;

                float4 frag( v2f i ) : COLOR {
                    SHADOW_CASTER_FRAGMENT(i)
                }
            ENDCG
        }
    }
}

