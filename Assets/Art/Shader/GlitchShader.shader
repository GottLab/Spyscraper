Shader "Custom/GlitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TimeMultiplier ("Time Multiplier", Float) = 2.0
        _Noise ("Noise", Range(0,1)) = 0.0
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "UI"
            Tags { "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _TimeMultiplier;
            float _Noise;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float2 screenUV : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 permute(float3 x) { return mod289(((x * 34.0) + 1.0) * x); }

            float snoise(float2 v)
            {
                const float4 C = float4(0.211324865405187, 0.366025403784439,
                                        -0.577350269189626, 0.024390243902439);
                float2 i = floor(v + dot(v, C.yy));
                float2 x0 = v - i + dot(i, C.xx);

                float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;

                i = mod289(i);
                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) +
                                   i.x + float3(0.0, i1.x, 1.0));

                float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
                m = m * m;
                m = m * m;

                float3 x = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;

                m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);

                float3 g;
                g.x = a0.x * x0.x + h.x * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;

                return 130.0 * dot(m, g);
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                o.screenUV = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _TimeMultiplier;

                float noise = max(0.0, snoise(float2(time, uv.y * 0.3)) - 0.3) * 0.5;
                noise += (snoise(float2(time * 10.0, uv.y * 2.4)) - 0.5) * 0.15;

                float xpos = uv.x - noise * noise * 0.25;

                fixed4 col = tex2D(_MainTex, float2(xpos, uv.y)) * i.color;

                float rnd = rand(float2(uv.y * time, 0.0));
                col.rgb = lerp(col.rgb, float3(rnd, rnd, rnd), noise * 0.3);

                if (fmod(floor(i.screenUV.y * 0.25), 2.0) == 0.0)
                {
                    col.rgb *= 1.0 - (0.15 * noise);
                }

                float g = tex2D(_MainTex, float2(xpos + noise * 0.05, uv.y)).g;
                float b = tex2D(_MainTex, float2(xpos - noise * 0.05, uv.y)).b;

                col.g = lerp(col.r, g, 0.75);
                col.b = lerp(col.r, b, 0.75);

                //return col;
                float randomTvStatic = rand(float2(uv.x, uv.y * time));
                fixed4 randomTvStaticColor = fixed4(randomTvStatic, randomTvStatic, randomTvStatic, 1);
                col.rgb += randomTvStaticColor.rgb * _Noise;
                //col.a = 1.0;
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
