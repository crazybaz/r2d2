Shader "Custom/OpaqueTransparentShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" "IgnoreProjector"="True"}
        Pass {
            Tags {"LightMode"="ForwardBase"}

            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaToMask On

            CGPROGRAM

            // compilation directives for this snippet, e.g.:
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            // the Cg/HLSL code itself

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                UNITY_FOG_COORDS(2)
                fixed3 diff : COLOR0; // diffuse lighting color
                fixed3 ambient : COLOR1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Cutoff;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0.rgb;

                o.ambient = ShadeSH9(half4(worldNormal,1));

                // compute shadows data
                TRANSFER_SHADOW(o)

                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 lighting = i.diff * shadow + i.ambient;

                // multiply by lighting
                col.rgb *= lighting;

                clip(col.a - _Cutoff);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }

            ENDCG
        }

        Pass {
            Name "Caster"
            Tags { "LightMode" = "ShadowCaster" }

            // Not implementet yet...
            // Offset 1, 1
            // Fog {Mode Off}
            // ZWrite On ZTest LEqual Cull Off

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

                uniform float4 _MainTex_ST;

                v2f vert( appdata_base v ) {
                    v2f o;
                    TRANSFER_SHADOW_CASTER(o)
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                uniform sampler2D _MainTex;
                uniform fixed _Cutoff;

                float4 frag( v2f i ) : COLOR {
                    fixed alpha = tex2D( _MainTex, i.uv ).a;

                    // or if you want to use grayscale texture:
                    // fixed alpha = tex2D( _MainTex, i.uv ).r;

                    clip(alpha - _Cutoff);

                    SHADOW_CASTER_FRAGMENT(i)
                }
            ENDCG
        }

        // shadow casting support f.e. // UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    Fallback "Transparent/Cutout/Diffuse"
}


/*Shader "Custom/OpaqueTransparentShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags {"Queue"="Geometry+1" "IgnoreProjector"="True" "RenderType"="Opaque" "ForceNoShadowCasting"="True"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert decal:blend exclude_path:deferred exclude_path:prepass novertexlights nolightmap

        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            if (c.a > _Cutoff)
                o.Alpha = c.a;
            else
                o.Alpha = 0;
        }

        ENDCG
    }
}*/