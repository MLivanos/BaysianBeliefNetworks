Shader "Unlit/GlitchUIScripted"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchIntensity ("Glitch Intensity", Range(0,1)) = 0.5
        _GlitchOffset ("Glitch Offset", Range(0,20)) = 5
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

            // Vertex shader: pass UV coordinates transformed by _MainTex_ST
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            // A simple pseudo-random function
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
            }
            
            // Fragment shader: applies the glitch effect based on a threshold per horizontal segment.
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Create a pseudo-random value for each horizontal segment
                // Multiply uv.y by a constant to divide the image into segments
                float segment = floor(uv.y * 100.0);
                float glitchChance = rand(float2(segment, _Time.y));
                
                // When the random chance is above (1 - intensity), apply an offset
                float glitch = step(1.0 - _GlitchIntensity, glitchChance);
                
                // Calculate a horizontal offset based on another random value
                float offset = glitch * _GlitchOffset * (rand(uv + _Time.y) - 0.5);
                uv.x += offset / _MainTex_ST.x;
                
                // Sample the texture with the modified UV coordinates
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}