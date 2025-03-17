Shader "Custom/GlitchTMP_Noise"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _GlitchIntensity ("Glitch Intensity", Range(0,1)) = 0
        _GlitchOffset ("Glitch Offset", Range(0,1)) = 0
        _Spread ("Spread", Range(0,1)) = 0
        _RGBSeparation ("RGB Separation", Range(0,1)) = 0
        _Smoothing ("Smoothing", Range(0.001, 0.1)) = 0.1
        _GlitchCycle ("Glitch Cycle Duration", Float) = 5.0
        _GlitchBurst ("Glitch Burst Duration", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _FaceColor;
            float _GlitchIntensity;
            float _GlitchOffset;
            float _Spread;
            float _RGBSeparation;
            float _Smoothing;
            float _GlitchCycle;
            float _GlitchBurst;

            // Simple 2D noise function returning a value in [0,1].
            float Noise(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898,78.233))) * 43758.5453);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;

                // Determine glitch active state:
                // Glitch is active if the current cycle phase is less than _GlitchBurst.
                float cyclePhase = fmod(time, _GlitchCycle);
                float glitchActive = 1.0 - step(_GlitchBurst, cyclePhase);

                // Use noise to generate offsets.
                // First, scale down the UV to reduce computation (like dividing by 5).
                float2 baseUV = i.uv / 5.0;
                float noiseScale = 10.0; // controls noise frequency

                // For each channel, compute a noise value.
                // We add a constant offset (seed) so each channel is different.
                float offsetR = (Noise(baseUV * noiseScale + float2(0.1, 0.1) + time) - 0.5) * _GlitchIntensity * _GlitchOffset;
                float offsetG = (Noise(baseUV * noiseScale + float2(0.3, 0.3) + time) - 0.5) * _GlitchIntensity * _GlitchOffset;
                float offsetB = (Noise(baseUV * noiseScale + float2(0.5, 0.5) + time) - 0.5) * _GlitchIntensity * _GlitchOffset;

                // Only apply offsets during glitch bursts.
                offsetR *= glitchActive;
                offsetG *= glitchActive;
                offsetB *= glitchActive;

                // Incorporate RGB separation.
                offsetR *= _RGBSeparation;
                offsetG *= _RGBSeparation;
                offsetB *= _RGBSeparation;

                // Create modified UVs for each channel.
                float2 uvR = i.uv + float2(offsetR, _Spread);
                float2 uvG = i.uv + float2(_Spread, offsetG);
                float2 uvB = i.uv + float2(-offsetB, -_Spread);

                // Sample the font atlas (SDF texture) three times.
                fixed4 colR = tex2D(_MainTex, uvR);
                fixed4 colG = tex2D(_MainTex, uvG);
                fixed4 colB = tex2D(_MainTex, uvB);

                // Recombine channels: use R from colR, G from colG, and B from colB.
                fixed4 col;
                col.r = colR.r;
                col.g = colG.g;
                col.b = colB.b;
                col.a = (colR.a + colG.a + colB.a) / 3.0;

                // Remap the alpha using SDF smoothing for crisp text edges.
                float sdf = saturate((col.a - _Smoothing) / (1.0 - _Smoothing));
                col.a = sdf;

                return col * _FaceColor;
            }
            ENDCG
        }
    }
    FallBack "TextMeshPro/Distance Field"
}
