Shader "Custom/PlanarStandardShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SmoothnessTexture ("SmoothnessTexture", 2D) = "White" {}
        _Smoothness ("Smoothness", Range(0,1)) = 1.0
        _Scale ("Scale", Float) = 1.0
        _NormalTexture ("NormalTexture", 2D) = "Blue" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SmoothnessTexture;
        sampler2D _NormalTexture;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            //INTERNAL_DATA 
        };

        half _Smoothness;
        fixed4 _Color;
        float _Scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
           
            
            float3 worldPos = IN.worldPos * _Scale;
            float3 n = normalize(cross(ddy(IN.worldPos), ddx(IN.worldPos)));

            float3 absN = abs(n);

            float2 uv;

            // Choose projection based on dominant normal axis
            if (absN.x > absN.y && absN.x > absN.z)
                uv = worldPos.zy; // X face -> project on YZ plane
            else if (absN.y > absN.z)
                uv = worldPos.xz; // Y face -> project on XZ plane
            else
                uv = worldPos.xy; // Z face -> project on XY plane

            fixed3 normal = UnpackNormal(tex2D(_NormalTexture, uv));



            fixed4 c = tex2D (_MainTex, uv) * _Color;
            o.Albedo = c;
            
            o.Normal = normal;
            o.Metallic = 0.0;
            o.Smoothness = (tex2D (_SmoothnessTexture, uv).r) * _Smoothness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
