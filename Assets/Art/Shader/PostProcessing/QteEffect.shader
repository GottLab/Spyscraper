Shader "Custom/QteEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Amount ("Amount", Range(0, 1)) = 1.0
        _VignetteRadius ("Vignette Radius", Range(0, 1)) = 0.6
        _VignetteSoftness ("Vignette Softness", Range(0.001, 1)) = 0.2
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.8
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Amount;
            float _VignetteRadius;
            float _VignetteSoftness;
            float _VignetteIntensity;

            fixed4 frag(v2f_img i) : SV_Target
            {

                fixed4 color = tex2D(_MainTex, i.uv);
                float gray = dot(color, float3(0.299, 0.587, 0.114)); // Standard luminance

                float3 finalColor = lerp(color.rgb, gray, _Amount);

                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float vignette = smoothstep(_VignetteRadius, _VignetteRadius - _VignetteSoftness, dist);
                finalColor *= lerp(1.0, 1.0 - _VignetteIntensity,( 1.0f - vignette) * _Amount);

                return fixed4(finalColor.r, finalColor.g, finalColor.b, color.a);
            }
            ENDCG
        }
    }
}
