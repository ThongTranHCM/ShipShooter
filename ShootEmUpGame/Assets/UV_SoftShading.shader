Shader "Unlit/UV_SoftShading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DensityTex ("Density", 2D) = "white" {}
        _Tint("Tint", COLOR) = (1,1,1,1)
        _StartColor("StartColor", COLOR) = (1,1,1,1)
        _EndColor("EndColor", COLOR) = (1,1,1,1)
        _Contrast("Contrast", float) = 0.5
        _HueShiftAmount("Hue Shift Amount", float) = 1
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
                float3 worldNormal : NORMAL;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DensityTex;
            float4 _DensityTex_ST;
            fixed4 _Tint;
            fixed4 _StartColor;
            fixed4 _EndColor;
            float _HueShiftAmount;
            float _Contrast;

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


            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0);
                fixed4 col = _Tint;
                float offset = tex2D(_MainTex, TRANSFORM_TEX(i.uv,_MainTex));
                float dense = tex2D(_DensityTex, TRANSFORM_TEX(i.uv,_DensityTex));
                float NdotL = offset * dense;
                NdotL *= 2;
                NdotL += 0.25;
                float lumi = RGB2YIQ(col).x;
                fixed4 unlit = ShiftHue(col, _StartColor, _HueShiftAmount, -_Contrast);
                fixed4 lit = ShiftHue(col, _EndColor, _HueShiftAmount, _Contrast);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col = lerp(unlit,col, saturate(NdotL * 2));
                col = lerp(lit,col, saturate(2 - NdotL * 2));
                return col;
            }
            ENDCG
        }
    }
}
