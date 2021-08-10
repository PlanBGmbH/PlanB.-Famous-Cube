Shader "NRSDK/NRBackground"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "black" {}
	}

		Subshader
	{
		Pass
		{
			ZWrite Off

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed2 uv = fixed2(i.uv.x, 1 - i.uv.y);
				fixed4 color = tex2D(_MainTex, uv);
				return color;
			}

			ENDCG
		}
	}

		FallBack Off
}
