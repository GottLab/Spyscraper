Shader "UI/CustomSpriteShader"
{
    Properties
    {
        _Color("Tint", Color) = (1,1,1,1)
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Glitch("Glitch", float) = 0.0 
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float _Glitch;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            float4 Noise(float2 uv, float time, float e)
            {
                float G = e + (time * 0.1);
                float2 r = G * sin(G * uv);
                return float4(frac(r.x * r.y * (1.0 + uv.x)), 0, 0, 1);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.z * 1;
                float4 noiseColor = Noise(i.uv, time, 2.71);
                float4 texColor = tex2D(_MainTex, i.uv) * _Color;
                return lerp(texColor, noiseColor, _Glitch);
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
