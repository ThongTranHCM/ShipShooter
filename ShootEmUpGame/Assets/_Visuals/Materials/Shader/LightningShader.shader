Shader "Custom/Mobile/Particles/LightningShader"
{
    Properties
    {
        _MainTex("Rectangle", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _StartColor("StartColor", COLOR) = (1,1,1,1)
        _MidColor("MidColor", COLOR) = (1,1,1,1)
        _Distortion("Distortion", float) = 1
        _FakeTime("FakeTime", float) = 0.4
    }

        CGINCLUDE

#include "UnityCG.cginc"

            sampler2D _MainTex;
        float4 _MainTex_ST;
        sampler2D _NoiseTex;
        float4 _NoiseTex_ST;
        fixed4 _StartColor;
        fixed4 _MidColor;
        float _Distortion;
        float _FakeTime;

        struct appdata_t
        {
            float4 position : POSITION;
            float4 texcoord : TEXCOORD0;
            fixed4 color : COLOR;
        };

        struct v2f
        {
            float4 position : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            float4 screenPos : TEXCOORD1;
            float4 worldPos : TEXCOORD2;
            fixed4 color : COLOR;
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            o.position = UnityObjectToClipPos(v.position);
            o.worldPos = mul(unity_ObjectToWorld, v.position);
            o.screenPos = ComputeScreenPos(v.position);
            o.texcoord = v.texcoord;
            o.color = v.color;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            _FakeTime = _Time;
            float delta_time = _FakeTime - int(_FakeTime);
            float2 noise_uv = TRANSFORM_TEX(i.worldPos, _NoiseTex) + float2(0.2, 0.1) * _FakeTime;
            float2 world_noise_uv = TRANSFORM_TEX(i.screenPos, _NoiseTex);
            noise_uv += tex2D(_NoiseTex, world_noise_uv.xy * _Distortion).xy;
            fixed4 noise_col = tex2D(_NoiseTex, noise_uv);
            noise_col = fixed4(lerp(i.texcoord.x, noise_col.x, 0.3f), lerp(i.texcoord.y, noise_col.y, 0.3f), 1, 1);


            fixed4 color = tex2D(_MainTex, noise_col);
            float ave = (color.x + color.y + color.z) / 3;
            color.w = ave;
            color *= _StartColor;
            return color;
        }

            ENDCG
            SubShader
        {
            Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
                Blend SrcAlpha OneMinusSrcAlpha
                Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                ENDCG
            }
        }
}
