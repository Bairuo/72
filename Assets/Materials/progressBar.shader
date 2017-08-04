// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UIBar"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Rate("Rate", float) = 1.0
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" "IgnoreProjector" = "True" }

		LOD 100

		Pass
		{
			ZWrite Off

			Blend SrcAlpha One // Hard light.

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 loc : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 loc : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float _Rate;

			v2f vert(appdata v)
			{
				v2f o;
				o.loc = UnityObjectToClipPos(v.loc);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if (i.uv.x > _Rate) discard;
				fixed4 color = tex2D(_MainTex, i.uv);
				color *= i.color;
				return color;
			}
			ENDCG
		}
	}
}
