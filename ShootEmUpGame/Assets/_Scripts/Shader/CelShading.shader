Shader "Unlit/CelShading"
{
    Properties
	{
		_LightenColor("Lighten Color", Color) = (0,0,0,1)
		_MainTex("Main Texture", 2D) = "white" {}
        _LitTex("Lit Texture", 2D) = "white" {}
        _UnlitTex("Unlit Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Range(0, 1)) = 0.002
		_AmbientLight ("Ambient Light", Range(0, 1)) = 0
		_Brightness ("Brightness", float) = 0
	}
		SubShader
		{
			Pass
			{
				Tags
				{
					"LightMode" = "ForwardBase"
					"PassFlags" = "OnlyDirectional"
				}

				Cull Back

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				struct v2f
				{
					fixed4 pos : SV_POSITION;
					fixed4 scrPos : TEXCOORD1;
					fixed3 worldNormal : NORMAL;
					fixed2 uv : TEXCOORD0;
					fixed4 base : COLOR0;
					fixed4 lit : COLOR1;
					fixed4 unlit : COLOR2;
				};

				sampler2D _MainTex;
				fixed4 _MainTex_ST;
                sampler2D _LitTex;
				fixed4 _LitTex_ST;
                sampler2D _UnlitTex;
				fixed4 _UnlitTex_ST;
				fixed _Brightness;
				fixed _AmbientLight;
				fixed4 _LightenColor;
				
				fixed4 Screen(fixed4 a, fixed4 b){
					fixed4 result = 1 - (1 - a) * (1 - b);
					return result;
				}

				v2f vert(appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.scrPos = ComputeScreenPos(o.pos);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.base = tex2Dlod(_MainTex, fixed4(o.uv,0,0)) + _Brightness;
					o.lit = tex2Dlod(_LitTex, fixed4(o.uv,0,0)) + _Brightness;
					o.unlit = tex2Dlod(_UnlitTex, fixed4(o.uv,0,0)) + _Brightness;
					return o;
				}

				fixed SpecularLighting(fixed3 LightDir, fixed3 Normal, fixed3 ViewDir){
					return dot(ViewDir, normalize(reflect(LightDir, Normal)));
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed3 normal = normalize(i.worldNormal);
					fixed3 viewDir = normalize(mul((fixed3x3)unity_CameraToWorld, fixed3(0,0,1)));
					fixed3 lightDir = normalize(_WorldSpaceLightPos0);

					fixed NdotL = dot(lightDir, normal);
					fixed specular = saturate(SpecularLighting(lightDir, normal, viewDir));

					fixed light = NdotL;
					fixed4 color = lerp(i.unlit, i.base, saturate(light + _AmbientLight));
					color = lerp(color, i.lit, saturate(pow(specular,2)));
					color = Screen(_LightColor0 * smoothstep(0,0.2f,light), color);
					color = Screen(_LightenColor, color);

					return color;
				}
				ENDCG
			}
			Pass
			{
				Tags
				{
					"LightMode" = "ForwardBase"
					"PassFlags" = "OnlyDirectional"
				}

				Cull Front

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				struct v2f
				{
					fixed4 pos : SV_POSITION;
					fixed4 scrPos : TEXCOORD1;
					fixed3 worldNormal : NORMAL;
					fixed2 uv : TEXCOORD0;
					fixed4 base : COLOR0;
					fixed4 lit : COLOR1;
					fixed4 unlit : COLOR2;
				};

				sampler2D _MainTex;
				fixed4 _MainTex_ST;
                sampler2D _LitTex;
				fixed4 _LitTex_ST;
                sampler2D _UnlitTex;
				fixed4 _UnlitTex_ST;
				fixed _SpecularThreshold;
				fixed _LitThreshold;
				fixed _HighlightInterval;
				fixed _HighlightStrength;
				fixed _Brightness;
				fixed4 _LightenColor;
				fixed4 _OutlineColor;
				fixed _OutlineWidth;

				v2f vert(appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					//o.worldNormal = UnityObjectToWorldNormal(v.normal);
					//o.scrPos = ComputeScreenPos(o.pos);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.base = tex2Dlod(_MainTex, fixed4(o.uv,0,0)) + _Brightness;
					//o.lit = tex2Dlod(_LitTex, fixed4(o.uv,0,0)) + _Brightness;
					o.unlit = tex2Dlod(_UnlitTex, fixed4(o.uv,0,0)) + _Brightness;
					fixed3 clipNormal = mul((fixed3x3) UNITY_MATRIX_VP, mul((fixed3x3) UNITY_MATRIX_M, v.normal));
					fixed2 offset = normalize(clipNormal.xy) / normalize(_ScreenParams.xy) * _OutlineWidth * o.pos.w * 2;
					o.pos.xy += offset;
					return o;
				}

				fixed SpecularLighting(fixed3 LightDir, fixed3 Normal, fixed3 ViewDir){
					return dot(ViewDir, normalize(reflect(LightDir, Normal)));
				}

				fixed4 frag(v2f i) : SV_Target
				{
					/*
					fixed3 normal = normalize(i.worldNormal);
					fixed3 viewDir = normalize(mul((fixed3x3)unity_CameraToWorld, fixed3(0,0,1)));
					fixed3 lightDir = normalize(_WorldSpaceLightPos0);

					fixed NdotL = dot(lightDir, normal);
					fixed specular = SpecularLighting(lightDir, normal, viewDir);

					fixed light = NdotL;
					fixed4 color = lerp(i.unlit, i.base, step(_LitThreshold,light));
					color = lerp(color, i.lit, step(_SpecularThreshold,specular));
					*/
					fixed4 color = fixed4(i.unlit.rgb * 0.5f, 1);
					color = 1 - (1 - color)*(1 - _LightenColor);
					return color;
				}
				ENDCG
			}
			// Shadow casting support.
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
    CustomEditor "CelShadingCustomGUI"
}