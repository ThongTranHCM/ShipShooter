Shader "Unlit/TempShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TargetHue ("Target Hue", Color) = (1,1,1,1)
        _PercentHueShift ("Percent Hue Shift", Float) = 0
        _HueShift ("Hue Shift", Float) = 0
        _PercentLumiShift ("PercentLumiShift", Float) = 0
        _LumiShift ("LumiShift", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TargetHue;
            float _PercentHueShift;
            float _HueShift;
            float _PercentLumiShift;
            float _LumiShift;

            
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

				
				float4 ShiftHue(float4 main, float4 sub, float percent_hue_shift, float hue_shift, float lumi_shift)
				{
					float4 YIQ_main = RGB2YIQ(main);
					float4 YIQ_sub = RGB2YIQ(sub);

					float cos_main = YIQ_main.y;
					float sin_main = YIQ_main.z;
					float cos_sub = YIQ_sub.y;
					float sin_sub = YIQ_sub.z;
					float sin_shift = sin_main * cos_sub - sin_sub * cos_main;
					float cos_shift = cos_main * cos_sub - sin_main * sin_sub;
                    float hue_diff = atan2(sin_shift, cos_shift);
                    float hue = atan2(sin_main, cos_main);
                    float chroma = sqrt(YIQ_main.z * YIQ_main.z + YIQ_main.y * YIQ_main.y);
                    hue -= (1 - chroma) * (hue_diff * percent_hue_shift + normalize(hue_diff) * hue_shift) * 3.1416 / 2;

					float lumi = YIQ_main.x;
					lumi += lumi_shift;
					lumi = max(0,min(lumi, 1));
					
					//chroma correction
					
					float Q = sin(hue);
					float I = cos(hue);
					float R = 0.956 * I + 0.621 * Q;
					float G = -0.272 * I - 0.647 * Q;
					float B = -1.107 * I + 1.704 * Q;
					
					chroma = max(chroma, min((1 - lumi)/R,-lumi/R));
					chroma = max(chroma, min((1 - lumi)/G,-lumi/G));
					chroma = max(chroma, min((1 - lumi)/B,-lumi/B));

					Q *= chroma;
					I *= chroma;

					return YIQ2RGB(float4(lumi,I,Q,0));
				}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 sample = tex2D(_MainTex, i.uv);
				float3 YIQ = RGB2YIQ(sample);
				float lumi = YIQ.x;
				float chroma = sqrt(YIQ.z * YIQ.z + YIQ.y * YIQ.y);
					
                sample = ShiftHue(sample, _TargetHue, 0, _HueShift * abs(lumi * (1 - chroma) * _PercentLumiShift + _LumiShift), lumi * _PercentLumiShift + _LumiShift);
                return sample;
            }
            ENDCG
        }
    }
}
