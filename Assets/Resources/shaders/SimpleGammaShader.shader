Shader "Custom/SimpleGammaShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GammaCorrect("Gamma Correct", Range(0.0, 10.0)) = 1.0
        _LightCorrect("Light Correct", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows exclude_path:deferred exclude_path:prepass novertexlights nolightmap

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float _GammaCorrect;
        float _LightCorrect;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //texture with gamma correction
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            c *= _LightCorrect;
            c = pow(c, 1.0 / _GammaCorrect);

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
