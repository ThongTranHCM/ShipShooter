Shader "Custom/Mobile/Particles/HueShiftBullet"
{
    Properties
    {
        _MainTex("Particle Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _HalftoneTex("Halftone Texture", 2D) = "white" {}
        _Distortion("Distortion", float) = 1
        _StartColor("StartColor", COLOR) = (1,1,1,1)
        _MidColor("MidColor", COLOR) = (1,1,1,1)
        _EndColor("EndColor", COLOR) = (1,1,1,1)
        _PatternStrength("Pattern Strength", range(0,1)) = 1
        _Clip("Clip", range(0,1)) = 0.1
        _HueShift("Hue Shift", float) = 1
        _Contrast("Contrast", float) = 0.2
        _LitHue("Lit Hue", Color) = (0.1, 1, 0.1, 1)
        _ShadeHue("Shade Hue", Color) = (1, 0.1, 1, 1)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    sampler2D _NoiseTex;
    float4 _NoiseTex_ST;
    sampler2D _HalftoneTex;
    float4 _HalftoneTex_ST;
    float _Distortion;
    fixed4 _StartColor;
    fixed4 _MidColor;
    fixed4 _EndColor;
    float _Contrast;
    float _PatternStrength;
    float _HueShiftAmount;
    float _Clip;

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

    float4 RGB2YIQ(float4 color)
				{
					const float4  kRGBToYPrime = float4 (0.299, 0.587, 0.114, 0.0);
    				const float4  kRGBToI     = float4 (0.596, -0.275, -0.321, 0.0);
    				const float4  kRGBToQ     = float4 (0.212, -0.523, 0.311, 0.0);
					float   YPrime  = dot (color, kRGBToYPrime);
					float   I      = dot (color, kRGBToI);
					float   Q      = dot (color, kRGBToQ);
					return float4(YPrime,I,Q,0.0);
				}

				float4 YIQ2RGB(float4 color)
				{
					const float4  kYIQToR   = float4 (1.0, 0.956, 0.621, 0.0);
    				const float4  kYIQToG   = float4 (1.0, -0.272, -0.647, 0.0);
    				const float4  kYIQToB   = float4 (1.0, -1.107, 1.704, 0.0);
					float R = dot (color, kYIQToR);
    				float G = dot (color, kYIQToG);
    				float B = dot (color, kYIQToB);
					return float4(R,G,B,0.0);
				}

				float4 ShiftHue(float4 main, float4 sub, float shift_amount, float lumi_scale)
				{
					float4 YIQ_main = RGB2YIQ(main);
					float4 YIQ_sub = RGB2YIQ(sub);

					float cos_main = YIQ_main.y;
					float sin_main = YIQ_main.z;
					float cos_sub = YIQ_sub.y;
					float sin_sub = YIQ_sub.z;
					float sin_shift = sin_main * cos_sub - sin_sub * cos_main;
					float cos_shift = cos_main * cos_sub - sin_main * sin_sub;
					float shift = normalize(atan2(sin_shift, cos_shift)) * 3.1416;
					shift *= abs(lumi_scale) * shift_amount;
					float hue = atan2(sin_main, cos_main);
					hue -= shift;

					float chroma = sqrt(YIQ_main.z * YIQ_main.z + YIQ_main.y * YIQ_main.y);
					
					float lumi = YIQ_main.x;
					lumi += lumi_scale;
					lumi = max(0,min(lumi, 1));

					//chroma correction
					
					float Q = sin(hue);
					float I = cos(hue);
					float R = 0.956 * I + 0.621 * Q;
					float G = -0.272 * I - 0.647 * Q;
					float B = -1.107 * I + 1.704 * Q;
					
					chroma = min(chroma, max((1 - lumi)/R,-lumi/R));
					chroma = min(chroma, max((1 - lumi)/G,-lumi/G));
					chroma = min(chroma, max((1 - lumi)/B,-lumi/B));

					Q *= chroma;
					I *= chroma;

					return YIQ2RGB(float4(lumi,I,Q,0));
				}

    v2f vert(appdata_t v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.worldPos = mul(unity_ObjectToWorld, v.position);
        o.screenPos = ComputeScreenPos(o.position);
        o.texcoord = v.texcoord;
        o.color = v.color;
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        float2 noise_uv = TRANSFORM_TEX(i.worldPos, _NoiseTex);
        float2 world_noise_uv = TRANSFORM_TEX(i.screenPos, _NoiseTex);
        noise_uv += tex2D(_NoiseTex, world_noise_uv.xy * _Distortion).xy;
        float tur = tex2D(_NoiseTex, noise_uv) ;
        float alpha = i.texcoord.y;
        tur = tur * alpha * 4;
        i.screenPos.xy *= normalize(_ScreenParams.xy) * 150 / (i.screenPos.w);
        fixed pattern = tex2D(_HalftoneTex, i.screenPos.xy);
        fixed value = 2 * tur - 2 + pattern;
        fixed4 color = lerp(_StartColor, _MidColor, smoothstep(0,1,value));
        value = 2 * tur - 3 + pattern;
        color = lerp(color, _EndColor, smoothstep(0,1,value));
        return fixed4(color.xyz, 2 * tur - 1 + pattern - _Clip) * i.color;
    }

    ENDCG
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    } 
    CustomEditor "CustomShaderGUI"
}