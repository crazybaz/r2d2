Shader "Custom/ColorMaskShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (RGB)", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _WhiteCorrect("White Correct", Range(0.0, 1.0)) = 0.0
        _RedCrop("Red Crop", Range(0.0, 1.0)) = 0.0
        _BlueCrop("Blue Crop", Range(0.0, 1.0)) = 0.0
        _GreenCrop("Green Crop", Range(0.0, 1.0)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert novertexlights nolightmap

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;
        fixed4 _Color;
        float _WhiteCorrect;
        float _RedCrop;
        float _BlueCrop;
        float _GreenCrop;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 texTwoCol = tex2D(_MaskTex, IN.uv_MainTex) * _Color;

            if (texTwoCol.r <= _RedCrop && texTwoCol.g <= _BlueCrop && texTwoCol.b <= _GreenCrop)
            {
                o.Albedo = mainCol.rgb;
            }
            else
            {
                o.Albedo = mainCol.rgb * texTwoCol.rgb + _WhiteCorrect;
            }

            o.Alpha = mainCol.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
