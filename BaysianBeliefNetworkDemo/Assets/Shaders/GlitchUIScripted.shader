Shader "UI/GlitchSpreadRGB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0,1)) = 0.5
        _GlitchOffset ("Glitch Offset", Range(0,20)) = 5
        _Spread ("Spread", Range(0,1)) = 0.0
        _RGBSeparation ("RGB Separation", Range(0,10)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GlitchIntensity;
            float _GlitchOffset;
            float _Spread;
            float _RGBSeparation;
            // _Time is built-in by Unity, so we don't redeclare it.

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // --- Glitch Effect ---
                float segment = floor(uv.y * 100.0);
                float glitchChance = rand(float2(segment, _Time.y));
                float glitch = step(1.0 - _GlitchIntensity, glitchChance);
                float offset = glitch * _GlitchOffset * (rand(uv + _Time.y) - 0.5);
                uv.x += offset / _MainTex_ST.x;
                
                // --- Revised Spread (Stretch) Effect ---
                float2 center = float2(0.5, 0.5);
                // Push UVs outward from the center:
                uv += (uv - center) * _Spread;
                // Fade out as it spreads:
                float fade = 1.0 - _Spread;
                
                // --- RGB Separation ---
                float2 uvR = uv + float2(_RGBSeparation / _MainTex_ST.x, 0);
                float2 uvG = uv;
                float2 uvB = uv - float2(_RGBSeparation / _MainTex_ST.x, 0);
                
                fixed4 colR = tex2D(_MainTex, uvR);
                fixed4 colG = tex2D(_MainTex, uvG);
                fixed4 colB = tex2D(_MainTex, uvB);
                
                fixed4 col;
                col.r = colR.r;
                col.g = colG.g;
                col.b = colB.b;
                col.a = (colR.a + colG.a + colB.a) / 3.0;
                
                col.a *= fade;
                
                return col;
            }
            ENDCG
        }
    }
}
