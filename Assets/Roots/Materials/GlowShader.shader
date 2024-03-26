Shader "UI/GlowOutline"
{
    Properties
    {
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _CenterX ("CenterX", Range(0, 1)) = 0.5
        _CenterY ("CenterY", Range(0, 1)) = 0.5
        _Radius ("Radius", Range(0, 3)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _CenterX;
            float _CenterY;
            float _Radius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.uv = v.uv;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float4 baseColor = tex2D(_MainTex, i.uv) * i.color;
                // Calculate the outline
                fixed4 outline = fixed4(0, 0, 0, 0);
                float2 uvStep = ddx(i.uv) * _OutlineWidth;
                float2 offsets[8] = {
                    float2(0, 0), float2(0, 0), float2(0, 0),
                    float2(0, 0), float2(0, 0),
                    float2(0, 0), float2(0, 0), float2(0, 0)
                    // float2(-1, -1), float2(-1, 0), float2(-1, 1),
                    // float2(0, -1), float2(0, 1),
                    // float2(1, -1), float2(1, 0), float2(1, 1)
                };
                float4 outlineColor = _OutlineColor * _Color;
                for (int j = 0; j < 8; j++)
                {
                    float2 uvOffset = offsets[j] * uvStep;
                    outline += tex2D(_MainTex, i.uv + uvOffset) * outlineColor;
                }
                // Calculate radial glow (distance from the center)
                float2 center = float2(_CenterX, _CenterY);
                float dist = distance(center, i.uv);
                float glow = 1.0 - smoothstep(0.0, _Radius, dist);
                // Apply the radial glow to the outline color
                outlineColor *= glow;
                // Mix the base color and the outline
                return lerp(baseColor, outline, outlineColor.a);
            }
            ENDCG
        }
    }
}
