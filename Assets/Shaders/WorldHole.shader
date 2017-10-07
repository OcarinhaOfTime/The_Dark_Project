Shader "Custom/WorldHole"{
    properties{
        _MainTex("Main Tex", 2D) = ""{}
        _DarkWorld("Dark World", 2D) = ""{}
        _Mask("Mask", 2D) = ""{}
        _Noise("Noise", 2D) = ""{}
        _Color("Color", Color) = (1,1,1,1)
    }

    subshader{
        Tags { "RenderType"="Opaque" }
        pass{
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
                float2 screen_uv : TEXCOORD1;
			};
            
			sampler2D _MainTex;
            sampler2D _Mask;
            sampler2D _Noise;
            sampler2D _DarkWorld;
            fixed4 _Color;

            v2f vert(appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screen_uv = (o.vertex.xy / o.vertex.w + 1) * .5;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                fixed4 alb = tex2D(_MainTex, i.screen_uv);
                fixed4 mask = tex2D(_Mask, i.screen_uv);
                fixed4 dark = tex2D(_DarkWorld, i.screen_uv);

                return alb * (1 - mask.a) + _Color * mask.a * dark;
            }
            ENDCG
        }
    }
}