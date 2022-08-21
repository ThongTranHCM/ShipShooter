Shader "Unlit/TriplanarShader"
{
    Properties
    {
        _MainTexZ ("TextureZ", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _BackgroundColor ("_BackgroundColor", COLOR) = (1,1,1,1)
        _Overlap ("Overlap", Float) = 0
        _FirstColor ("First Color", COLOR) = (1,1,1,1)
        _SecondColor ("Second Color", COLOR) = (1,1,1,1)
        _ThirdColor ("Third Color", COLOR) = (1,1,1,1)
        _RepeatPattern ("Repeat Pattern", Float) = 1
        _Brightness ("Brightness", range(0,1)) = 1
    }
    SubShader
    {
        Tags
		{
			"LightMode" = "ForwardBase"
			"PassFlags" = "OnlyDirectional"
		}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 mask_uv : TEXCOORD0;
                float2 main_uv : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTexZ;
            fixed4 _MainTexZ_ST;
            sampler2D _MaskTex;
            fixed4 _MaskTex_ST;
            fixed4 _BackgroundColor;
            fixed _Overlap;
            fixed _RepeatPattern; 
            fixed4 _FirstColor;
            fixed4 _SecondColor;
            fixed4 _ThirdColor;
            fixed _Brightness;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.mask_uv = TRANSFORM_TEX(v.texcoord, _MaskTex);
                o.main_uv = TRANSFORM_TEX(v.texcoord, _MainTexZ);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed mask = cos(i.mask_uv.y * 3.1416f/2) * cos(i.mask_uv.y * 3.1416f/2);
                fixed sin_mask = sin(i.mask_uv.x / _RepeatPattern  * 3.1416f * 5 );
                mask *= 1 - _Overlap + _Overlap * sin_mask * sin_mask;
                mask = saturate(mask);
                mask *= tex2D(_MainTexZ, TRANSFORM_TEX(i.mask_uv.xy,_MainTexZ));

                fixed y = fmod(i.mask_uv.x, _RepeatPattern) / _RepeatPattern;
                fixed4 color = _FirstColor;
                color = lerp(color,_SecondColor, smoothstep(0,0.25f,y));
                color = lerp(color,_ThirdColor, smoothstep(0.25f,0.5f,y));
                color = lerp(color,_SecondColor, smoothstep(0.5f,0.75f,y));
                color = lerp(color,_FirstColor, smoothstep(0.75f,1.0f,y));
                return _BackgroundColor + color * saturate(mask) * _Brightness;
            }
            ENDCG
        }
    }
    
}
