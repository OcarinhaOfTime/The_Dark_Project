Shader "Unlit/Fader"
{
	Properties
	{
		[PerRendererData]_MainTex ("Texture", 2D) = "white" {}
		_Gradient ("Gradient", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		ZWrite Off
		Cull Off
		ZTest Always

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
				half4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _Gradient;
			float4 _MainTex_ST;
			float _Cutoff;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				o.uv1 = o.vertex.xy / o.vertex.w;
				o.uv1 = (o.uv1 + 1) * .5;
				//o.uv1.x = 1 - o.uv1.x;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 grad = tex2D(_Gradient, i.uv1);
				col.rgb = 1 - col.rgb;
				clip(grad.r - _Cutoff);
				return col * i.color * i.color.a;
			}
			ENDCG
		}
	}
}
